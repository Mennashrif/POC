namespace RoomManagement.Domain.Abstractions;

public abstract class BaseEntity<TId>
{
    public TId Id { get; protected set; }

    protected BaseEntity(TId id)
    {
        Id = id;
    }

}
