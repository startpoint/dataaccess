using System;
using Ecommerce.Data.RepositoryStore;
using ECommerce.Data.EntityFramework;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ECommerce.Data.Tests
{
    [TestClass]
    public class SqlServerConfigurationStoreTest : TestBase
    {
        private readonly ILoggerFactory _loggerFactory;
        private RepositoryStoreFactory<TestDocument> _repository;

        public SqlServerConfigurationStoreTest()
        {
            _loggerFactory = new LoggerFactory();
            _loggerFactory.AddConsole();
        }

        [TestInitialize]
        public void StartTestVoid()
        {
            var sqlConnection = new SqlConnectionOptions { ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=EFProviders.SqlServer;Trusted_Connection=True;ConnectRetryCount=3" };

            _repository = new RepositoryStoreFactory<TestDocument>("ECommerce.Data.EntityFramework",
                new ConnectionOptions
                {
                    Provider = "ECommerce.Data.EntityFramework.SqlServer",
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
        public void WithSqlServer_ShouldAddSomeItemsAndReturnTheListOfByApplication(string applicationName)
        {
            var key = Guid.NewGuid().ToString();
            TestInsert(_repository, applicationName, key, "DataSource=local,Database=Catalog,User Id=sa,Password=sql128!");
        }

        [TestMethod]
        public void WithSqlServer_ShouldAddAItemGetItByKeyThenUpdateIt()
        {
            var key = Guid.NewGuid().ToString();

            TestInsert(_repository, applicationName: "Test", key: key, value: "test");
            TestUpdate(_repository, applicationName: "Test", key: key, value: "mod");
        }

        [TestMethod]
        public void WithSqlServer_ShouldAddAItemGetItByKeyThenDeleteIt()
        {
            var key = Guid.NewGuid().ToString();

            TestInsert(_repository, applicationName: "Test", key: key, value: "test");
            TestDelete(_repository, applicationName: "Test", key: key);
        }

        [TestMethod, DataRow("Catalog")]
        public void WithSqlServer_ShouldAddTwoItemsWithSameKey(string applicationName)
        {
            var key = Guid.NewGuid().ToString();

            TestInsert(_repository, applicationName: "Test", key: key, value: "test");
            TestInsertDuplicate(_repository, applicationName: "Test", key: key, value: "test");
        }
    }
}