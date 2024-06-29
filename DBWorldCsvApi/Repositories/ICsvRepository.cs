namespace DBWorldCsvApi.Repositories
{
    public interface ICsvRepository
    {
        Task<IEnumerable<dynamic>> GetRecordsAsync(Stream csvStream);
    }
}
