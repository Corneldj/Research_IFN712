using MongoDB.Driver;
using SharpCompress.Common;

namespace Research.Cmd.Databases
{
    internal static class MongoDbWrapper
    {
        internal static void InitializeMongoDB()
        {
            // MongoDB will auto-create collections when data is inserted.
            // However, you can explicitly create a collection if needed.
            var client = new MongoClient("mongodb://root:examplepassword@localhost:27017");
            var database = client.GetDatabase("testdb");
            if (!database.ListCollectionNames().ToList().Contains("dataentries"))
            {
                database.CreateCollection("dataentries");
            }
        }

        internal static void InsertSet(IEnumerable<DataEntry> entries, bool verbose)
        {
            var client = new MongoClient("mongodb://root:examplepassword@localhost:27017");
            var database = client.GetDatabase("testdb");
            var collection = database.GetCollection<DataEntry>("dataentries");

            var loop = Parallel.ForEach(entries, (entry) =>
            {
                collection.InsertOne(entry);
         
            });

            while (!loop.IsCompleted) { }
            if (verbose)
                Console.WriteLine("Redis insert complete");
        }
    }
}
