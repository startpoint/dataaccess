using Raven.Client.Documents;
using Raven.TestDriver;

namespace ECommerce.Data.RavenDbProvider
{
    internal class RavenDbTestDriver: RavenTestDriver<RavenDbLocator>
    {
        protected override void PreInitialize(IDocumentStore documentStore)
        {
            documentStore.Conventions.MaxNumberOfRequestsPerSession = 50;
        }
    }
}