using System.Linq;

namespace TaskManagement.Application.Interfaces
{
    public interface IRepository
    {
        public IUnitOfWork CreateUnitOfWork();
        public IQueryable<T> GetEntities<T>() where T : class;
    }
}