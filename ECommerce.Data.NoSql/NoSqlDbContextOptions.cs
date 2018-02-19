using Microsoft.Extensions.Logging;

namespace ECommerce.Data.NoSql
{
    public class NoSqlDbContextOptions
    {
        public NoSqlConnectionString Connectionstring { get; }
        public ILoggerFactory LoggerFactory { get; }
        public bool IsTest { get; set; }

        public NoSqlDbContextOptions(NoSqlConnectionString connectionstring, ILoggerFactory loggerFactory, bool isTest)
        {
            Connectionstring = connectionstring;
            LoggerFactory = loggerFactory;
            IsTest = isTest;
        }

    }
}