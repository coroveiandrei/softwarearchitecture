using InventoryService.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService.Application.Repositories.Base
{
    public interface IRepository<T> where T : class, IEntity
    {
        IQueryable<T> Query(Expression<Func<T, bool>> whereFilter = null) ;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        [Obsolete("We use Async version of SaveChanges")]
        int SaveChanges();

        void Add(T entity, bool saveImmediately = false);
        Task AddAsync(T entity, bool saveImmediately = false, CancellationToken cancellationToken = default);

        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
