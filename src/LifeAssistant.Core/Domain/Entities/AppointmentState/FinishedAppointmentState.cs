using LifeAssistant.Core.Domain.Entities.Appointments;

namespace LifeAssistant.Core.Domain.Entities.AppointmentState;

public class FinishedAppointmentState : AppointmentState
{
    public override string Name => "Finished";

    public override bool AcceptState(IAppointmentState state)
    {
        return false;
    }
}