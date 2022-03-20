using System;

namespace LifeAssistant.Core.Domain.Entities;

public class ApplicationUser : BaseEntity
{
    private string userName;

    public string UserName
    {
        get => userName;
        private set => userName = string.IsNullOrEmpty(value) ? throw new ArgumentException() : value;
    }

    private string password;

    public string Password
    {
        get => password;
        private set => password = string.IsNullOrEmpty(value) ? throw new ArgumentException() : value;
    }

    private string firstName;

    public string FirstName
    {
        get => firstName;
        private set => firstName = string.IsNullOrEmpty(value) ? throw new ArgumentException() : value;
    }
    
    private string lastName;
    
    public string LastName
    {
        get => lastName;
        private set => lastName = string.IsNullOrEmpty(value) ? throw new ArgumentException() : value;
    }
    
    public ApplicationUserRole Role { get; }
    public bool Validated { get; set; }

    public List<Appointment> Appointments { get; }
    
    public ApplicationUser(Guid id,
        string userName,
        string password,
        string firstname, 
        string lastname, 
        ApplicationUserRole role,
        bool validated, List<Appo) : base(id)
    {
        UserName = userName;
        Password = password;
        FirstName = firstname;
        LastName = lastname;
        Role = role;
        Validated = validated;
        Appointments = new List<Appointment>();
    }

    public ApplicationUser(string userName, string password,string firstname, string lastname, ApplicationUserRole role) 
        : this(Guid.NewGuid(), userName, password, firstname, lastname, role, false)
    {
    }
}