using System.Threading.Tasks;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Persistence;

namespace LifeAssistant.Web.Database.Respositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    private readonly ApplicationDbContext context;

    public Repository(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task Insert(TEntity entity)
    {
        await this.context.Set<TEntity>().AddAsync(entity);
    }

    public async Task Save()
    {
        await this.context.SaveChangesAsync();
    }
}