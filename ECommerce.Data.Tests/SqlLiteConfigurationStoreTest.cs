using System;
using System.IO;
using Ecommerce.Data.RepositoryStore;
using ECommerce.Data.EntityFramework;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ECommerce.Data.Tests
{
    [TestClass]
    public class SqlLiteConfigurationStoreTest : TestBase
    {
        private readonly ILoggerFactory _loggerFactory;
        private RepositoryStore<TestDocument> _repository;

        public SqlLiteConfigurationStoreTest()
        {
            _loggerFactory = new LoggerFactory();
            _loggerFactory.AddConsole();
        }

        [TestInitialize]
        public void StartTestVoid()
        {
            var sqlConnection = new SqlConnectionOptions { ConnectionString = $"Data Source={new FileInfo("data\\data.db").FullName}" };

            _repository = new RepositoryStore<TestDocument>("ECommerce.Data.EntityFramework",
                new ConnectionOptions
                {
                    Provider = "ECommerce.Data.EntityFramework.SqlLite",
                    ConnectionString = JsonConvert.SerializeObject(sqlConnection)
                },
                _loggerFactory, new MyDiagnosticSource());
        }

        [TestCleanup]
        public void EndTestVoid()
        {
            _repository.Dispose();
        }

        [DataRow("Catalog")]
        [TestMethod]
        public void WithSqlLite_ShouldAddSomeItemsAndReturnTheListOfByApplication(string applicationName)
        {
            var key = Guid.NewGuid().ToString();
            TestInsert(_repository, applicationName, key, "DataSource=local,Database=Catalog,User Id=sa,Password=sql128!");
        }

        [TestMethod]
        public void WithSqlLite_ShouldAddAItemGetItByKeyThenUpdateIt()
        {
            var key = Guid.NewGuid().ToString();

            TestInsert(_repository, applicationName: "Test", key: key, value: "test");
            TestUpdate(_repository, applicationName: "Test", key: key, value: "mod");
        }

        [TestMethod]
        public void WithSqlLite_ShouldAddAItemGetItByKeyThenDeleteIt()
        {
            var key = Guid.NewGuid().ToString();

            TestInsert(_repository, applicationName: "Test", key: key, value: "test");
            TestDelete(_repository, applicationName: "Test", key: key);
        }

        [TestMethod, DataRow("Catalog")]
        public void WithSqlLite_ShouldAddTwoItemsWithSameKey(string applicationName)
        {
            var key = Guid.NewGuid().ToString();

            TestInsert(_repository, applicationName: "Test", key: key, value: "test");
            TestInsertDuplicate(_repository, applicationName: "Test", key: key, value: "test");
        }
    }
}