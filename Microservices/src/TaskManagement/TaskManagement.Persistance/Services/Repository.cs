using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Common;
using TaskManagement.Persistance.SqlExceptionHandlers;
using System.Linq;

namespace TaskManagement.Persistance
{
    [MapServiceDependency(nameof(Repository))]
    internal class Repository : IRepository
    {
        
        private readonly IDbContext context;   
        private ISqlExceptionHandler exceptionHandler;
        public Repository(IDbContext context, ISqlExceptionHandler exceptionHandler)
        {
            this.context = context;
            this.exceptionHandler = exceptionHandler;
        }


        public IUnitOfWork CreateUnitOfWork()
        {
            return new UnitOfWork(context, exceptionHandler);
        }

        public IQueryable<T> GetEntities<T>() where T : class
        {
            return context.Set<T>().AsNoTracking();
        }
    }
}