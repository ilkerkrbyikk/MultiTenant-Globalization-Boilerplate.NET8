using BetopanWeb.Models.Domain;
using BetopanWeb.Services;
using Microsoft.EntityFrameworkCore;

namespace BetopanWeb.Data.Context
{
    public class BetopanDbContext : DbContext
    {
        
        public BetopanDbContext(DbContextOptions<BetopanDbContext> options)
       : base(options)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BetopanDbContext).Assembly);

            // Global Query Filter for future multi-tenant entities
            // modelBuilder.Entity<Article>().HasQueryFilter(x => x.TenantId == _tenantService.GetCurrentTenantId());
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<LocalizationResource> LocalizationResources { get; set; }
    }
}

