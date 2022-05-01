using LifeAssistant.Core.Domain.Entities.Appointments;

namespace LifeAssistant.Core.Domain.Entities.AppointmentState;

public class AppointmentStateFactory : IAppointmentStateFactory
{
    public IAppointmentState BuildStateFromAppointment(Appointment appointment, string stateName)
    {
        return stateName switch
        {
            "Planned" => new PlannedAppointmentState(),
            "Pending Pickup" => new PendingAppointmentState(),
            "Finished" => new FinishedAppointmentState(),
            "Refused" => new RefusedAppointmentState(),
            _ => throw new ArgumentException($"No Appoint State for name {stateName}")
        };
    }
}