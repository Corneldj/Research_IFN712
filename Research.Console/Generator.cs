using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Research.Cmd
{
    public static class Generator
    {
        public static void GenerateData(string path, int amount, bool verbose)
        {
            ConcurrentBag<DataEntry> bag = new ConcurrentBag<DataEntry>();
            Random rand = new Random();
            Console.WriteLine("Generating files...");
            ParallelLoopResult result = Parallel.For(0, amount, (i) =>
            {
                DataEntry entry = new DataEntry()
                {
                    Id = Guid.NewGuid(),
                    StringValue1 = Guid.NewGuid().ToString(),
                    StringValue2 = Guid.NewGuid().ToString(),
                    StringValue3 = Guid.NewGuid().ToString(),
                    IntValue1 = i,
                    IntValue2 = i,
                    IntValue3 = i,
                    DoubleValue1 = rand.NextDouble(),
                    DoubleValue2 = rand.NextDouble(),
                    DoubleValue3 = rand.NextDouble(),
                    DateTimeValue1 = DateTime.Now,
                    DateTimeValue2 = DateTime.UtcNow,
                    DateTimeValue3 = DateTime.MaxValue,
                };

                bag.Add(entry);
                if (verbose) Console.WriteLine($"Entry {i} Completed");
            });

            while (!result.IsCompleted) {  }
            WriteToCsv(bag, path);
        }

        public static void WriteToCsv(ConcurrentBag<DataEntry> dataBag, string filePath)
        {
            var sb = new StringBuilder();

            // Add header
            sb.AppendLine("Id,StringValue1,StringValue2,StringValue3,IntValue1,IntValue2,IntValue3,DateTimeValue1,DateTimeValue2,DateTimeValue3,DoubleValue1,DoubleValue2,DoubleValue3");

            // Add data
            foreach (var entry in dataBag)
            {
                sb.AppendLine($"{entry.Id},{Escape(entry.StringValue1)},{Escape(entry.StringValue2)},{Escape(entry.StringValue3)},{entry.IntValue1},{entry.IntValue2},{entry.IntValue3},{entry.DateTimeValue1.ToString(CultureInfo.InvariantCulture)},{entry.DateTimeValue2.ToString(CultureInfo.InvariantCulture)},{entry.DateTimeValue3.ToString(CultureInfo.InvariantCulture)},{entry.DoubleValue1.ToString(CultureInfo.InvariantCulture)},{entry.DoubleValue2.ToString(CultureInfo.InvariantCulture)},{entry.DoubleValue3.ToString(CultureInfo.InvariantCulture)}");
            }

            File.WriteAllText(filePath, sb.ToString());
        }

        // Utility function to handle CSV escaping
        private static string Escape(string s)
        {
            if (s.Contains(",") || s.Contains("\"") || s.Contains("\n"))
            {
                return $"\"{s.Replace("\"", "\"\"")}\"";
            }
            else
            {
                return s;
            }
        }
    }
}
