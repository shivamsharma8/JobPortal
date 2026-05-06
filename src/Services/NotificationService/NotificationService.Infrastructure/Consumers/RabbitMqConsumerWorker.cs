using System.Text;
using System.Text.Json;
using BuildingBlocks.Contracts.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationService.Application.Services;
using NotificationService.Domain.Enums;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationService.Infrastructure.Consumers;

public sealed class RabbitMqOptions
{
    public bool Enabled { get; set; } = true;
    public string HostName { get; set; } = "rabbitmq";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "hireconnect";
    public string Password { get; set; } = "hireconnect_rabbit_pass";
    public string VirtualHost { get; set; } = "hireconnect";
    public string ExchangeName { get; set; } = "hireconnect.events";
    public string QueueName { get; set; } = "notification_service_queue";
}

public class RabbitMqConsumerWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMqConsumerWorker> _logger;
    private readonly RabbitMqOptions _options;
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMqConsumerWorker(
        IServiceProvider serviceProvider,
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqConsumerWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("RabbitMQ consumer is disabled in settings.");
            return Task.CompletedTask;
        }

        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(_options.ExchangeName, ExchangeType.Topic, durable: true, autoDelete: false);
            _channel.QueueDeclare(_options.QueueName, durable: true, exclusive: false, autoDelete: false);

            _channel.QueueBind(_options.QueueName, _options.ExchangeName, nameof(ApplicationSubmittedEvent));
            _channel.QueueBind(_options.QueueName, _options.ExchangeName, nameof(ApplicationStatusChangedEvent));
            _channel.QueueBind(_options.QueueName, _options.ExchangeName, nameof(InterviewScheduledEvent));
            _channel.QueueBind(_options.QueueName, _options.ExchangeName, nameof(InterviewUpdatedEvent));

            _logger.LogInformation("RabbitMQ Consumer started listening on queue {QueueName}.", _options.QueueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize RabbitMQ consumer.");
        }

        return base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel is null) return Task.CompletedTask;

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var type = ea.BasicProperties.Type;

                _logger.LogInformation("Received event {EventType}", type);

                using var scope = _serviceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<NotificationAppService>();

                if (type == nameof(ApplicationSubmittedEvent))
                {
                    var @event = JsonSerializer.Deserialize<ApplicationSubmittedEvent>(message, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                    if (@event != null)
                    {
                        await service.CreateNotificationAsync(
                            @event.CandidateId,
                            "Application Submitted",
                            "Your application has been successfully submitted.",
                            NotificationType.ApplicationSubmitted,
                            @event.ApplicationId,
                            stoppingToken);

                        await service.CreateNotificationAsync(
                            @event.RecruiterId,
                            "New Application Received",
                            "A new candidate has applied to your job.",
                            NotificationType.ApplicationSubmitted,
                            @event.ApplicationId,
                            stoppingToken);
                    }
                }
                else if (type == nameof(ApplicationStatusChangedEvent))
                {
                    var @event = JsonSerializer.Deserialize<ApplicationStatusChangedEvent>(message, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                    if (@event != null)
                    {
                        await service.CreateNotificationAsync(
                            @event.CandidateId,
                            "Application Status Update",
                            $"Your application status has changed to {@event.NewStatus}.",
                            NotificationType.StatusChanged,
                            @event.ApplicationId,
                            stoppingToken);
                    }
                }
                else if (type == nameof(InterviewScheduledEvent))
                {
                    var @event = JsonSerializer.Deserialize<InterviewScheduledEvent>(message, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                    if (@event != null)
                    {
                        await service.CreateNotificationAsync(
                            @event.CandidateId,
                            "Interview Scheduled",
                            $"An interview has been scheduled for {@event.ScheduledAt:g}.",
                            NotificationType.InterviewScheduled,
                            @event.InterviewId,
                            stoppingToken);
                    }
                }
                else if (type == nameof(InterviewUpdatedEvent))
                {
                    var @event = JsonSerializer.Deserialize<InterviewUpdatedEvent>(message, new JsonSerializerOptions(JsonSerializerDefaults.Web));
                    if (@event != null)
                    {
                        await service.CreateNotificationAsync(
                            @event.CandidateId,
                            "Interview Update",
                            $"Your interview status has changed to {@event.NewStatus}.",
                            NotificationType.InterviewUpdated,
                            @event.InterviewId,
                            stoppingToken);

                        await service.CreateNotificationAsync(
                            @event.RecruiterId,
                            "Interview Update",
                            $"Candidate interview status changed to {@event.NewStatus}.",
                            NotificationType.InterviewUpdated,
                            @event.InterviewId,
                            stoppingToken);
                    }
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing event");
                _channel.BasicNack(ea.DeliveryTag, false, requeue: false);
            }
        };

        _channel.BasicConsume(queue: _options.QueueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Dispose();
        _connection?.Dispose();
        return base.StopAsync(cancellationToken);
    }
}
