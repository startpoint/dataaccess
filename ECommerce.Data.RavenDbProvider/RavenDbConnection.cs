using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using ECommerce.Data.NoSql;
using Raven.Client.Documents;
using Raven.Client.ServerWide.Operations;

namespace ECommerce.Data.RavenDbProvider
{
    public class RavenDbConnection<T> : INoSqlDbConnection<T> where T : class
    {
        private readonly IDocumentStore _store;
        private NoSqlConnectionString _dbContextOptions;

        public RavenDbConnection(NoSqlConnectionString dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;

            if (_dbContextOptions.IsTest)
            {
                _store = new RavenDbTestDriver().GetDocumentStore().Initialize();
            }
            else
            {
                _store = new DocumentStore
                {
                    Urls = new[] {dbContextOptions.Url},
                    Database = dbContextOptions.Database,
                    Certificate = !string.IsNullOrEmpty(dbContextOptions.Certificate)
                        ? LoadCertificate(dbContextOptions.Certificate)
                        : null
                }.Initialize();
            }
        }

        private X509Certificate2 LoadCertificate(string certificate)
        {
            var computerCaStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine);

            try
            {
                computerCaStore.Open(OpenFlags.ReadOnly);

                var certificatesInStore = computerCaStore.Certificates;

                foreach (var cert in certificatesInStore)
                {
                    if (cert.Thumbprint == certificate)
                        return cert;
                }
            }
            finally
            {
                computerCaStore.Close();
            }

            return null;
        }

        public async Task AddAsync(T value)
        {
            await Task.Run(() =>
            {
                using (var session = _store.OpenSession())
                {
                    session.Store(value);
                    session.SaveChanges();
                }
            });
        }

        public async Task<IList<T>> Where(Func<T, bool> func)
        {
            return await Task.Run(() =>
            {
                using (var session = _store.OpenSession())
                {
                    var result = Enumerable.Where(session.Query<T>(), func).ToList();
                    return result;
                }
            });
        }

        public async Task UpdateAsync(T value)
        {
            await Task.Run(() =>
            {
                using (var session = _store.OpenSession())
                {
                    session.Store(value);
                    session.SaveChanges();
                }
            });
        }

        public async Task RemoveAsync(string id)
        {
            await Task.Run(() =>
            {
                using (var session = _store.OpenSession())
                {
                    session.Delete(id);
                    session.SaveChanges();
                }
            });
        }

        public void Dispose()
        {
            _store.Dispose();
        }

        public async Task Clear()
        {
            await Task.Run(() =>
            {
                using (var session = _store.OpenSession())
                {
                    session.Advanced.Clear();
                    session.SaveChanges();
                }
            });
        }

        public void DropDatabase()
        {
            _store.Maintenance.Server.Send(new DeleteDatabasesOperation(_dbContextOptions.Database,
                true));
        }
    }
}