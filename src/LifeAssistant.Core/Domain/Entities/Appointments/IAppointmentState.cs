namespace LifeAssistant.Core.Domain.Entities.Appointments;

public interface IAppointmentState
{
    Appointment Appointment { get; set; }
    string Name { get; }

    bool AcceptState(IAppointmentState state);
}