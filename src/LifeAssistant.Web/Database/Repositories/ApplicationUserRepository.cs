using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Domain.Exceptions;
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
        ApplicationUserEntity? applicationUserEntity = await this.context
            .Users
            .FirstOrDefaultAsync(user => user.UserName == username);

        if (applicationUserEntity is null)
        {
            throw new EntityNotFoundException($"No user with username : {username}");
        }

        return applicationUserEntity.ToDomainEntity(appointmentStateFactory);
    }

    public async Task<IApplicationUserWithAppointments> FindByIdWithAppointments(Guid entityId)
    {
        ApplicationUserEntity entity = await FindEntityById(entityId);

        return entity.ToDomainEntity(appointmentStateFactory);
    }

    public async Task<List<IApplicationUser>> FindValidatedByRole(ApplicationUserRole role)
    {
        List<ApplicationUserEntity> applicationUserEntities = await this.context
            .Users
            .Where(user => user.Role == role)
            .Where(user => user.Validated)
            .ToListAsync();

        return applicationUserEntities
            .Select(entity => entity.ToDomainEntity(this.appointmentStateFactory) as IApplicationUser)
            .ToList();
    }

    public async Task<List<IApplicationUserWithAppointments>> FindValidatedWithAppointmentByRole(
        ApplicationUserRole role)
    {
        List<ApplicationUserEntity> applicationUserEntities = await this.context
            .Users
            .Include(user => user.Appointments)
            .Where(user => user.Role == role)
            .Where(user => user.Validated)
            .ToListAsync();

        return applicationUserEntities
            .Select(entity => entity.ToDomainEntity(this.appointmentStateFactory) as IApplicationUserWithAppointments)
            .ToList();
    }

    private async Task<ApplicationUserEntity> FindEntityById(Guid entityId)
    {
        ApplicationUserEntity? applicationUserEntity = await this.context
            .Users
            .Include(user => user.Appointments)
            .FirstOrDefaultAsync(user => user.Id == entityId);

        if (applicationUserEntity is null)
        {
            throw new EntityNotFoundException($"No user with id : {entityId}");
        }

        return applicationUserEntity;
    }


    public async Task Insert(IApplicationUserWithAppointments domainEntity)
    {
        ApplicationUserEntity entity = new ApplicationUserEntity(domainEntity);
        await this.context.Users.AddAsync(entity);
    }

    public async Task Update(IApplicationUserWithAppointments domainUserEntity)
    {
        ApplicationUserEntity persistenceUserEntity = await FindEntityById(domainUserEntity.Id);

        IDictionary<Guid, AppointmentEntity> appointmentEntitiesById =
            persistenceUserEntity.Appointments
                .ToDictionary(appointmentEntity => appointmentEntity.Id, appointmentEntity => appointmentEntity);

        persistenceUserEntity.UserName = domainUserEntity.UserName;
        persistenceUserEntity.Password = domainUserEntity.Password;
        persistenceUserEntity.FirstName = domainUserEntity.FirstName;
        persistenceUserEntity.LastName = domainUserEntity.LastName;
        persistenceUserEntity.Role = domainUserEntity.Role;
        persistenceUserEntity.Validated = domainUserEntity.Validated;
        persistenceUserEntity.Appointments = domainUserEntity.Appointments
            .Select(appointment => BuildOrUpdateAppointmentEntity(appointmentEntitiesById, appointment))
            .ToList();

        this.context.Users.Update(persistenceUserEntity);
    }

    private static AppointmentEntity BuildOrUpdateAppointmentEntity(IDictionary<Guid, AppointmentEntity> appointmentEntitiesById,
        Appointment appointment)
    {
        if (!appointmentEntitiesById.ContainsKey(appointment.Id))
        {
            return new AppointmentEntity(appointment);
        }

        AppointmentEntity appointmentEntity = appointmentEntitiesById[appointment.Id];
        appointmentEntity.State = appointment.State.Name;
        appointmentEntity.DateTime = appointment.DateTime;
        return appointmentEntity;
    }

    public async Task<IApplicationUser> FindById(Guid entityId)
    {
        ApplicationUserEntity? applicationUserEntity = await this.context.Users
            .FirstOrDefaultAsync(u => u.Id == entityId);

        if (applicationUserEntity is null)
        {
            throw new EntityNotFoundException($"No user with id : {entityId}");
        }

        return applicationUserEntity.ToDomainEntity(this.appointmentStateFactory);
    }

    public async Task Save()
    {
        await this.context.SaveChangesAsync();
    }
}