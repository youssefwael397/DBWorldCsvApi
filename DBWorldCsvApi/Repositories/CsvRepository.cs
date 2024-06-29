using CsvHelper;
using System.Globalization;
using System.Text;

namespace DBWorldCsvApi.Repositories
{
    public class CsvRepository : ICsvRepository
    {
        public async Task<IEnumerable<dynamic>> GetRecordsAsync(Stream csvStream)
        {
            using (var reader = new StreamReader(csvStream, Encoding.UTF8))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<dynamic>().ToList();
                return records;
            }
        }
    }
}
