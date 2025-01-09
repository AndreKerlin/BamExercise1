using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using System.Net;
using StargateAPI.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace StargateAPI.Controllers
{
   
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly StarbaseApiCallLogger _apiLogger;
        public PersonController(IMediator mediator, StarbaseApiCallLogger apiCallLogger)
        {
            _mediator = mediator;
            _apiLogger = apiCallLogger;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetPeople()
        {
                var result = await _mediator.Send(new GetPeople()
                {

                });

                return this.GetResponse(result);
            
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetPersonByName(string name)
        {
            var validationResult = await ValidationHelper.ValidateFieldsAsync([name], "GetPersonByName", ["Name"], _apiLogger, this);
            if (validationResult != null)
            {
                return validationResult; // essential Paramater was null
            }
            else{
                var result = await _mediator.Send(new GetPersonByName()
                {
                    Name = name
                });

                return this.GetResponse(result);
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> CreatePerson(CreatePerson command)
        {
            var validationResult = await ValidationHelper.ValidateFieldsAsync([command.Name], "CreatePerson", ["Name"], _apiLogger, this);
            if (validationResult != null)
            {
                return validationResult; // essential Paramater was null
            }
            else
            {
                var result = await _mediator.Send(command);

                return Ok(result);
            }
            
        }

        [HttpPut("")]
        public async Task<IActionResult> UpdatePerson(UpdatePerson command)
        {
            var validationResult = await ValidationHelper.ValidateFieldsAsync([command.Name, command.NewName], "UpdatePerson", ["Name", "NewName"], _apiLogger, this);
            if (validationResult != null)
            {
                return validationResult; // essential Paramater was null
            }
            else
            {
                var result = await _mediator.Send(command);
                
                return this.GetResponse(result);
            }
            
        }
    }
}