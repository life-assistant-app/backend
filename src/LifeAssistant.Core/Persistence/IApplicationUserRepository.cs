using LifeAssistant.Core.Domain.Entities;

namespace LifeAssistant.Core.Persistence;

public interface IApplicationUserRepository
{
    Task Save();
    Task Insert(IApplicationUserWithAppointments entity);
    Task Update(IApplicationUserWithAppointments entity);
    Task<IApplicationUser> FindById(Guid entityId);
    Task<IApplicationUser> FindByUsername(string username);

    Task<IApplicationUserWithAppointments> FindByIdWithAppointments(Guid entityId);
    Task<IList<IApplicationUser>> FindValidatedByRole(ApplicationUserRole role);
}