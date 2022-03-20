namespace LifeAssistant.Core.Domain.Entities;

public abstract class Entity
{
    protected Entity(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}