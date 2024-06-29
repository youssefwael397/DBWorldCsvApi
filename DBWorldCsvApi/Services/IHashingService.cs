namespace DBWorldCsvApi.Services
{
    public interface IHashingService
    {
        string Hash(string input);
        bool VerifyHash(string input, string hash);
    }
}
