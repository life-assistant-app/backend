using LifeAssistant.Core.Domain.Entities;

namespace LifeAssistant.Web.Database.Entities;

public class ApplicationUserEntity : BaseDbEntity
{
    public ApplicationUserEntity()
    {
    }

    public ApplicationUserEntity(ApplicationUser applicationUser)
    {
        this.Id = applicationUser.Id;
        this.UserName = applicationUser.UserName;
        this.Password = applicationUser.Password;
        this.FirstName = applicationUser.FirstName;
        this.LastName = applicationUser.LastName;
        this.Role = applicationUser.Role;
        this.Validated = applicationUser.Validated;
    }

    public string UserName { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ApplicationUserRole Role { get; set; }
    public bool Validated { get; set; }

    public ApplicationUser ToDomainEntity()
    {
        return new ApplicationUser(
            this.Id, 
            this.UserName,
            this.Password,
            this.FirstName,
            this.LastName, 
            this.Role,
            this.Validated
        );
    }
}