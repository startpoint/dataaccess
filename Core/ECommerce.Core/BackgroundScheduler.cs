using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ECommerce.Core
{
    public class BackgroundWorkScheduler : IHostedService, IDisposable
    {

        // Flag: Has Dispose already been called?
        private bool _disposed;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ILogger<BackgroundWorkScheduler> _logger;
        private readonly BackgroundWorkSchedulerOptions _options;
        private int _workInProgress;

        public BackgroundWorkScheduler(ILogger<BackgroundWorkScheduler> logger,
                                       BackgroundWorkSchedulerOptions options)
        {
            _logger = logger;
            _options = options;
        }

        private void CancelTasks()
        {
            lock (this)
            {
                try
                {
                    _cancellationTokenSource.Cancel();
                }
                catch (AggregateException ex)
                {
                    foreach (var inner in ex.InnerExceptions)
                    {
                        if (inner is TaskCanceledException)
                        {
                            _logger.LogInformation("Background work cancelled during shutdown.");
                        }
                        else
                        {
                            _logger.LogError("Error cancelling background work {0}", ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Exception occured cancelling background work: {0}", ex);
                }
            }
        }

        public void QueueWork(Action<CancellationToken> work)
        {
            if (work == null)
            {
                throw new ArgumentNullException($"Work cannot be null.");
            }

            if (_cancellationTokenSource.IsCancellationRequested)
            {
                throw new InvalidOperationException($"You cannot queue new background work when shutdown has started.");
            }

            ThreadPool.UnsafeQueueUserWorkItem(state =>
            {
                lock (this)
                {
                    if (_cancellationTokenSource.IsCancellationRequested)
                    {
                        return;
                    }
                    else
                    {
                        _workInProgress++;
                    }
                }

                RunWork((Action<CancellationToken>)state);
            }, work);
        }

        private void RunWork(Action<CancellationToken> work)
        {
            var token = _cancellationTokenSource.Token;

            try
            {
                work(token);
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException operationCanceled && operationCanceled.CancellationToken == token)
                {
                    _logger.LogInformation($"Work cancelled by throwing {nameof(OperationCanceledException)}");
                    return;
                }

                _logger.LogError($"Exception occured in background task: {ex}");
            }
            finally
            {
                lock (this)
                {
                    _workInProgress--;
                }
            }
        }

        public void Start()
        {
            if (_cancellationTokenSource.IsCancellationRequested)
            {
                //TODO: We can probably allow this and re-instantiate the cancellationTokenSource if there is no work in progress.
                throw new InvalidOperationException($"The {nameof(BackgroundWorkScheduler)} cannot be started again after it has stopped.");
            }
        }

        public void Stop()
        {
            CancelTasks();

            for (var i = 0; i < _options.Timeout.TotalMilliseconds; i++)
            {
                int curentWorkInProgress;
                lock (this)
                {
                    curentWorkInProgress = _workInProgress;
                }

                if (curentWorkInProgress == 0)
                {
                    return;
                }

                Thread.Sleep(1);
            }
            _logger.LogError("Unable to gracefully shutdown all background work.");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() => Start(), cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() => Stop(), cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if(_disposed)
                return;

            if (disposing)
            {
                _cancellationTokenSource.Dispose();
            }

            _disposed = true;
        }
    }
}