using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Persistence;
using LifeAssistant.Web.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeAssistant.Web.Database.Repositories;

public class ApplicationUserRepository : IApplicationUserRepository
{
    private readonly ApplicationDbContext context;
    private readonly IAppointmentStateFactory appointmentStateFactory;

    public ApplicationUserRepository(ApplicationDbContext context, IAppointmentStateFactory appointmentStateFactory)
    {
        this.context = context;
        this.appointmentStateFactory = appointmentStateFactory;
    }

    public async Task<IApplicationUser> FindByUsername(string username)
    {
        ApplicationUserEntity entity = await this.context
            .Users
            .FirstAsync(user => user.UserName == username);

        return entity.ToDomainEntity(appointmentStateFactory);
    }

    public async Task<IApplicationUserWithAppointments> FindByIdWithAppointments(Guid entityId)
    {
        ApplicationUserEntity entity = await FindEntityById(entityId);

        return entity.ToDomainEntity(appointmentStateFactory);
    }

    private async Task<ApplicationUserEntity> FindEntityById(Guid entityId)
    {
        return await this.context
            .Users
            .Include(user => user.Appointments)
            .FirstAsync(user => user.Id == entityId);
    }


    public async Task Insert(IApplicationUserWithAppointments domainEntity)
    {
        ApplicationUserEntity entity = new ApplicationUserEntity(domainEntity);
        await this.context.Users.AddAsync(entity);
    }

    public async Task Update(IApplicationUserWithAppointments entity)
    {
        ApplicationUserEntity user = await FindEntityById(entity.Id);

        user.UserName = entity.UserName;
        user.Password = entity.Password;
        user.FirstName = entity.FirstName;
        user.LastName = entity.LastName;
        user.Role = entity.Role;
        user.Validated = entity.Validated;
        user.Appointments = entity.Appointments.Select(appointment => new AppointmentEntity(appointment)).ToList();

        this.context.Update(user);
    }

    public async Task<IApplicationUser> FindById(Guid entityId)
    {
        ApplicationUserEntity applicationUserEntity = await this.context.Users
            .FirstAsync(u => u.Id == entityId);
        return applicationUserEntity.ToDomainEntity(this.appointmentStateFactory);
    }

    public async Task Save()
    {
        await this.context.SaveChangesAsync();
    }
}