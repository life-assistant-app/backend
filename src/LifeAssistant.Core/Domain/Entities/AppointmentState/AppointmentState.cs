using LifeAssistant.Core.Domain.Entities.Appointments;

namespace LifeAssistant.Core.Domain.Entities.AppointmentState;

public abstract class AppointmentState : IAppointmentState
{
    public Appointment Appointment { get; set; }

    public abstract string Name { get; }
    public abstract bool AcceptState(IAppointmentState state);
}