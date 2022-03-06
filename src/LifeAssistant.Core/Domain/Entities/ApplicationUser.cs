using System;

namespace LifeAssistant.Core.Domain.Entities;

public class ApplicationUser
{
    public ApplicationUserId Id { get; }

    private string username;

    public string Username
    {
        get => username;
        private set => username = string.IsNullOrEmpty(value) ? throw new ArgumentException() : value;
    }

    private string password;

    public string Password
    {
        get => password;
        private set => password = string.IsNullOrEmpty(value) ? throw new ArgumentException() : value;
    }

    public ApplicationUserRole Role { get; }
    public bool Validated { get; set; }

    public ApplicationUser(ApplicationUserId id, string username, string password, ApplicationUserRole role,
        bool validated)
    {
        Id = id;
        Username = username;
        Password = password;
        Role = role;
        Validated = validated;
    }

    public ApplicationUser(string username, string password, ApplicationUserRole role)
    {
        Id = ApplicationUserId.New();
        Validated = false;
        Username = username;
        Password = password;
        Role = role;
    }
}