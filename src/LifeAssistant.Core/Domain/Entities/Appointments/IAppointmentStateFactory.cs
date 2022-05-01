namespace LifeAssistant.Core.Domain.Entities.Appointments;

public interface IAppointmentStateFactory
{
    IAppointmentState BuildStateFromAppointment(Appointment appointment, string stateName);
}