using System.Collections.Generic;
using System.Threading.Tasks;

namespace LifeAssistant.Core.Tests.Application.FakePersistence;

public class FakeRepository<TEntity,TKey>
{
    public ISet<TEntity> Data { get; }

    public FakeRepository()
    {
        Data = new HashSet<TEntity>();
    }

    public Task Insert(TEntity entity)
    {
        Data.Add(entity);
        return Task.CompletedTask;
    }
}