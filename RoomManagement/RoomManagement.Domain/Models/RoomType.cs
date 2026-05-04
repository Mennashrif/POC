using RoomManagement.Domain.Abstractions;

namespace RoomManagement.Domain.Models;

public class RoomType : BaseEntity<Guid>
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }

    private RoomType() : base(Guid.NewGuid()) { }

    public RoomType(string name, decimal price) : base(Guid.NewGuid())
    {
        Name = name;
        Price = price;
    }

    public void Update(string name, decimal price)
    {
        Name = name;
        Price = price;
    }
}
