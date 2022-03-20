using System.Threading.Tasks;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Persistence;
using LifeAssistant.Web.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeAssistant.Web.Database.Respositories;

public class ApplicationUserRepository : Repository<ApplicationUserEntity>,IApplicationUserRepository
{
    private readonly ApplicationDbContext context;

    public ApplicationUserRepository(ApplicationDbContext context) : base(context)
    {
        this.context = context;
    }
    
    public async Task<ApplicationUser> FindByUsername(string username)
    {
        ApplicationUserEntity entity = await this.context
            .Users
            .FirstAsync(user => user.UserName == username);

        return entity.ToDomainEntity();
    }

    public async Task Insert(ApplicationUser applicationUser)
    {
        await this.Insert(new ApplicationUserEntity(applicationUser));
    }
}