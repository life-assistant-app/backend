using System.Threading.Tasks;
using LifeAssistant.Core.Domain.Entities;

namespace LifeAssistant.Core.Persistence;

public interface IRepository<TEntity,TKey>
{
    Task Insert(ApplicationUser applicationUser);
}