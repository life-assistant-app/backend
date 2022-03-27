using LifeAssistant.Core.Domain.Entities;

namespace LifeAssistant.Core.Application.Users.Contracts;

public record GetUserResponse(
    Guid Id,
    string UserName,
    string FirstName,
    string LastName,
    ApplicationUserRole Role
);