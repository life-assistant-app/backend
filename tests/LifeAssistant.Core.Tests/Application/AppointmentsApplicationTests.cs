using System.Threading.Tasks;
using LifeAssistant.Core.Application.Appointments;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Domain.Rules;
using LifeAssistant.Core.Tests.Application.FakePersistence;
using LifeAssistant.Web.Tests;
using Xunit;

namespace LifeAssistant.Core.Tests.Application;

public class AppointmentsApplicationTests
{
    private readonly DataFactory dataFactory = new DataFactory();
    
    [Fact]
    public Task CreateAppointment_ByAgencyEmployeeWithLifeAssistant_InsertsPlannedAppointForLifeAssistant()
    {
        // Given
        ApplicationUser agencyEmployee = dataFactory.CreateAgencyEmployee();
        ApplicationUser lifeAssistant = dataFactory.CreateLifeAssistant();

        var fakeUserRepository = new FakeApplicationUserRepository();
        fakeUserRepository.Data.Add(agencyEmployee);
        fakeUserRepository.Data.Add(lifeAssistant);
        
        var accessControlManager = new AccessControlManager(agencyEmployee.Id, fakeUserRepository);
        var appointmentApplication = new AppointmentsApplication(fakeUserRepository, accessControlManager);
        
        // When
        var appointment = 
        
    }
}