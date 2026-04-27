using AuthService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.Services;

public class EventPublisher(ILogger<EventPublisher> logger) : IEventPublisher
{
    // RabbitMQ / MassTransit ready — wire up IPublishEndpoint here when MassTransit is configured.
    public Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : class
    {
        logger.LogInformation("Publishing event {EventType}: {@Event}", typeof(T).Name, @event);
        // TODO: inject IPublishEndpoint from MassTransit and call PublishAsync
        return Task.CompletedTask;
    }
}
