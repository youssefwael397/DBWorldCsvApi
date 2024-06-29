using DBWorldCsvApi.Repositories;

namespace DBWorldCsvApi.Services
{
    public class CsvService
    {
        private readonly ICsvRepository _csvRepository;

        public CsvService(ICsvRepository csvRepository)
        {
            _csvRepository = csvRepository;
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetCleanedRecordsAsync(Stream csvStream)
        {
            var records = await _csvRepository.GetRecordsAsync(csvStream);
            var cleanedRecords = records.Select(record => CleanKeys((IDictionary<string, object>)record)).ToList();
            return cleanedRecords;
        }

        private IDictionary<string, object> CleanKeys(IDictionary<string, object> record)
        {
            var cleanedRecord = new Dictionary<string, object>();
            foreach (var kvp in record)
            {
                var cleanedKey = CleanKey(kvp.Key);
                cleanedRecord[cleanedKey] = kvp.Value;
            }
            return cleanedRecord;
        }

        private string CleanKey(string key)
        {
            var cleanedKey = key.Replace(" ", "")
                                .Replace(".", "")
                                .Replace("-", "");
            return cleanedKey;
        }
    }
}
