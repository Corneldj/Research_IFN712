using Cassandra;

namespace Research.Cmd.Databases
{


    internal static class CassandraWrapper
    {
        internal static void InitializeCassandra()
        {
            var cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
            var session = cluster.Connect();

            // Create keyspace if not exists
            session.Execute("CREATE KEYSPACE IF NOT EXISTS testkeyspace WITH REPLICATION = { 'class' : 'SimpleStrategy', 'replication_factor' : 1 }");
            session.ChangeKeyspace("testkeyspace");

            // Create table if not exists
            session.Execute(@"
                CREATE TABLE IF NOT EXISTS DataEntry (
                    Id UUID PRIMARY KEY,
                    StringValue1 TEXT,
                    StringValue2 TEXT,
                    StringValue3 TEXT,
                    IntValue1 INT,
                    IntValue2 INT,
                    IntValue3 INT,
                    DateTimeValue1 TIMESTAMP,
                    DateTimeValue2 TIMESTAMP,
                    DateTimeValue3 TIMESTAMP,
                    DoubleValue1 DOUBLE,
                    DoubleValue2 DOUBLE,
                    DoubleValue3 DOUBLE
                );");
        }

        internal static void InsertSet(IEnumerable<DataEntry> entries)
        {
            while (Parallel.ForEach(entries, entry => Insert(entry)).IsCompleted) { }
        }

       internal static void Insert(DataEntry entry)
        {
            var cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
            var session = cluster.Connect("testkeyspace");

            string insertQuery = $@"
                INSERT INTO DataEntry (
                    Id, StringValue1, StringValue2, StringValue3,
                    IntValue1, IntValue2, IntValue3,
                    DateTimeValue1, DateTimeValue2, DateTimeValue3,
                    DoubleValue1, DoubleValue2, DoubleValue3
                ) VALUES (
                    {entry.Id}, '{entry.StringValue1}', '{entry.StringValue2}', '{entry.StringValue3}',
                    {entry.IntValue1}, {entry.IntValue2}, {entry.IntValue3},
                    '{entry.DateTimeValue1:yyyy-MM-dd HH:mm:ss}', '{entry.DateTimeValue2:yyyy-MM-dd HH:mm:ss}', '{entry.DateTimeValue3:yyyy-MM-dd HH:mm:ss}',
                    {entry.DoubleValue1}, {entry.DoubleValue2}, {entry.DoubleValue3}
                )";

            session.Execute(insertQuery);
            Console.WriteLine($"Inserted entry with ID {entry.Id} into Cassandra");
        }

    }
}
