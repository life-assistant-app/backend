using System.Threading.Tasks;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Persistence;

namespace LifeAssistant.Web.Database.Respositories;

public class ApplicationUserRepository: IApplicationUserRepository
{
    public Task Insert(ApplicationUser applicationUser)
    {
        throw new System.NotImplementedException();
    }

    public Task<ApplicationUser> FindByUsername(string username)
    {
        throw new System.NotImplementedException();
    }
}