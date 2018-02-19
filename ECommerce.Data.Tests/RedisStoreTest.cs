using System;
using Ecommerce.Data.RepositoryStore;
using ECommerce.Data.NoSql;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ECommerce.Data.Tests
{
    [TestClass]
    public class RedisStoreTest : TestBase
    {
        private readonly ILoggerFactory _loggerFactory;
        private RepositoryStore<TestDocument> _repository;

        public RedisStoreTest()
        {
            _loggerFactory = new LoggerFactory();
            _loggerFactory.AddConsole();
        }

        [TestInitialize]
        public void StartTestVoid()
        {
            var config = new NoSqlConnectionString
            {
                Url = "localhost"
            };

            _repository = new RepositoryStore<TestDocument>("ECommerce.Data.NoSql",
                new ConnectionOptions { Provider = "ECommerce.Data.RedisProvider", ConnectionString = JsonConvert.SerializeObject(config) },
                _loggerFactory, new MyDiagnosticSource());
        }

        [TestCleanup]
        public void EndTestVoid()
        {
            _repository.Dispose();
        }

        [DataRow("Catalog")]
        [TestMethod]
        public void WithRedis_ShouldAddSomeItemsAndReturnTheListOfByApplication(
            string applicationName)
        {
            var key = Guid.NewGuid().ToString();
            TestInsert(_repository, applicationName, key,
                "DataSource=local,Database=Catalog,User Id=sa,Password=sql128!");
        }

        [TestMethod]
        public void WithRedis_ShouldAddAItemGetItByKeyThenUpdateIt()
        {
            var key = Guid.NewGuid().ToString();

            TestInsert(_repository, applicationName: "Test", key: key, value: "test");
            TestUpdate(_repository, applicationName: "Test", key: key, value: "mod");
        }

        [TestMethod]
        public void WithRedis_ShouldAddAItemGetItByKeyThenDeleteIt()
        {
            var key = Guid.NewGuid().ToString();

            TestInsert(_repository, applicationName: "Test", key: key, value: "test");
            TestDelete(_repository, applicationName: "Test", key: key);
        }

        [TestMethod, DataRow("Catalog")]
        public void WithRedis_ShouldAddTwoItemsWithSameKey(string applicationName)
        {
            var key = Guid.NewGuid().ToString();

            TestInsert(_repository, applicationName: "Test", key: key, value: "test");
            TestInsertDuplicate(_repository, applicationName: "Test", key: key, value: "test");
        }
    }
}