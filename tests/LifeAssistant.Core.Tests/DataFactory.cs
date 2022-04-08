using System;
using System.Collections.Generic;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Domain.Entities.ApplicationUser;

namespace LifeAssistant.Web.Tests;

public class DataFactory
{
    public string UserPassword => "ized5è54é-@!é";
    private static int userCount = 0;

    public ApplicationUser CreateAgencyEmployee()
    {
        ApplicationUser user = new ApplicationUser(
            $"shepard.n7-{userCount++}",
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
            $"shepard.n7-{userCount++}",
            BCrypt.Net.BCrypt.HashPassword("ized5è54é-@!é"),
            "John",
            "Shepard",
            ApplicationUserRole.LifeAssistant
        );
        return user;
    }
    
    public ApplicationUser CreateLifeAssistantWithAppointments()
    {
        ApplicationUser user = CreateLifeAssistant();
        DateTime dateTime1 = DateTime.Now.AddDays(1);
        DateTime dateTime2 = dateTime1.AddDays(1);
        DateTime dateTime3 = dateTime2.AddDays(1);
        user.Appointments = new List<Appointment>
        {
            new(dateTime1),
            new(dateTime2),
            new(dateTime3),
        };

        return user;
    }
}