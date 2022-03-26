using LifeAssistant.Web.Database.Entities;

namespace LifeAssistant.Web.Database.Respositories;

public class Repository<TDbEntity> where TDbEntity : BaseDbEntity
{
    private readonly ApplicationDbContext context;

    public Repository(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task Insert(TDbEntity entity)
    {
        await this.context.Set<TDbEntity>().AddAsync(entity);
    }

    public async Task Save()
    {
        await this.context.SaveChangesAsync();
    }
}