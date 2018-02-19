using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ECommerce.Data.EntityFramework
{
    internal static class ModelBuilderExtensions
    {
        public static void RemovePluralizingTableNameConvention(this ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.Relational().TableName = entity.DisplayName();
            }
        }
    }

    public class DataStoreDbContext<T> : DbContext where T:class, new()
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.RemovePluralizingTableNameConvention();
        }

        public DataStoreDbContext([NotNull] DbContextOptions options) : base(options)
        {
        }

        public DbSet<T> DataSet { get; set; }
    }
}
