using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ECommerce.Data.NoSql;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace ECommerce.Data.DocumentDbProvider
{
    public class DocumentDbConnection<T> : INoSqlDbConnection<T> where T : class
    {
        private readonly NoSqlConnectionString _documentDbConnectionOptions;
        private readonly DocumentClient _client;

        public DocumentDbConnection(NoSqlConnectionString dbContextOptions)
        {
            _documentDbConnectionOptions = dbContextOptions;

            _client = new DocumentClient(new Uri(dbContextOptions.Url), dbContextOptions.AuthorizationKey);

            CreateDatabaseIfNotExistsAsync(_client, dbContextOptions.Database).Wait();
            CreateCollectionIfNotExistsAsync(_client, dbContextOptions.Database, dbContextOptions.Collection).Wait();
        }

        private static async Task CreateDatabaseIfNotExistsAsync(IDocumentClient client, string databaseId)
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = databaseId });
                }
                else
                {
                    throw;
                }
            }
        }
        private static async Task CreateCollectionIfNotExistsAsync(IDocumentClient client, string databaseId, string collectionId)
        {
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(databaseId),
                        new DocumentCollection { Id = collectionId},
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<IList<T>> Where(Func<T, bool> func)
        {
            return await Task.Run(() =>
            {
                var query = _client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(_documentDbConnectionOptions.Database, _documentDbConnectionOptions.Collection),
                    new FeedOptions { MaxItemCount = -1 }).Where(func).ToList();

                return query;
            });
        }

        public async Task AddAsync(T value)
        {
            var collectionUri = UriFactory.CreateDocumentCollectionUri(_documentDbConnectionOptions.Database,
                _documentDbConnectionOptions.Collection);

            await _client.CreateDocumentAsync(collectionUri, value);
        }

        public async Task UpdateAsync(T value)
        {
            var propertyKey = FindKey();
            var id = propertyKey.GetValue(value).ToString();

            var document = Enumerable.FirstOrDefault(_client.CreateDocumentQuery(UriFactory.CreateDocumentCollectionUri(_documentDbConnectionOptions.Database, _documentDbConnectionOptions.Collection), 
                new FeedOptions {MaxItemCount = -1}), doc => doc.GetPropertyValue<string>(propertyKey.Name) == id);

            if (document != null)
            {
                foreach (var property in value.GetType().GetProperties())
                {
                    if(property.CanWrite)
                        document.SetPropertyValue(property.Name, property.GetValue(value));
                }

                await _client.ReplaceDocumentAsync(document);
            }
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
            var propertyKey = FindKey();

            var document = Enumerable.FirstOrDefault(_client.CreateDocumentQuery(UriFactory.CreateDocumentCollectionUri(_documentDbConnectionOptions.Database, _documentDbConnectionOptions.Collection),
                new FeedOptions { MaxItemCount = -1 }), doc => doc.GetPropertyValue<string>(propertyKey.Name) == id);

            if(document != null)
            {
                var uri = UriFactory.CreateDocumentUri(_documentDbConnectionOptions.Database,
                    _documentDbConnectionOptions.Collection, document.Id);

                await _client.DeleteDocumentAsync(uri);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _client.Dispose();
        }


        public async Task Clear()
        {
            var uri = UriFactory.CreateDocumentCollectionUri(_documentDbConnectionOptions.Database,
                _documentDbConnectionOptions.Collection);

            await _client.DeleteDocumentCollectionAsync(uri);
        }

        public void DropDatabase()
        {
            var uri = UriFactory.CreateDatabaseUri(_documentDbConnectionOptions.Database);
            _client.DeleteDatabaseAsync(uri);
        }
    }
}