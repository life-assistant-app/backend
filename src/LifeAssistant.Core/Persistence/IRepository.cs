using System.Threading.Tasks;
using LifeAssistant.Core.Domain.Entities;

namespace LifeAssistant.Core.Persistence;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task Insert(TEntity entity);
    Task Save();
    Task<TEntity> FindById(Guid entityId);
}