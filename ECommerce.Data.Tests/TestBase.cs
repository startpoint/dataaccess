using System;
using System.Data;
using Ecommerce.Data.RepositoryStore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECommerce.Data.Tests
{
    public class TestBase
    {
        public void TestInsert(RepositoryStoreFactory<TestDocument> repository,  string applicationName, string key, string value)
        {
            var result = repository.AddAsync(new TestDocument
            {
                ApplicationName = applicationName,
                Key = key,
                Value = value
            }).Result;

            Assert.IsTrue(result.IsSuccessful);

            var searchResult = repository.SearchASingleItemAsync(x => x.Key == key).Result;

            Assert.IsNotNull(searchResult);
            Assert.IsTrue(searchResult.IsSuccessful);
        }

        public void TestUpdate(RepositoryStoreFactory<TestDocument> repository, string applicationName, string key, string value)
        {
            var searchResult = repository.SearchASingleItemAsync(x => x.Key == key).Result;

            Assert.IsNotNull(searchResult);
            Assert.IsTrue(searchResult.IsSuccessful);

            var item = searchResult.Result;

            Assert.IsNotNull(item);

            item.Value = "mod";

            var updateResult = repository.UpdateAsync(item).Result;

            Assert.IsNotNull(updateResult);
            Assert.IsTrue(updateResult.IsSuccessful);
            Assert.IsTrue(updateResult.Result.Value.Equals("mod"));
        }

        public void TestDelete(RepositoryStoreFactory<TestDocument> repository, string applicationName, string key)
        {
            var searchResult = repository.SearchASingleItemAsync(x => x.Key == key).Result;

            Assert.IsNotNull(searchResult);
            Assert.IsTrue(searchResult.IsSuccessful);

            var item = searchResult.Result;

            Assert.IsNotNull(item);

            item.Value = "mod";

            var removeResult = repository.RemoveAsync(item).Result;

            Assert.IsNotNull(removeResult);
            Assert.IsTrue(removeResult.IsSuccessful);
        }

        public void TestInsertDuplicate(RepositoryStoreFactory<TestDocument> repository, string applicationName, string key, string value)
        {
            Assert.ThrowsException<DuplicateNameException>(() =>
            {
                try
                {
                    repository.AddAsync(new TestDocument
                    {
                        ApplicationName = applicationName,
                        Key = key,
                        Value = value
                    }).Wait();
                }
                catch (AggregateException ex)
                {
                    if (ex is AggregateException aggregate)
                    {
                        aggregate.Handle((x) => throw x);
                    }

                    throw;
                }
            });
        }
    }
}
