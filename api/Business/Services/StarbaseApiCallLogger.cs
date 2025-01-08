using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;

public class StarbaseApiCallLogger
{
    private readonly StargateContext _context;

    public StarbaseApiCallLogger(StargateContext context)
    {
        _context = context;
    }

    public async Task LogApiCall(string apiEndpoint, bool successStatus, string? changedField = null, string ?oldValue = null, string? newValue = null, string? errorLog = null)
    {
       try
        {
            var log = new StarbaseApiCallLog
            {
                ApiEndpoint = apiEndpoint ?? throw new ArgumentNullException(nameof(apiEndpoint)),
                SuccessStatus = successStatus,
                CallDate = DateTime.UtcNow,
                ChangedField = changedField,
                OldValue = oldValue,
                NewValue = newValue,
                ErrorLog = errorLog
            };

            _context.StarbaseApiCallLogs.Add(log);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Log the exception details for troubleshooting
            Console.WriteLine($"DbUpdateException: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"InnerException: {ex.InnerException.Message}");
            }
            throw;
        }
    }
}