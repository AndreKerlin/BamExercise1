using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Queries;

namespace StargateAPI.Controllers
{
    public static class ControllerBaseExtensions
    {

        public static IActionResult GetResponse(this ControllerBase controllerBase, BaseResponse response)
        {
            if (response.ResponseCode == 200)
            {
                if (response is GetPeopleResult getPeopleResult)
                {
                    return new OkObjectResult(getPeopleResult.People);
                }
                return new OkObjectResult(response);
            }
            var httpResponse = new ObjectResult(response);
            httpResponse.StatusCode = response.ResponseCode;
            return httpResponse;
        }
    }
}