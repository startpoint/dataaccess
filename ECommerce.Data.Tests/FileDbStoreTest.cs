using System;
using System.IO;
using Ecommerce.Data.RepositoryStore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECommerce.Data.Tests
{
    [TestClass]
    public class FileDbStoreTests:TestBase
    {
        private readonly ILoggerFactory _loggerFactory;
        private RepositoryStore<TestDocument> _repository;

        public FileDbStoreTests()
        {
            _loggerFactory = new LoggerFactory();
            _loggerFactory.AddConsole();
        }

        [TestInitialize]
        public void StartTestVoid()
        {
            _repository = new RepositoryStore<TestDocument>("ECommerce.Data.FileStore",
                new ConnectionOptions { Provider = "FileDb", ConnectionString = new FileInfo($"data\\data_{Guid.NewGuid()}.json").FullName},
                _loggerFactory, new MyDiagnosticSource());
        }

        [TestCleanup]
        public void EndTestVoid()
        {
            _repository.Dispose();
        }

        [DataRow("Catalog")]
        [TestMethod]
        public void WithFileDb_ShouldAddSomeItemsAndReturnTheListOfByApplication(string applicationName)
        {
            var key = Guid.NewGuid().ToString();
            TestInsert(_repository, applicationName, key, "DataSource=local,Database=Catalog,User Id=sa,Password=sql128!");
        }

        [TestMethod]
        public void WithFileDb_ShouldAddAItemGetItByKeyThenUpdateIt()
        {
            var key = Guid.NewGuid().ToString();

            TestInsert(_repository, applicationName: "Test", key: key, value: "test");
            TestUpdate(_repository, applicationName: "Test", key: key, value: "mod");
        }

        [TestMethod]
        public void WithFileDb_ShouldAddAItemGetItByKeyThenDeleteIt()
        {
            var key = Guid.NewGuid().ToString();

            TestInsert(_repository, applicationName: "Test", key: key, value: "test");
            TestDelete(_repository, applicationName: "Test", key: key);
        }

        [TestMethod, DataRow("Catalog")]
        public void WithFileDb_ShouldAddTwoItemsWithSameKey(string applicationName)
        {
            var key = Guid.NewGuid().ToString();

            TestInsert(_repository, applicationName: "Test", key: key, value: "test");
            TestInsertDuplicate(_repository, applicationName: "Test", key: key, value: "test");
        }
    }
}
