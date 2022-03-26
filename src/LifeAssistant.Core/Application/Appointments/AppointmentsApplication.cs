using LifeAssistant.Core.Application.Appointments.Contracts;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Domain.Rules;
using LifeAssistant.Core.Persistence;

namespace LifeAssistant.Core.Application.Appointments;

public class AppointmentsApplication
{
    private readonly IApplicationUserRepository applicationUserRepository;
    private readonly AccessControlManager accessControlManager;


    public AppointmentsApplication(IApplicationUserRepository applicationUserRepository,
        AccessControlManager accessControlManager)
    {
        this.applicationUserRepository = applicationUserRepository;
        this.accessControlManager = accessControlManager;
    }

    public async Task<GetAppointmentResponse> CreateAppointment(Guid lifeAssistantId, DateTime dateTime)
    {
        IApplicationUserWithAppointments lifeAssistant =
            await applicationUserRepository.FindByIdWithAppointments(lifeAssistantId);
        var appointment = new Appointment(dateTime);
        lifeAssistant.Appointments.Add(appointment);

        await this.applicationUserRepository.Update(lifeAssistant);
        await this.applicationUserRepository.Save();

        return new GetAppointmentResponse(appointment.Id,
            appointment.State.Name, 
            appointment.DateTime
        );
    }
}