using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Research.Cmd.Databases
{
    internal static class RedisWrapper
    {
        public static void Insert(DataEntry entry, IDatabase db, bool verbose)
        {
            string jsonData = JsonConvert.SerializeObject(entry);
            db.StringSet(entry.Id.ToString(), jsonData);
            if (verbose)
                Console.WriteLine($"Inserted entry with ID {entry.Id} into Redis");
        }

        public static void InsertSet(IEnumerable<DataEntry> entries, bool verbose)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");
            IDatabase db = redis.GetDatabase();
            var loop = Parallel.ForEach(entries, entry => Insert(entry, db, verbose));
            while (!loop.IsCompleted) { }
        }
    }
}
