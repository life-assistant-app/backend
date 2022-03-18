using System.Linq;
using System.Threading.Tasks;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Persistence;

namespace LifeAssistant.Core.Tests.Application.FakePersistence;

public class FakeApplicationUserRepository : FakeRepository<ApplicationUser>, IApplicationUserRepository
{
    public Task<ApplicationUser> FindByUsername(string username)
    {
        return Task.FromResult(this.Data.First(user => user.UserName == username));
    }
}