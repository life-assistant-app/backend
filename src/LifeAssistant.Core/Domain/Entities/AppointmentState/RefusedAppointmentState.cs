using LifeAssistant.Core.Domain.Entities.Appointments;

namespace LifeAssistant.Core.Domain.Entities.AppointmentState;

public class RefusedAppointmentState : AppointmentState
{
    public override string Name => "Refused";
    public override bool AcceptState(IAppointmentState state) => false;
}