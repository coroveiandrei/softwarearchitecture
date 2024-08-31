using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskManagement.Application.Interfaces
{
    public interface IDbContext
    {
        DbSet<T> Set<T>() where T : class;
        Task SaveChangesAsync(CancellationToken cancellationToken);
        void SaveChanges();
    }
}
