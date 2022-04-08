using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Domain.Exceptions;
using LifeAssistant.Core.Persistence;

namespace LifeAssistant.Core.Domain.Rules;

public class AccessControlManager
{
    private readonly Guid currentUserId;
    private readonly IApplicationUserRepository userRepository;

    private IApplicationUser? currentUser;


    public AccessControlManager(Guid currentUserId, IApplicationUserRepository userRepository)
    {
        this.currentUserId = currentUserId;
        this.userRepository = userRepository;
    }

    public async Task EnsureUserCanCreateAppointment()
    {
        IApplicationUser user = await GetCurrentUser();
        if (user.Role is not ApplicationUserRole.AgencyEmployee)
        {
            throw new IllegalAccessException("Only Agency Employees can create Appointments");
        }
    }

    public async Task<IApplicationUser> GetCurrentUser()
    {
        return this.currentUser ??= await userRepository.FindById(this.currentUserId);
    }

    public async Task EnsureUserCanUpdateAppointmentState(Guid lifeAssistantId)
    {
        IApplicationUser user = await GetCurrentUser();
        if (user.Role is not ApplicationUserRole.AgencyEmployee && user.Id != lifeAssistantId)
        {
            throw new IllegalAccessException("An appointment can only be modified by the owning life assistant or an agency employee");
        }
    }
}