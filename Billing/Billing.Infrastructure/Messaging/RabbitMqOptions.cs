namespace Billing.Infrastructure.Messaging;

public class RabbitMqOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";

    // Single shared exchange — all services publish and consume from here
    public string ExchangeName { get; set; } = "booking";

    // Queue Billing consumes from (ReservationCheckedInEvent + CheckInRevertedEvent from Booking)
    public string QueueName { get; set; } = "billing.reservation-events";
}
