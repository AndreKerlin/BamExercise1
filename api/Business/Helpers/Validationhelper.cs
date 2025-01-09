using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace StargateAPI.Helpers
{
        public static class ValidationHelper
    {
        public static async Task<IActionResult> ValidateFieldsAsync(List<string> necessaryFieldsValues , string action, List<string> necessaryFields , StarbaseApiCallLogger apiLogger, ControllerBase controller)
        {
            int i = 0;
            string invalidFields = "";
            bool errorsExist = false;
            // loop through necessary fields and make note of any that are null or empty
            foreach (var fieldValue in necessaryFieldsValues)
            {
                if (string.IsNullOrEmpty(fieldValue))
                {
                    invalidFields += $"{necessaryFields[i]}, ";
                    errorsExist = true;
                }
                i++;
            }
            // if any fields are null or empty, log the error and return a bad request with the invalid fields
            if(errorsExist){
                if (invalidFields.EndsWith(", "))
                {
                    invalidFields = invalidFields.Substring(0, invalidFields.Length - 2);
                }
                await apiLogger.LogApiCall($"{action}", false, errorLog: $"{invalidFields} cannot be null or empty");

                return controller.BadRequest(new BaseResponse()
                    {
                        Message = invalidFields + $" cannot be null or empty",
                        Success = false,
                        ResponseCode = (int)HttpStatusCode.BadRequest
                    });
            }else{
                return null;
            }
        }
    }

    public class BaseResponse
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public int ResponseCode { get; set; }
    }
}