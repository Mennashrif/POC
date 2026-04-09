using Booking.Domain.Abstractions;

namespace Booking.Domain.Models
{
    public class Transaction:BaseEntity<Guid>
    {
        public string Name { get; set; }
        public Transaction(string name) : base(Guid.NewGuid())
        {
            Name = name;
        }
    }
}
