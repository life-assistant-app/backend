namespace LifeAssistant.Core.Domain.Entities.AppointmentState;

public class PlannedAppointmentState : AppointmentState
{
    public override string Name => "Planned";
    
    public PlannedAppointmentState(Appointment appointment) : base(appointment)
    {
    }
    
    public override IAppointmentState Accept()
    {
        return new AcceptedAppointmentState(this.appointment);
    }

    public override IAppointmentState PickUp()
    {
        throw new InvalidOperationException();
    }


}