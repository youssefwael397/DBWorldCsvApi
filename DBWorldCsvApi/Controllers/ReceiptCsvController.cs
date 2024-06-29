using DBWorldCsvApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DBWorldCsvApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "BasicAuthentication")]
    public class ReceiptCsvController : ControllerBase
    {
        private readonly CsvService _csvService;

        public ReceiptCsvController(CsvService csvService)
        {
            _csvService = csvService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty or null.");
            }

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                var cleanedRecords = await _csvService.GetCleanedRecordsAsync(stream);
                var transformedRecords = TransformRecords(cleanedRecords);
                return Ok(transformedRecords);
            }
        }

        private IEnumerable<dynamic> TransformRecords(IEnumerable<IDictionary<string, object>> records)
        {
            var groupedRecords = records.GroupBy(
                record => new { BusinessUnit = record["BusinessUnit"], ReceiptMethodID = record["ReceiptMethodID"] },
                (key, group) => new
                {
                    key.BusinessUnit,
                    key.ReceiptMethodID,
                    Transactions = group.Select(record =>
                    {
                        var transaction = new Dictionary<string, object>();
                        foreach (var kvp in record)
                        {
                            if (kvp.Key != "BusinessUnit" && kvp.Key != "ReceiptMethodID")
                            {
                                transaction[kvp.Key] = kvp.Value;
                            }
                        }
                        return transaction;
                    }).ToList()
                }
            );

            return groupedRecords;
        }
    }
}
