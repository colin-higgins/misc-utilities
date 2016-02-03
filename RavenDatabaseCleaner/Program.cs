using System;

namespace RavenDatabaseCleaner
{
    using System.IO;
    using Raven.Client;
    using Raven.Client.Document;

    class Program
    {
        static void Main(string[] args)
        {
            var currentDirectory = Environment.CurrentDirectory;

            var databasePaths = Directory.GetDirectories(currentDirectory);

            foreach (var folderPath in databasePaths)
            {
                var fileName = Path.GetFileName(folderPath);
                var filePortions = fileName.Split('.');
                var databaseName = filePortions[0];

                if (databaseName.ToLower().Contains("system") || filePortions.Length > 1)
                {
                    continue;
                }

                try
                {
                    using (var documentStore = new DocumentStore
                    {
                        Url = "http://localhost:8083",
                        DefaultDatabase = databaseName,
                        ResourceManagerId = Guid.NewGuid() /* This is OK for our purposes */
                    })
                    {

                        documentStore.Initialize();
                        Cleanup(documentStore, databaseName);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

        }

        private static void Cleanup(IDocumentStore documentStore, string databaseName)
        {
            documentStore.DatabaseCommands.GlobalAdmin.DeleteDatabase(databaseName, hardDelete: true);
            Console.WriteLine("Deleted '{0}' database", databaseName);
        }
    }
}
