namespace LifeAssistant.Core.Domain.Exceptions;

public class IllegalAccessException : InvalidOperationException
{
    public IllegalAccessException(string message) : base(message)
    {
    }
}