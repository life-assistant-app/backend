using LifeAssistant.Core.Domain.Entities;

namespace LifeAssistant.Web.Tests;

public class DataFactory
{
    public string UserPassword => "ized5è54é-@!é";

    public ApplicationUser CreateAgencyEmployee()
    {
        ApplicationUser user = new ApplicationUser(
            "shepard.n7",
            BCrypt.Net.BCrypt.HashPassword("ized5è54é-@!é"),
            "John",
            "Shepard",
            ApplicationUserRole.AgencyEmployee
        );
        return user;
    }

    public ApplicationUser CreateLifeAssistant()
    {
        ApplicationUser user = new ApplicationUser(
            "shepard.n7",
            BCrypt.Net.BCrypt.HashPassword("ized5è54é-@!é"),
            "John",
            "Shepard",
            ApplicationUserRole.LifeAssistant
        );
        return user;
    }
}