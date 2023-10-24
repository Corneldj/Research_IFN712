using MySql.Data.MySqlClient;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Research.Cmd.Databases
{
    internal static class MySqlWrapper
    {
        internal static void InitializeMySQL()
        {
            string connectionString = "server=localhost;user=user;database=mydatabase;password=userpassword";
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            string createTableQuery =
                @"CREATE TABLE IF NOT EXISTS DataEntry (
                    Id CHAR(36) PRIMARY KEY,
                    StringValue1 LONGTEXT,
                    StringValue2 LONGTEXT,
                    StringValue3 LONGTEXT,
                    IntValue1 INT,
                    IntValue2 INT,
                    IntValue3 INT,
                    DateTimeValue1 DATETIME,
                    DateTimeValue2 DATETIME,
                    DateTimeValue3 DATETIME,
                    DoubleValue1 FLOAT,
                    DoubleValue2 FLOAT,
                    DoubleValue3 FLOAT
                );

                ";

            using var cmd = new MySqlCommand(createTableQuery, connection);
            cmd.ExecuteNonQuery();
        }

        internal static void InsertSet(IEnumerable<DataEntry> entries, bool verbose)
        {
            var query = QueryBuilder(entries);
            string connectionString = "server=localhost;user=user;database=mydatabase;password=userpassword";
            using var connection = new MySqlConnection(connectionString);
            connection.Open();


            using var cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            if (verbose)
                Console.WriteLine($"Inserted Sql Batch");
        }


        static string QueryBuilder(IEnumerable<DataEntry> entries)
        {
            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append(@"
INSERT INTO DataEntry
(Id, StringValue1, StringValue2, StringValue3, IntValue1, IntValue2, IntValue3, DateTimeValue1, DateTimeValue2, DateTimeValue3, DoubleValue1, DoubleValue2, DoubleValue3)
VALUES
");

            List<string> valueSets = new List<string>();
            foreach (var entry in entries)
            {
                string valueSet = string.Format(
                    "( '{0}', '{1}', '{2}', '{3}', {4}, {5}, {6}, '{7}', '{8}', '{9}', {10}, {11}, {12} )",
                    entry.Id,
                    entry.StringValue1, entry.StringValue2, entry.StringValue3,
                    entry.IntValue1, entry.IntValue2, entry.IntValue3,
                    entry.DateTimeValue1.ToString("yyyy-MM-dd HH:mm:ss"),
                    entry.DateTimeValue2.ToString("yyyy-MM-dd HH:mm:ss"),
                    entry.DateTimeValue3.ToString("yyyy-MM-dd HH:mm:ss"),
                    entry.DoubleValue1, entry.DoubleValue2, entry.DoubleValue3
                );
                valueSets.Add(valueSet);
            }

            sqlCommand.Append(string.Join(",", valueSets));
            sqlCommand.Append(";");
            return sqlCommand.ToString();
        }
    }
}
