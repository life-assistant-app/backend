using LifeAssistant.Core.Domain.Entities;
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

    public async Task<IList<Appointment>> GetAppointments()
    {
        var appointmentEntities = await this.context
            .Appointments
            .OrderBy(appointment => appointment.DateTime)
            .ToListAsync();
        return appointmentEntities
            .Select(CreateAppointmentFromEntity)
            .ToList();
    }

    private Appointment CreateAppointmentFromEntity(AppointmentEntity entity)
    {
        return new Appointment(entity.Id, entity.DateTime, appointmentStateFactory, entity.State);
    }
}