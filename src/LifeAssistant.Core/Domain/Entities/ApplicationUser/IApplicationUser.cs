namespace LifeAssistant.Core.Domain.Entities;

public interface IApplicationUser
{
    string UserName { get; }
    string Password { get; }
    string FirstName { get; }
    string LastName { get; }
    ApplicationUserRole Role { get; }
    bool Validated { get; set; }
    Guid Id { get; }
}