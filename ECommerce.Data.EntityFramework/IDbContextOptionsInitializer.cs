using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Data.EntityFramework
{
    public interface IDbContextOptionsInitializer
    {
        DbContextOptionsBuilder<DataStoreDbContext<T>> Initialize<T>(string connectionString) where T : class, new();
    }
}
