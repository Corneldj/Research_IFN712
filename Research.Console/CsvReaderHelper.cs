using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace Research.Cmd
{
    public static class CsvReaderHelper
    {
        public static IEnumerable<TModel> ReadCsv<TModel>(string filePath, CsvConfiguration config = null) where TModel : class, new()
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config ?? new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                return csv.GetRecords<TModel>().ToList();
            }
        }
    }
}
