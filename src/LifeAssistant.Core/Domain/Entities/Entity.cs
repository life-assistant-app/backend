namespace LifeAssistant.Core.Domain.Entities;

public class BaseEntity
{
    public Guid Id { get; }

    public BaseEntity(Guid id)
    {
        Id = id;
    }
}