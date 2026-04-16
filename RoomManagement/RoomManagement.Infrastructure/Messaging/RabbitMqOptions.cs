namespace RoomManagement.Infrastructure.Messaging;

public class RabbitMqOptions
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    // Single shared exchange — all services publish and consume from here
    public string ExchangeName { get; set; } = "booking";

    // Queue RoomManagement consumes from (ReservationCheckedInEvent from Booking)
    public string ReservationQueueName { get; set; } = "roommanagement.reservation-events";
}
