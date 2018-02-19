extern alias signed;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ECommerce.Data.NoSql;
using signed::StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace ECommerce.Data.RedisProvider
{
    public class RedisConnection<T>:IDisposable where T:class , new()
    {
        private readonly ConnectionMultiplexer _connection;
        private readonly StackExchangeRedisCacheClient _cacheClient;
        private bool _disposed;

        public RedisConnection(NoSqlConnectionString connectionString)
        {
            _connection = ConnectionMultiplexer.Connect(connectionString.Url);

            var serializer = new NewtonsoftSerializer();
            _cacheClient = new StackExchangeRedisCacheClient(_connection, serializer);
        }
        
        public void CloseConnection(bool allowCommandsToComplete = true)
        {
            _connection.Close(allowCommandsToComplete);
        }

        public async Task<T> AddItemAsync(T value)
        {
            var key = FindKey();
            var id = key.GetValue(value);

            if (string.IsNullOrEmpty(id?.ToString()))
                id = $"{typeof(T).Name}_{Guid.NewGuid()}";

            key.SetValue(value, id);

            var isSuccess = await _cacheClient.AddAsync(id.ToString(), value);

            return (isSuccess) ? value : throw new InvalidOperationException();
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

        public async Task<IEnumerable<T>> WhereAsync(Func<T, bool> func)
        {
            return (await _cacheClient.GetAllAsync<T>(_cacheClient.SearchKeys($"{typeof(T).Name}*"))).Values
                .Where(func).ToList();
        }

        public async Task ReplaceItemAsync(T value)
        {
            var key = FindKey();
            var id = key.GetValue(value);

            if(string.IsNullOrEmpty(id.ToString())) throw new ConstraintException(nameof(id));

            await _cacheClient.ReplaceAsync(id.ToString(), value);
        }

        public async Task RemoveItemAsync(T value)
        {
            var key = FindKey();
            var id = key.GetValue(value);

            if (string.IsNullOrEmpty(id.ToString())) throw new ConstraintException(nameof(id));

            await _cacheClient.RemoveAsync(id.ToString());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if(_disposed) return;

            if (!disposing) return;

            _connection.Dispose();
            _disposed = true;
        }

    }
}
