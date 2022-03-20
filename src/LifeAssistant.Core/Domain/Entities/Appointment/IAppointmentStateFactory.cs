namespace LifeAssistant.Core.Domain.Entities;

public interface IAppointmentStateFactory
{
    IAppointmentState BuildStateFromAppointment(Appointment appointment, string stateName);
}