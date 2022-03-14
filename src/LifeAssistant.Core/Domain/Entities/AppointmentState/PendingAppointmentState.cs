namespace LifeAssistant.Core.Domain.Entities.AppointmentState;

public class AcceptedAppointmentState : AppointmentState
{
    public AcceptedAppointmentState(Appointment appointment) : base(appointment)
    {
    }

    public override string Name => "Pending Pickup";
    public override IAppointmentState Accept()
    {
        throw new InvalidOperationException();
    }

    public override IAppointmentState PickUp()
    {
        return new FinishedAppointmentState(this.appointment);
    }
}