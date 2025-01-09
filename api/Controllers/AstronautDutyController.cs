using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using StargateAPI.Helpers;
using System.Net;

namespace StargateAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AstronautDutyController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly StarbaseApiCallLogger _apiLogger; 

        public AstronautDutyController(IMediator mediator, StarbaseApiCallLogger apiCallLogger)
        {
            _mediator = mediator;
            _apiLogger = apiCallLogger;
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetAstronautDutiesByName(string name)
        {
            var validationResult = await ValidationHelper.ValidateNameAsync(name, "AstronautDuty/GetAstronautDutiesByName", "Name", _apiLogger, this);
            if (validationResult != null)
            {
                return validationResult;
            }
            else
            {
                try
                {
                    var result = await _mediator.Send(new GetPersonByName()
                    {
                        Name = name
                    });

                    return this.GetResponse(result);
                }
                catch (Exception ex)
                {
                    return this.GetResponse(new BaseResponse()
                    {
                        Message = ex.Message,
                        Success = false,
                        ResponseCode = (int)HttpStatusCode.InternalServerError
                    });
                }   
            }         
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateAstronautDuty([FromBody] CreateAstronautDuty request)
        {
                var result = await _mediator.Send(request);
                return this.GetResponse(result);           
        }
    }
}