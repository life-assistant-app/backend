using LifeAssistant.Core.Domain.Entities;

namespace LifeAssistant.Web.Tests;

public class DataFactory
{
    public ApplicationUser CreateAgencyEmployee()
    {
        ApplicationUser user = new ApplicationUser(
            "John Doe",
            "ized5è54é-@!é",
            ApplicationUserRole.AgencyEmployee
        );
        return user;
    }
    
    public ApplicationUser CreateLifeAssistant()
    {
        ApplicationUser user = new ApplicationUser(
            "John Doe",
            "ized5è54é-@!é",
            ApplicationUserRole.LifeAssistant
        );
        return user;
    }
}