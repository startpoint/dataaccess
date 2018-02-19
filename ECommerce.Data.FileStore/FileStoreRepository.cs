using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ecommerce.Data.RepositoryStore;
using ECommerce.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ECommerce.Data.FileStore
{
    public class FileStoreRepository<T> : IRepositoryStore<T> where T : class, new()
    {
        ConcurrentDictionary<string, T> _items = new ConcurrentDictionary<string, T>();
        readonly BackgroundWorkScheduler _backgroundWorkScheduler;
        private string _filePath;

        public FileStoreRepository()
        {
            var logger = new LoggerFactory().CreateLogger<BackgroundWorkScheduler>();
            _backgroundWorkScheduler = new BackgroundWorkScheduler(logger, new BackgroundWorkSchedulerOptions{Timeout = TimeSpan.FromMinutes(5)});
        }

        public string Connect(ConnectionOptions connectionOptions)
        {
            var fileInfo = new FileInfo(connectionOptions.ConnectionString);

            _filePath = fileInfo.FullName;

            if (fileInfo.Directory != null && !fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            if (!fileInfo.Exists) return "OK";

            var json = File.ReadAllText(_filePath);
            _items = JsonConvert.DeserializeObject<ConcurrentDictionary<string, T>>(json);

            return "OK";
        }

        public async Task<T> AddAsync(T value)
        {
            var key = FindKey();

            if (key != null)
            {
                return await Task.Run(() =>
                {
                    var id = key.GetValue(value)?.ToString() ?? Guid.NewGuid().ToString();
                    key.SetValue(value, Convert.ChangeType(id, key.PropertyType));
                    _items.TryAdd(id, value);
                    PersistFile();

                    return value;
                });
            }

            throw new NotSupportedException();
        }

        private void PersistFile()
        {
            _backgroundWorkScheduler.QueueWork((cancellationToken) =>
            {
                if (!cancellationToken.IsCancellationRequested)
                    File.WriteAllText(_filePath, JsonConvert.SerializeObject(_items));
            });

            _backgroundWorkScheduler.Start();
        }

        public async Task<T> RemoveAsync(T value)
        {
            return await RemoveAsync(value, true);
        }

        private async Task<T> RemoveAsync(T value, bool persisted)
        {
            var key = FindKey();

            if (key != null)
            {
                return await Task.Run(() =>
                {
                    var id = key.GetValue(value)?.ToString();
                    _items.TryRemove(id, out T item);

                    if (persisted)
                        PersistFile();

                    return item;
                });
            }

            throw new NotSupportedException();
        }

        public async Task<T> SearchASingleItemAsync(Func<T, bool> filter)
        {
            return await  Task.Run(()=> _items.Values.FirstOrDefault(filter));
        }

        public async Task<System.Collections.Generic.IEnumerable<T>> SearchAsync(Func<T, bool> filter)
        {
            return await Task.Run(() => _items.Values.Where(filter).ToList());
        }

        public async Task<T> UpdateAsync(T value)
        {
            await RemoveAsync(value, false);
            return await AddAsync(value);
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

        protected void Dispose(bool disposing)
        {
            _backgroundWorkScheduler.Dispose();
            // Cleanup
        }
    }
}
