using LifeAssistant.Core.Domain.Entities;

namespace LifeAssistant.Core.Persistence;

public interface IAppointmentRepository
{
    Task<IList<Appointment>> GetAppointments();
}