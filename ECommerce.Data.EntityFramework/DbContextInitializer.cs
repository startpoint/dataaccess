using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Data.EntityFramework
{
    public class FirebirdDbContextInitializer:IDbContextOptionsInitializer
    {
        public DbContextOptionsBuilder<DataStoreDbContext<T>> Initialize<T>(string connectionString) where T:class, new()
        {
            return  new DbContextOptionsBuilder<DataStoreDbContext<T>>()
                .UseFirebird(connectionString);
        }
    }

    public class SqlLiteDbContextInitializer : IDbContextOptionsInitializer
    {
        public DbContextOptionsBuilder<DataStoreDbContext<T>> Initialize<T>(string connectionString) where T : class, new()
        {
            return new DbContextOptionsBuilder<DataStoreDbContext<T>>()
                .UseSqlite(connectionString);
        }
    }
    public class SqlServerDbContextInitializer : IDbContextOptionsInitializer
    {
        public DbContextOptionsBuilder<DataStoreDbContext<T>> Initialize<T>(string connectionString) where T : class, new()
        {
            return new DbContextOptionsBuilder<DataStoreDbContext<T>>()
                .UseSqlServer(connectionString);
        }
    }
    public class MySqlDbContextInitializer : IDbContextOptionsInitializer
    {
        public DbContextOptionsBuilder<DataStoreDbContext<T>> Initialize<T>(string connectionString) where T : class, new()
        {
            return new DbContextOptionsBuilder<DataStoreDbContext<T>>()
                .UseMySQL(connectionString);
        }
    }
    public class InMemoryDbContextInitializer : IDbContextOptionsInitializer
    {
        public DbContextOptionsBuilder<DataStoreDbContext<T>> Initialize<T>(string connectionString) where T : class, new()
        {
            return new DbContextOptionsBuilder<DataStoreDbContext<T>>()
                .UseInMemoryDatabase(connectionString);
        }
    }
    public class ProgressDbDbContextInitializer : IDbContextOptionsInitializer
    {
        public DbContextOptionsBuilder<DataStoreDbContext<T>> Initialize<T>(string connectionString) where T : class, new()
        {
            return new DbContextOptionsBuilder<DataStoreDbContext<T>>()
                .UseNpgsql(connectionString);
        }
    }
}
