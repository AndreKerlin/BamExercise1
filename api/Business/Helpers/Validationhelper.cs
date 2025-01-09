using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace StargateAPI.Helpers
{
        public static class ValidationHelper
    {
        public static async Task<IActionResult> ValidateNameAsync(string name, string action, string field, StarbaseApiCallLogger apiLogger, ControllerBase controller)
        {
            if (string.IsNullOrEmpty(name))
            {
                await apiLogger.LogApiCall($"{action}/{name}", false, errorLog: $"{field} cannot be null or empty");

                return controller.BadRequest(new BaseResponse()
                {
                    Message = "Name cannot be null or empty",
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.BadRequest
                });
            }

            return null;
        }
    }

    public interface IApiLogger
    {
        Task LogApiCall(string action, bool success, string errorLog = null);
    }

    public class BaseResponse
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public int ResponseCode { get; set; }
    }
}