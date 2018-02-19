using System.Security;

namespace ECommerce.Data.NoSql
{
    public class NoSqlConnectionString
    {
        public string Url { get; set; }
        public string Database { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Certificate { get; set; }
        public string AuthorizationKey { get; set; }
        public string Collection { get; set; }
        public bool IsTest { get; set; }
    }
}