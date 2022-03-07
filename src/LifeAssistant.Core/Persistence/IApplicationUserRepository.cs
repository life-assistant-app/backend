using System.Threading.Tasks;
using LifeAssistant.Core.Domain.Entities;

namespace LifeAssistant.Core.Persistence;

public interface IApplicationUserRepository : IRepository<ApplicationUser>
{
    Task<ApplicationUser> FindByUsername(string username);
}