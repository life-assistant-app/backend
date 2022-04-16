namespace LifeAssistant.Core.Domain.Entities.AppointmentState;

public class RefusedAppointmentState : AppointmentState
{
    public override string Name => "Refused";
    public override bool AcceptState(IAppointmentState state) => false;
}