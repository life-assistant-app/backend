namespace LifeAssistant.Core.Application.Users.Contracts;

public record RegisterRequest(string Username, string Password, string Role);