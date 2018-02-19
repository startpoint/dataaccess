using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Data.EntityFramework
{
    public class SqlConnectionOptions
    {
        public string ConnectionString { get; set; }
        public bool SensitiveDataLoggingEnabled { get; set; }
        public bool QueryTrackingEnabled { get; set; }
        public bool LogWarningEnabled { get; set; }
    }
}
