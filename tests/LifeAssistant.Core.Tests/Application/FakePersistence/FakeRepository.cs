using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LifeAssistant.Core.Persistence;

namespace LifeAssistant.Core.Tests.Application.FakePersistence;

public class FakeRepository<TEntity> : IRepository<TEntity>
{
    public ISet<TEntity> Data { get; private set; }
    private ISet<TEntity> newData;
    public FakeRepository()
    {
        Data = new HashSet<TEntity>();
        newData = new HashSet<TEntity>();
    }

    public Task Insert(TEntity entity)
    {
        newData.Add(entity);
        return Task.CompletedTask;
    }

    public Task Save()
    {
        this.Data = newData.ToHashSet();
        return Task.CompletedTask;
    }
}