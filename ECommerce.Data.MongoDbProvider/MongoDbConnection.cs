using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ECommerce.Data.NoSql;
using MongoDB.Driver;

namespace ECommerce.Data.MongoDbProvider
{
    public class MongoDbConnection<T> : INoSqlDbConnection<T> where T : class
    {
        private readonly NoSqlConnectionString _documentDbConnectionOptions;
        private readonly MongoClient _client;

        public MongoDbConnection(NoSqlConnectionString dbContextOptions)
        {
            _documentDbConnectionOptions = dbContextOptions;

            var url = $"mongodb://{dbContextOptions.UserId}:{dbContextOptions.Password}@{dbContextOptions.Url}";

            _client = new MongoClient(new MongoUrl(url));

            CreateCollectionIfNotExistsAsync(_client, dbContextOptions.Database, dbContextOptions.Collection);

            //_client.StartSession();
        }

        private static void CreateCollectionIfNotExistsAsync(MongoClient client, string databaseId, string collectionId)
        {
            client.GetDatabase(databaseId).GetCollection<T>(collectionId);
        }

        public async Task<IList<T>> Where(Func<T, bool> func)
        {
            return await Task.Run(() =>
            {
                var mongoCollection = _client.GetDatabase(_documentDbConnectionOptions.Database)
                    .GetCollection<T>(_documentDbConnectionOptions.Collection);

                return Enumerable.Where(mongoCollection.AsQueryable(), func).ToList();
            });
        }

        public async Task AddAsync(T value)
        {
            await _client.GetDatabase(_documentDbConnectionOptions.Database).GetCollection<T>(_documentDbConnectionOptions.Collection).InsertOneAsync(value);
        }

        public async Task UpdateAsync(T value)
        {
            var propertyKey = FindKey();
            var id = propertyKey.GetValue(value);

            var filter = Builders<T>.Filter.Eq("_id", id);

            await _client.GetDatabase(_documentDbConnectionOptions.Database)
                .GetCollection<T>(_documentDbConnectionOptions.Collection).ReplaceOneAsync(filter, value);
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

        public async Task RemoveAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);

            await _client.GetDatabase(_documentDbConnectionOptions.Database)
                .GetCollection<T>(_documentDbConnectionOptions.Collection).DeleteOneAsync(filter);
        }
        public void Dispose()
        {
            //
        }

        public async Task Clear()
        {
            await _client.GetDatabase(_documentDbConnectionOptions.Database)
                .DropCollectionAsync(_documentDbConnectionOptions.Collection);
        }

        public void DropDatabase()
        {
            _client.DropDatabase(_documentDbConnectionOptions.Database);
        }
    }
}