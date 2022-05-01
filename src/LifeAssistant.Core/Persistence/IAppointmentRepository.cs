using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Domain.Entities.Appointments;

namespace LifeAssistant.Core.Persistence;

public interface IAppointmentRepository
{
    Task<List<Appointment>> FindAppointmentsToDelete();
    Task<List<Appointment>> FindAppointments(int pageIndex);
    Task DeleteAppointments(List<Appointment> appointments);
}