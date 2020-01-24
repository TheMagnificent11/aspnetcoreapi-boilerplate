using System;
using System.Threading.Tasks;
using EntityManagement;
using Microsoft.EntityFrameworkCore;
using SampleApiWebApp.Data.Configuration;
using SampleApiWebApp.Domain;

namespace SampleApiWebApp.Data
{
    public sealed class DatabaseContext : DbContext, IDatabaseContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public int AttachedRepositories { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<T> EntitySet<T>()
            where T : class
        {
            return this.Set<T>();
        }

        public Task<int> SaveChangesAsync()
        {
            return this.SaveChangesAsync(true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new TeamConfiguration());
            modelBuilder.ApplyConfiguration(new PlayerConfiguration());
        }
    }
}
