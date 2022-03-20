using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Domain.Rules;
using LifeAssistant.Core.Persistence;

namespace LifeAssistant.Core.Application.Appointments;

public class AppointmentsApplication
{
    private readonly IApplicationUserRepository applicationUserRepository;
    private readonly AccessControlManager accessControlManager;


    public AppointmentsApplication(IApplicationUserRepository applicationUserRepository, AccessControlManager accessControlManager)
    {
        this.applicationUserRepository = applicationUserRepository;
        this.accessControlManager = accessControlManager;
    }

    public Appointment CreateAppointment(ApplicationUser lifeAssistant, DateTime dateTime)
    {
        throw new NotImplementedException();
    }
    
}