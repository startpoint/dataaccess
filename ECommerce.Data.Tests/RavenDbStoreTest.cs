using System;
using Ecommerce.Data.RepositoryStore;
using ECommerce.Data.NoSql;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ECommerce.Data.Tests
{
    [TestClass]
    public class RavenDbStoreTest : TestBase
    {
        private readonly ILoggerFactory _loggerFactory;
        private RepositoryStoreFactory<TestDocument> _repository;

        public RavenDbStoreTest()
        {
            _loggerFactory = new LoggerFactory();
            _loggerFactory.AddConsole();
        }

        [TestInitialize]
        public void StartTestVoid()
        {
            var config = new NoSqlConnectionString
            {
                Url = "http://localhost:8080",
                Database = "",
                IsTest = true
            };

            _repository = new RepositoryStoreFactory<TestDocument>("ECommerce.Data.NoSql",
                new ConnectionOptions { Provider = "ECommerce.Data.RavenDbProvider", ConnectionString = JsonConvert.SerializeObject(config) },
                _loggerFactory, new MyDiagnosticSource());
        }

        [TestCleanup]
        public void EndTestVoid()
        {
            _repository.Dispose();
        }

        [DataRow("Catalog")]
        [TestMethod]
        public void WithRavenDb_ShouldAddSomeItemsAndReturnTheListOfByApplication(
            string applicationName)
        {
            var key = Guid.NewGuid().ToString();
            TestInsert(_repository, applicationName, key,
                "DataSource=local,Database=Catalog,User Id=sa,Password=sql128!");
        }

        [TestMethod]
        public void WithRavenDb_ShouldAddAItemGetItByKeyThenUpdateIt()
        {
            var key = Guid.NewGuid().ToString();

            TestInsert(_repository, applicationName: "Test", key: key, value: "test");
            TestUpdate(_repository, applicationName: "Test", key: key, value: "mod");
        }

        [TestMethod]
        public void WithRavenDb_ShouldAddAItemGetItByKeyThenDeleteIt()
        {
            var key = Guid.NewGuid().ToString();

            TestInsert(_repository, applicationName: "Test", key: key, value: "test");
            TestDelete(_repository, applicationName: "Test", key: key);
        }

        [TestMethod, DataRow("Catalog")]
        public void WithRavenDb_ShouldAddTwoItemsWithSameKey(string applicationName)
        {
            var key = Guid.NewGuid().ToString();

            TestInsert(_repository, applicationName: "Test", key: key, value: "test");
            TestInsertDuplicate(_repository, applicationName: "Test", key: key, value: "test");
        }
    }
}