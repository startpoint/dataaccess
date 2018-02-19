using Microsoft.EntityFrameworkCore;

namespace ECommerce.Data.EntityFramework.SqlLite
{
    public class SqlLiteDbContextProvider: IDbContextOptionsInitializer
    {
        public DbContextOptionsBuilder<DataStoreDbContext<T>> Initialize<T>(string connectionString) where T : class, new()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<DataStoreDbContext<T>>()
                .UseSqlite(connectionString);

            using (var dbContext = new DataStoreDbContext<T>(dbContextOptionsBuilder.Options))
            {
                dbContext.Database.EnsureCreated();
            }

            return dbContextOptionsBuilder;
        }
    }
}