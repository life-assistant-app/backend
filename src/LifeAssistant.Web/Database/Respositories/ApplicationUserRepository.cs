using System.Threading.Tasks;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Persistence;

namespace LifeAssistant.Web.Database.Respositories;

public class ApplicationUserRepository: Repository<ApplicationUser>, IApplicationUserRepository
{
    private readonly ApplicationDbContext context;

    public ApplicationUserRepository(ApplicationDbContext context) : base(context)
    {
        this.context = context;
    }
    
    public Task<ApplicationUser> FindByUsername(string username)
    {
        throw new System.NotImplementedException();
    }
}