using LifeAssistant.Core.Domain.Entities;

namespace LifeAssistant.Web.Tests;

public class DataFactory
{
    public ApplicationUser CreateApplicationUser()
    {
        ApplicationUser user = new ApplicationUser(
            "John Doe",
            "ized5è54é-@!é",
            ApplicationUserRole.AgencyEmployee
        );
        return user;
    }
}