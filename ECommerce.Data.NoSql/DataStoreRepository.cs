using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ecommerce.Data.RepositoryStore;
using ECommerce.Core;
using Newtonsoft.Json;

namespace ECommerce.Data.NoSql
{
    /// <inheritdoc />
    public class DataStoreRepository<T> : IRepositoryStore<T> where T : class, new()
    {
        private INoSqlDbConnection<T> _dbContext;
        private bool _disposed;

        public string Connect(ConnectionOptions connectionOptions)
        {
            var noSqlConnectionString =
                JsonConvert.DeserializeObject<NoSqlConnectionString>(connectionOptions.ConnectionString);

            _dbContext = PluginContainer.GetInstance<INoSqlDbConnection<T>>(connectionOptions.Provider, noSqlConnectionString);

            return "OK";
        }

        public async Task<T> AddAsync(T value)
        {
            await _dbContext.AddAsync(value);
            return value;
        }

        public async Task<T> UpdateAsync(T value)
        {
            await _dbContext.UpdateAsync(value);
            return value;
        }

        public async Task<T> RemoveAsync(T value)
        {
            var key = FindKey();
            var id = key.GetValue(value).ToString();
            await _dbContext.RemoveAsync(id);
            return value;
        }

        public async Task<IEnumerable<T>> SearchAsync(Func<T, bool> filter)
        {
            return await _dbContext.Where(filter);
        }

        public async Task<T> SearchASingleItemAsync(Func<T, bool> filter)
        {
            return (await SearchAsync(filter)).FirstOrDefault();
        }

        private static PropertyInfo FindKey()
        {
            var et = typeof(T);
            var at = typeof(KeyAttribute);

            var props = from p in et.GetProperties()
                let attr = Attribute.GetCustomAttribute(p, at) as KeyAttribute
                select new { Property = p, IsKey = attr != null };

            return props.Where(p => p.IsKey).Select(p => p.Property).FirstOrDefault();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool isDisposing)
        {
            if (_disposed)
                return;

            if (isDisposing)
                _dbContext.Dispose();

            _disposed = true;
        }
    }
}
