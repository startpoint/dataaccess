using Microsoft.EntityFrameworkCore;

namespace ECommerce.Data.EntityFramework.InMemory
{
    public class InMemoryDbContextProvider: IDbContextOptionsInitializer
    {
        public DbContextOptionsBuilder<DataStoreDbContext<T>> Initialize<T>(string connectionString) where T : class, new()
        {
            return new DbContextOptionsBuilder<DataStoreDbContext<T>>()
                .UseInMemoryDatabase(connectionString);
        }
    }
}