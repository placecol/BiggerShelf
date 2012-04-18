using System;
using Raven.Client;
using Raven.Client.Embedded;

namespace Kaiser.BiggerShelf.Test
{
    public static class RavenTestExtentions
    {
        public static void ExecuteInStore(this IRavenTest test, object[] dbObjects = null, Action<IDocumentStore> action = null)
        {
            using (var store = new EmbeddableDocumentStore())
            {
                store.Configuration.RunInMemory = true;
                store.Configuration.RunInUnreliableYetFastModeThatIsNotSuitableForProduction = true;
                store.Initialize();

                using (var session = store.OpenSession())
                {
                    foreach (var obj in dbObjects)
                    {
                        session.Store(obj);
                    }
                    session.SaveChanges();
                }

                action(store);
            }
        }
    }
}
