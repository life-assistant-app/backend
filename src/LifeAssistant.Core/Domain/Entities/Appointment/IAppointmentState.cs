namespace LifeAssistant.Core.Domain.Entities;

public interface IAppointmentState
{
    Appointment Appointment { get; set; }
    string Name { get; }

    bool AcceptState(IAppointmentState state);
}