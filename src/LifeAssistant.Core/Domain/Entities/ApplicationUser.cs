using System;

namespace LifeAssistant.Core.Domain.Entities;

public class ApplicationUser : BaseEntity
{
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

    public ApplicationUser(Guid id, string username, string password, ApplicationUserRole role,
        bool validated) : base(id)
    {
        Username = username;
        Password = password;
        Role = role;
        Validated = validated;
    }

    public ApplicationUser(string username, string password, ApplicationUserRole role) : base(Guid.NewGuid())
    {
        Validated = false;
        Username = username;
        Password = password;
        Role = role;
    }
}