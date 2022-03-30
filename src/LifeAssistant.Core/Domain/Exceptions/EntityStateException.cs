namespace LifeAssistant.Core.Domain.Exceptions;

public class EntityStateException : InvalidOperationException
{
    public EntityStateException(string message) : base(message)
    {
    }
}