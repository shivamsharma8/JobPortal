using System.Text.Json;
using InterviewService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace InterviewService.Infrastructure.Services;

public sealed class RabbitMqOptions
{
    public bool Enabled { get; set; } = true;
    public string HostName { get; set; } = "rabbitmq";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "hireconnect";
    public string Password { get; set; } = "hireconnect_rabbit_pass";
    public string VirtualHost { get; set; } = "hireconnect";
    public string ExchangeName { get; set; } = "hireconnect.events";
}

public class RabbitMqEventPublisher(IOptions<RabbitMqOptions> options, ILogger<RabbitMqEventPublisher> logger) : IEventPublisher
{
    public Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : class
    {
        if (!options.Value.Enabled)
        {
            logger.LogInformation("RabbitMQ disabled. Skipping publish for {EventType}.", typeof(T).Name);
            return Task.CompletedTask;
        }

        var factory = new ConnectionFactory
        {
            HostName = options.Value.HostName,
            Port = options.Value.Port,
            UserName = options.Value.UserName,
            Password = options.Value.Password,
            VirtualHost = options.Value.VirtualHost,
            DispatchConsumersAsync = true
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(options.Value.ExchangeName, ExchangeType.Topic, durable: true, autoDelete: false);

        var payload = JsonSerializer.SerializeToUtf8Bytes(@event, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";
        properties.Type = typeof(T).Name;

        channel.BasicPublish(options.Value.ExchangeName, typeof(T).Name, properties, payload);
        logger.LogInformation("Published event {EventType} to RabbitMQ.", typeof(T).Name);

        return Task.CompletedTask;
    }
}
