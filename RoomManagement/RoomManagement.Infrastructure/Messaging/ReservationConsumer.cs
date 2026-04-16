using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RoomManagement.Application.Commands;
using System.Text;
using System.Text.Json;

namespace RoomManagement.Infrastructure.Messaging;

public class ReservationConsumer : BackgroundService
{
    private readonly RabbitMqOptions _options;
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection? _connection;
    private IChannel? _channel;

    public ReservationConsumer(IOptions<RabbitMqOptions> options, IServiceScopeFactory scopeFactory)
    {
        _options = options.Value;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.Username,
            Password = _options.Password
        };

        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            var body = Encoding.UTF8.GetString(ea.Body.ToArray());
            var eventType = ea.RoutingKey;
            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            if (eventType == "ReservationCheckedInEvent")
            {
                var message = JsonSerializer.Deserialize<ReservationCheckedInMessage>(body, jsonOptions);
                if (message is not null)
                    await mediator.Send(new CheckInRoomsCommand(
                        message.ReservationId,
                        message.PhysicalRoomIds,
                        message.OccurredAt
                    ), stoppingToken);
            }


            await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
        };

        await _channel.BasicConsumeAsync(
            queue: _options.ReservationQueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null) await _channel.DisposeAsync();
        if (_connection is not null) await _connection.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}

internal record ReservationCheckedInMessage(
    Guid ReservationId,
    List<string> PhysicalRoomIds,
    string GuestName,
    DateTime CheckInDate,
    DateTime OccurredAt
);

internal record ReservationConfirmedMessage(
    Guid Id,
    string Name,
    string Email,
    DateTime CheckIn,
    DateTime CheckOut
);
