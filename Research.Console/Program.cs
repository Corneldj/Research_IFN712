using Cassandra;
using CommandLine;
using MongoDB.Bson;
using MongoDB.Driver;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using Research.Cmd;
using Research.Cmd.Databases;
using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Xml.Serialization;

Parser
    .Default
    .ParseArguments<Options>(args)
    .WithParsed(o =>
    {
        if (o.Amount != null)
        {
            Generator.GenerateData(o.Path, o.Amount.Value, o.Verbose);
        }

        if (!File.Exists(o.Path))
        {
            Console.WriteLine($"{Path.GetFileName(o.Path)} does not exist.");
            return;
        }

        /* Initializes */
        IEnumerable<DataEntry> entries = CsvReaderHelper.ReadCsv<DataEntry>(o.Path);

        MongoDbWrapper.InitializeMongoDB();
        MySqlWrapper.InitializeMySQL();
        CassandraWrapper.InitializeCassandra();
        // No need for Redis initializer

        ///* Inserts */
        Stopwatch sw = new Stopwatch();
        //Console.WriteLine("================================================================");
        //Console.WriteLine("MondoDb Insert");
        //sw.Start();
        //MongoDbWrapper.InsertSet(entries, o.Verbose);
        //sw.Stop();
        //Console.WriteLine($"For {entries.Count()} records it took: {sw.Elapsed.TotalMilliseconds}");

        Console.WriteLine("================================================================");
        Console.WriteLine("MySql Insert");
        sw.Reset();
        sw.Start();
        MySqlWrapper.InsertSet(entries, o.Verbose);
        sw.Stop();
        Console.WriteLine($"For {entries.Count()} records it took: {sw.Elapsed.TotalMilliseconds}");


        //Console.WriteLine("================================================================");
        //Console.WriteLine("Cassandra Insert");

        //sw.Reset();
        //sw.Start();
        //CassandraWrapper.InsertSet(entries, o.Verbose);
        //sw.Stop();
        //Console.WriteLine($"For {entries.Count()} records it took: {sw.Elapsed.TotalMilliseconds}");


        //Console.WriteLine("================================================================");
        //Console.WriteLine("Redis Insert");

        //sw.Reset();
        //sw.Start();
        //RedisWrapper.InsertSet(entries, o.Verbose);
        //sw.Stop();
        //Console.WriteLine($"For {entries.Count()} records it took: {sw.Elapsed.TotalMilliseconds}");
    });





static List<DataEntry> ReadCSv(string path)
{
    XmlSerializer serializer = new XmlSerializer(typeof(List<DataEntry>), new XmlRootAttribute("persons"));
    using StreamReader reader = new StreamReader(path);
    return (List<DataEntry>)serializer.Deserialize(reader);
}

static void InsertMongoDB(DataEntry record)
{
    var client = new MongoClient("mongodb://root:examplepassword@localhost:27017");
    var database = client.GetDatabase("testdb");
    var collection = database.GetCollection<BsonDocument>("testcollection");
    var document = new BsonDocument { { "name", "John Doe" }, { "age", 30 } };
    collection.InsertOne(document);
    Console.WriteLine("Inserted record into MongoDB");
}

static void InsertMySQL(DataEntry record)
{
    string connectionString = "server=localhost;user=user;database=mydatabase;password=userpassword";
    using var connection = new MySqlConnection(connectionString);
    connection.Open();
    using var cmd = new MySqlCommand("INSERT INTO testtable (name, age) VALUES ('John Doe', 30)", connection);
    cmd.ExecuteNonQuery();
    Console.WriteLine("Inserted record into MySQL");
}

static void InsertCassandra(DataEntry record)
{
    var cluster = Cluster.Builder().AddContactPoint("127.0.0.1").Build();
    var session = cluster.Connect("testkeyspace");
    session.Execute("INSERT INTO testtable (id, name, age) VALUES (uuid(), 'John Doe', 30)");
    Console.WriteLine("Inserted record into Cassandra");
}

static void InsertRedis(DataEntry record)
{
    ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");
    var db = redis.GetDatabase();
    db.StringSet("name", "John Doe");
    Console.WriteLine("Inserted record into Redis");
}



