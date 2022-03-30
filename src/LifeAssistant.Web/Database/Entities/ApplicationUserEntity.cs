using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Domain.Entities.ApplicationUser;

namespace LifeAssistant.Web.Database.Entities;

public class ApplicationUserEntity : BaseDbEntity
{
    public ApplicationUserEntity()
    {
    }

    public ApplicationUserEntity(IApplicationUserWithAppointments applicationUser)
    {
        this.Id = applicationUser.Id;
        this.UserName = applicationUser.UserName;
        this.Password = applicationUser.Password;
        this.FirstName = applicationUser.FirstName;
        this.LastName = applicationUser.LastName;
        this.Role = applicationUser.Role;
        this.Validated = applicationUser.Validated;
        this.Appointments = applicationUser.Appointments
            .Select(appointment => new AppointmentEntity(appointment))
            .ToList();
    }

    public string UserName { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ApplicationUserRole Role { get; set; }
    public bool Validated { get; set; }
    public List<AppointmentEntity> Appointments { get; set; } = new();

    public ApplicationUser ToDomainEntity(IAppointmentStateFactory factory)
    {
        return new ApplicationUser(
            this.Id,
            this.UserName,
            this.Password,
            this.FirstName,
            this.LastName,
            this.Role,
            this.Validated,
            this.Appointments
                .Select(appointment => appointment.ToDomainEntity(factory))
                .ToList()
        );
    }
}