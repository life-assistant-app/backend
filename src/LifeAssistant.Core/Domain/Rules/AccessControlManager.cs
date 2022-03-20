﻿using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Persistence;

namespace LifeAssistant.Core.Domain.Rules;

public class AccessControlManager
{
    private readonly Guid currentUserId;
    private readonly IApplicationUserRepository userRepository;

    private ApplicationUser? currentUser;


    public AccessControlManager(Guid currentUserId, IApplicationUserRepository userRepository)
    {
        this.currentUserId = currentUserId;
        this.userRepository = userRepository;
    }

    public async Task EnsureUserCanCreateAppointment()
    {
        ApplicationUser user = await GetCurrentUser();
        if (user.Role is not ApplicationUserRole.AgencyEmployee)
        {
            throw new InvalidOperationException("Only Agency Employees can create Appointments");
        }
    }

    public async Task<ApplicationUser> GetCurrentUser()
    {
        return this.currentUser ??= await userRepository.FindById(this.currentUserId);
    }
}