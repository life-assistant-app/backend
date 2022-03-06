using System.Threading.Tasks;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Persistence;

namespace LifeAssistant.Web.Database.Respositories;

public class Repository<TEntity,TKey>: IRepository<TEntity,TKey>
{
    private readonly ApplicationDbContext context;

    public Repository(ApplicationDbContext context)
    {
        this.context = context;
    }

    public Task Insert(ApplicationUser applicationUser)
    {
        throw new System.NotImplementedException();
    }
}