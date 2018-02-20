using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ECommerce.Core;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Data.RepositoryStore
{
    /// <summary>
    /// Generic Repository store to use with provider injected
    /// </summary>
    /// <typeparam name="T">The type of the model to use</typeparam>
    public class RepositoryStore<T> : IDisposable where T:class, new()
    {
        private readonly IRepositoryStore<T> _instance;
        private readonly ILogger<T> _logger;
        private readonly DiagnosticSource _diagnosticSource;
        private readonly ConnectionOptions _connectionOptions;

        public Action<T> BeforeInsert { get; set; }

        /// <summary>
        /// base constructor
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="connectionOptions"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="diagnosticSource"></param>
        public RepositoryStore(string assembly, ConnectionOptions connectionOptions, ILoggerFactory loggerFactory, DiagnosticSource diagnosticSource )
        {
            _instance = PluginContainer.GetInstance<IRepositoryStore<T>>(assembly);
            _logger = loggerFactory.CreateLogger<T>();
            _diagnosticSource = diagnosticSource;
            _connectionOptions = connectionOptions;
        }

        private string Connect()
        {
            if (_diagnosticSource.IsEnabled($"{typeof(T).Name}_Connect"))
                _diagnosticSource.Write($"{typeof(T).Name}_Connect", 1);

            var resultConnection = string.Empty;
            var maxAttempted = 0;

            while (!resultConnection.Equals("OK") && maxAttempted < _connectionOptions.RetryCount)
            {
                try
                {
                    resultConnection = _instance.Connect(_connectionOptions);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "can not connect to datasource");

                    var continues = _connectionOptions.Handles?.Invoke(ex);

                    if (continues == false) return ex.Message;
                }

                if (_connectionOptions.IntervalRetry > 0)
                    Task.Delay(TimeSpan.FromSeconds(_connectionOptions.IntervalRetry)).Wait();

                maxAttempted++;
            }

            if(!resultConnection.Equals("OK"))
                _connectionOptions.FallbackConnection?.Invoke();

            return resultConnection;
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

        /// <summary>
        /// Add a value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<ExecutionResult<T>> AddAsync(T value)
        {
            var connectMessage = Connect();

            if (connectMessage != "OK")
                throw new ConnectionException(connectMessage);

            BeforeInsert?.Invoke(value);

            var stop = new Stopwatch();
            stop.Start();

            if(_diagnosticSource.IsEnabled($"{typeof(T).Name}_Add"))
                _diagnosticSource.Write($"{typeof(T).Name}_Add", value);

            _logger.LogTrace("Try to add");

            var key = FindKey();
            var id = key.GetValue(value);

            var item = await _instance.SearchASingleItemAsync(x =>
            {
                var val = key.GetValue(x);
                return val.Equals(id);
            });

            if (item == null)
                item = await TryAdd(value);
            else
                throw new DuplicateNameException();

            stop.Stop();

            _logger.LogTrace($"End Add in {stop.Elapsed}");

            return new ExecutionResult<T>(item != null, item);
        }

        private async Task<T> TryAdd(T value)
        {
            var maxAttempted = 0;

            while (maxAttempted < _connectionOptions.RetryCount)
            {
                try
                {
                    return await _instance.AddAsync(value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "can not add item");

                    var continues = _connectionOptions.Handles?.Invoke(ex);

                    if (continues == false) return null;
                }

                if (_connectionOptions.IntervalRetry > 0)
                    await Task.Delay(TimeSpan.FromSeconds(_connectionOptions.IntervalRetry));

                maxAttempted++;
            }

            _connectionOptions.FallBackAction?.Invoke(nameof(TryAdd), value);

            return null;
        }


        /// <summary>
        /// Update the value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<ExecutionResult<T>> UpdateAsync(T value)
        {
            var connectMessage = Connect();

            if (connectMessage != "OK")
                throw new ConnectionException(connectMessage);

            var stop = new Stopwatch();
            stop.Start();

            if (_diagnosticSource.IsEnabled($"{typeof(T).Name}_Update"))
                _diagnosticSource.Write($"{typeof(T).Name}_Update", value);

            _logger.LogTrace("Try to update item");
            T result = await TryUpdate(value);

            stop.Stop();
            _logger.LogTrace($"End Update in {stop.Elapsed}");

            return new ExecutionResult<T>(result != null, result);
        }

        private async Task<T> TryUpdate(T value)
        {
            var maxAttempted = 0;

            while (maxAttempted < _connectionOptions.RetryCount)
            {
                try
                {
                    return await _instance.UpdateAsync(value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "can not update item");

                    var continues = _connectionOptions.Handles?.Invoke(ex);

                    if (continues == false) return null;
                }

                if (_connectionOptions.IntervalRetry > 0)
                    await Task.Delay(TimeSpan.FromSeconds(_connectionOptions.IntervalRetry));

                maxAttempted++;
            }

            _connectionOptions.FallBackAction?.Invoke(nameof(TryUpdate), value);

            return null;            
        }

        /// <summary>
        /// Remove the value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<ExecutionResult<T>> RemoveAsync(T value)
        {
            var connectMessage = Connect();

            if (connectMessage != "OK")
                throw new ConnectionException(connectMessage);

            var stop = new Stopwatch();
            stop.Start();

            if (_diagnosticSource.IsEnabled($"{typeof(T).Name}_Remove"))
                _diagnosticSource.Write($"{typeof(T).Name}_Remove", value);

            _logger.LogTrace("Try to remove item");

            T result = await TryRemove(value);

            stop.Stop();
            _logger.LogTrace($"End Remove in {stop.Elapsed}");

            return new ExecutionResult<T>(true, result);
        }

        private async Task<T> TryRemove(T value)
        {
            var maxAttempted = 0;

            while (maxAttempted < _connectionOptions.RetryCount)
            {
                try
                {
                    return await _instance.RemoveAsync(value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "can not remove item");

                    var continues = _connectionOptions.Handles?.Invoke(ex);

                    if (continues == false) return null;
                }

                if (_connectionOptions.IntervalRetry > 0)
                    await Task.Delay(TimeSpan.FromSeconds(_connectionOptions.IntervalRetry));

                maxAttempted++;
            }

            _connectionOptions.FallBackAction?.Invoke(nameof(TryRemove), value);

            return null;            
        }

        /// <summary>
        /// Search a list of items
        /// </summary>
        /// <param name="filter">filter to be used to search items</param>
        /// <returns></returns>
        public async Task<ExecutionResult<IEnumerable<T>>> SearchAsync(Func<T, bool> filter)
        {
            var connectMessage = Connect();

            if (connectMessage != "OK")
                throw new ConnectionException(connectMessage);

            var stop = new Stopwatch();
            stop.Start();

            if (_diagnosticSource.IsEnabled($"{typeof(T).Name}_Search"))
                _diagnosticSource.Write($"{typeof(T).Name}_Search", 1);

            _logger.LogTrace("Try to search items");
            var result = await TrySearch(filter);

            stop.Stop();
            _logger.LogTrace($"End Search items in {stop.Elapsed}");

            return new ExecutionResult<IEnumerable<T>>(true, result);
        }

        private async Task<IEnumerable<T>> TrySearch(Func<T, bool> filter)
        {
            var maxAttempted = 0;

            while (maxAttempted < _connectionOptions.RetryCount)
            {
                try
                {
                    return await _instance.SearchAsync(filter);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "can not search item");

                    var continues = _connectionOptions.Handles?.Invoke(ex);

                    if (continues == false) return null;
                }

                if (_connectionOptions.IntervalRetry > 0)
                    await Task.Delay(TimeSpan.FromSeconds(_connectionOptions.IntervalRetry));

                maxAttempted++;
            }

            _connectionOptions.FallBackAction?.Invoke(nameof(TrySearch), filter);

            return null;
        }

        /// <summary>
        /// Search a single item
        /// </summary>
        /// <param name="filter">filter to be used to search the item</param>
        /// <returns></returns>
        public async Task<ExecutionResult<T>> SearchASingleItemAsync(Func<T, bool> filter)
        {
            var connectMessage = Connect();

            if (connectMessage != "OK")
                throw new ConnectionException(connectMessage);

            var stop = new Stopwatch();
            stop.Start();

            if (_diagnosticSource.IsEnabled($"{typeof(T).Name}_SearchASingleItem"))
                _diagnosticSource.Write($"{typeof(T).Name}_SearchASingleItem", 1);

            _logger.LogTrace("Try to search an item");
            T result = await TrySearchSingle(filter);

            stop.Stop();
            _logger.LogTrace($"End Search an item in {stop.Elapsed}");

            return new ExecutionResult<T>(true, result);
        }

        private async Task<T> TrySearchSingle(Func<T, bool> filter)
        {
            var maxAttempted = 0;

            while (maxAttempted < _connectionOptions.RetryCount)
            {
                try
                {
                    return await _instance.SearchASingleItemAsync(filter);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "can not select a single item");

                    var continues = _connectionOptions.Handles?.Invoke(ex);

                    if (continues == false) return null;
                }

                if (_connectionOptions.IntervalRetry > 0)
                    await Task.Delay(TimeSpan.FromSeconds(_connectionOptions.IntervalRetry));

                maxAttempted++;
            }

            _connectionOptions.FallBackAction?.Invoke(nameof(TrySearchSingle), filter);

            return null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if(disposing)
                _instance.Dispose();
        }
    }
}
