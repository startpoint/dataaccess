using System;

namespace Ecommerce.Data.RepositoryStore
{
    public class ConnectionOptions
    {
        public string ConnectionString { get; set; }
        public string Provider { get; set; }
        public int RetryCount { get; set; } = 3;
        public int IntervalRetry { get; set; }
        public Func<Exception, bool> Handles { get; set; }
        public Action FallbackConnection { get; set; }
        public Action<string, object> FallBackAction { get; set; }
    }
}