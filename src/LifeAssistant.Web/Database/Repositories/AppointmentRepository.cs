using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Domain.Entities.Appointments;
using LifeAssistant.Core.Domain.Entities.AppointmentState;
using LifeAssistant.Core.Persistence;
using LifeAssistant.Web.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeAssistant.Web.Database.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly ApplicationDbContext context;
    private readonly IAppointmentStateFactory appointmentStateFactory;

    public AppointmentRepository(ApplicationDbContext context, IAppointmentStateFactory appointmentStateFactory)
    {
        this.context = context;
        this.appointmentStateFactory = appointmentStateFactory;
    }

    public async Task<List<Appointment>> FindAppointmentsToDelete()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return await this.context
            .Appointments
            .Where(appointment =>
                (appointment.State == "Planned" && appointment.CreatedDate.AddDays(14) < today)
                ||
                (appointment.State == "Finished" && appointment.CreatedDate < today.AddYears(-1))
                ||
                (appointment.State == "Refused")
            )
            .Select(appointment => appointment.ToDomainEntity(this.appointmentStateFactory))
            .ToListAsync();
    }

    public async Task<List<Appointment>> FindAppointments(int pageIndex)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAppointments(List<Appointment> appointments)
    {
        ISet<Guid> appointmentToDeleteIds =
            appointments
                .Select(appointment => appointment.Id)
                .ToHashSet();

        AppointmentEntity[] entitiesToDelete = await this.context
            .Appointments
            .Where(entity => appointmentToDeleteIds.Contains(entity.Id))
            .ToArrayAsync();

        this.context.Appointments.RemoveRange(entitiesToDelete);
    }

    public async Task Save()
    {
        await this.context.SaveChangesAsync();
    }
}