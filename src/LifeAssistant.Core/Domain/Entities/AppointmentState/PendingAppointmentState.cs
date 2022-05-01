using LifeAssistant.Core.Domain.Entities.Appointments;

namespace LifeAssistant.Core.Domain.Entities.AppointmentState;

public class PendingAppointmentState : AppointmentState
{
    public override string Name => "Pending Pickup";

    public override bool AcceptState(IAppointmentState state)
    {
        return state is FinishedAppointmentState;
    }
}