using LifeAssistant.Core.Domain.Entities;

namespace LifeAssistant.Core.Application.Users.Contracts;

public record RegisterResponse(ApplicationUserId Id, string Username, string Role, bool Validated);