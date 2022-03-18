using LifeAssistant.Core.Domain.Entities;

namespace LifeAssistant.Core.Application.Users.Contracts;

public record RegisterResponse(
    Guid Id,
    string Username, 
    string Role, 
    string FirstName,
    string LastName,
    bool Validated
);