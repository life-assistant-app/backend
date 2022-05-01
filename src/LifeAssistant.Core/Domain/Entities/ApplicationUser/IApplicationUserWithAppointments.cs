using LifeAssistant.Core.Domain.Entities.Appointments;

namespace LifeAssistant.Core.Domain.Entities;

public interface IApplicationUserWithAppointments : IApplicationUser
{
    List<Appointment> Appointments { get; set; }
}