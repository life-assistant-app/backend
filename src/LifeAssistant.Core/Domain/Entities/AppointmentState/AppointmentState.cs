namespace LifeAssistant.Core.Domain.Entities.AppointmentState;

public abstract class AppointmentState : IAppointmentState
{
    protected Appointment appointment;

    protected AppointmentState(Appointment appointment)
    {
        this.appointment = appointment;
    }

    public abstract string Name { get; }
    public abstract IAppointmentState Accept();
    public abstract IAppointmentState PickUp();
}