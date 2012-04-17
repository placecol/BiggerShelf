using System.Reflection;
using Kaiser.BiggerShelf.Web.Models;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace Kaiser.BiggerShelf.Web.Infrastructure.Raven
{
    public class DocumentStoreHolder
    {
        public static IDocumentStore Store { get; set; }

        static DocumentStoreHolder()
        {
            Store = new DocumentStore {ConnectionStringName = "RavenDB"};
            Store.Initialize();

            BigShelfRavenDbInitializer.Seed(Store.OpenSession());

            IndexCreation.CreateIndexes(Assembly.GetExecutingAssembly(), Store);
        }
    }
}