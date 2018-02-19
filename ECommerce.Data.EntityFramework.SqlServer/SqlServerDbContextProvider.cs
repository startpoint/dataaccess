using Microsoft.EntityFrameworkCore;

namespace ECommerce.Data.EntityFramework.SqlServer
{
    public class SqlServerDbContextProvider: IDbContextOptionsInitializer
    {
        public DbContextOptionsBuilder<DataStoreDbContext<T>> Initialize<T>(string connectionString) where T : class, new()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<DataStoreDbContext<T>>()
                .UseSqlServer(connectionString);

            using (var dbContext = new DataStoreDbContext<T>(dbContextOptionsBuilder.Options))
            {
                dbContext.Database.EnsureCreated();
            }

            return dbContextOptionsBuilder;
        }
    }
}