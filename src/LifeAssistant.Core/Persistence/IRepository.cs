using System.Threading.Tasks;
using LifeAssistant.Core.Domain.Entities;

namespace LifeAssistant.Core.Persistence;

public interface IRepository<TEntity>
{
    Task Insert(TEntity applicationUser);
    Task Save();
}