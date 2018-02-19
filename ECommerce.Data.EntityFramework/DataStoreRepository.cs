using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Data.RepositoryStore;
using ECommerce.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ECommerce.Data.EntityFramework
{
    public class DataStoreRepository<T> : IRepositoryStore<T> where T : class, new()
    {
        private DataStoreDbContext<T> _dbContext;
        private bool _disposed;

        public string Connect(ConnectionOptions connectionOptions)
        {
            var sqlConnectionOptions =
                JsonConvert.DeserializeObject<SqlConnectionOptions>(connectionOptions.ConnectionString);

            var initializer = PluginContainer.GetInstance<IDbContextOptionsInitializer>(connectionOptions.Provider);
            var dbContextOptions = initializer.Initialize<T>(sqlConnectionOptions.ConnectionString);

            dbContextOptions.ConfigureWarnings(c =>
            {
                if (sqlConnectionOptions.LogWarningEnabled)
                    c.Log();
                else
                    c.Ignore();
            });

            dbContextOptions.EnableSensitiveDataLogging(sqlConnectionOptions.SensitiveDataLoggingEnabled);

            dbContextOptions.UseQueryTrackingBehavior(sqlConnectionOptions.QueryTrackingEnabled
                ? QueryTrackingBehavior.TrackAll
                : QueryTrackingBehavior.NoTracking);

            _dbContext = new DataStoreDbContext<T>(dbContextOptions.Options);

            return "OK";
        }

        public async Task<T> AddAsync(T value)
        {
            var item = await _dbContext.DataSet.AddAsync(value);
            await _dbContext.SaveChangesAsync();

            return item.Entity;
        }

        public async Task<T> UpdateAsync(T value)
        {
            _dbContext.DataSet.Update(value);
            await _dbContext.SaveChangesAsync();
            return value;
        }

        public async Task<T> RemoveAsync(T value)
        {
            _dbContext.DataSet.Remove(value);
            await _dbContext.SaveChangesAsync();
            return value;
        }

        public async Task<IEnumerable<T>> SearchAsync(Func<T, bool> filter)
        {
            return await Task.Run(() => _dbContext.DataSet.Where(filter));
        }

        public async Task<T> SearchASingleItemAsync(Func<T, bool> filter)
        {
            return await Task.Run(() => _dbContext.DataSet.FirstOrDefault(filter));
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool isDisposing)
        {
            if(_disposed) return;

            if (isDisposing)
                _dbContext.Dispose();

            _disposed = true;
        }
    }
}
