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
            var validationResult = await ValidationHelper.ValidateFieldsAsync([name], "AstronautDuty/GetAstronautDutiesByName", ["Name"], _apiLogger, this);
            if (validationResult != null)
            {
                return validationResult;
            }
            else
            {
                var result = await _mediator.Send(new GetAstronautDutiesByName()
                {
                    Name = name
                });

                return this.GetResponse(result);
            }         
        }

        [HttpPost]
        public async Task<IActionResult> CreateAstronautDuty(CreateAstronautDuty command)
        {
            var validationResult = await ValidationHelper.ValidateFieldsAsync([command.Name, command.Rank,command.DutyTitle], "AstronautDuty/GetAstronautDutiesByName", ["Name","Rank","DutyTitle"], _apiLogger, this);
            if (validationResult != null)
            {
                return validationResult;
            }
            else
            {
                var result = await _mediator.Send(command);

                return Ok(result); 
            }
            
        }
    }
}