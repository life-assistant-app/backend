namespace LifeAssistant.Core.Domain.Entities.AppointmentState;

public class FinishedAppointmentState : AppointmentState
{
    public FinishedAppointmentState(Appointment appointment) : base(appointment)
    {
    }

    public override string Name => "Finished";
    public override IAppointmentState Accept()
    {
        throw new InvalidOperationException();
    }

    public override IAppointmentState PickUp()
    {
        throw new InvalidOperationException();
    }
}