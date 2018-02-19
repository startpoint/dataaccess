using System;
using Ecommerce.Data.RepositoryStore;
using ECommerce.Data.NoSql;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace ECommerce.Data.Tests
{
    [TestClass]
    public class DocumentDbStoreTest : TestBase
    {
        private readonly ILoggerFactory _loggerFactory;
        private RepositoryStore<TestDocument> _repository;

        public DocumentDbStoreTest()
        {
            _loggerFactory = new LoggerFactory();
            _loggerFactory.AddConsole();
        }

        #region Initialize and cleanup methods
        [TestInitialize]
        public void StartTestVoid()
        {
            var config = new NoSqlConnectionString
            {
                Url = "https://rechercherunproduit-dev-sql.documents.azure.com:443/",
                AuthorizationKey =
                    "IWLYmQ6TcRhGDFVN6W2eQAYMJD8m0FFd0MKtV7wPiAQ8EDD8pnhs6b5hSvnXxbZHAhGQQWlvVCe1HqofPjnp7w==",
                Database = "Configuration",
                Collection = $"ConfigurationItem_{Guid.NewGuid()}"
            };

            _repository = new RepositoryStore<TestDocument>(
                "ECommerce.Data.NoSql",
                new ConnectionOptions
                {
                       Provider = "ECommerce.Data.DocumentDbProvider",
                    ConnectionString = JsonConvert.SerializeObject(config)
                },
                _loggerFactory, new MyDiagnosticSource());
        }

        [TestCleanup]
        public void EndTestVoid()
        {
            _repository.Dispose();
        }
        #endregion

        [DataRow("Catalog")]
        [TestMethod]
        public void WithDocumentDb_ShouldAddSomeItemsAndReturnTheListOfByApplication(string applicationName)
        {
            var key = Guid.NewGuid().ToString();
            TestInsert(_repository, applicationName, key, "DataSource=local,Database=Catalog,User Id=sa,Password=sql128!");
        }

        [TestMethod]
        public void WithDocumentDb_ShouldAddAItemGetItByKeyThenUpdateIt()
        {
            var key = Guid.NewGuid().ToString();

            TestInsert(_repository, applicationName: "Test", key: key, value: "test");
            TestUpdate(_repository, applicationName: "Test", key: key, value: "mod");
        }

        [TestMethod]
        public void WithDocumentDb_ShouldAddAItemGetItByKeyThenDeleteIt()
        {
            var key = Guid.NewGuid().ToString();

            TestInsert(_repository, applicationName: "Test", key: key, value: "test");
            TestDelete(_repository, applicationName: "Test", key: key);
        }

        [TestMethod, DataRow("Catalog")]
        public void WithDocumentDb_ShouldAddTwoItemsWithSameKey(string applicationName)
        {
            var key = Guid.NewGuid().ToString();

            TestInsert(_repository, applicationName: "Test", key: key, value: "test");
            TestInsertDuplicate(_repository, applicationName: "Test", key: key, value: "test");
        }
    }
}