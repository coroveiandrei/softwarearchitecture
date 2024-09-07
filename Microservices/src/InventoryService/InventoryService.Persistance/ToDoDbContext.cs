using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using InventoryService.Common;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Projections;
using InventoryService.Persistance.DataModel;
using Quotation.Domain.Entities.Authentication;

namespace InventoryService.Persistance
{
    [MapServiceDependency(nameof(ToDoDbContext))]
    public class ToDoDbContext : IdentityDbContext<InventoryService.Domain.Entities.User>, IToDoDbContext
    {
        public DbSet<StockProjection> Stocks { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }

        public ToDoDbContext(DbContextOptions<ToDoDbContext> options)
           : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ToDoDbContext).Assembly);
        }
    }
}
