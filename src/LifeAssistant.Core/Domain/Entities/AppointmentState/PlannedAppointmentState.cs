using LifeAssistant.Core.Domain.Entities.Appointments;

namespace LifeAssistant.Core.Domain.Entities.AppointmentState;

public class PlannedAppointmentState : AppointmentState
{
    public override string Name => "Planned";

    public override bool AcceptState(IAppointmentState state)
    {
        return state is (PendingAppointmentState or RefusedAppointmentState);
    }
}