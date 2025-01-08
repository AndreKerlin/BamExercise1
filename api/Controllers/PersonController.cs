using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using System.Net;

namespace StargateAPI.Controllers
{
   
    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly StarbaseApiCallLogger _logger; 
        public PersonController(IMediator mediator, StarbaseApiCallLogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetPeople()
        {
            try
            {
                var result = await _mediator.Send(new GetPeople()
                {

                });
                await _logger.LogApiCall("GetPeople", true);

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                await _logger.LogApiCall("GetPeople", false, errorLog: ex.Message);
                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetPersonByName(string name)
        {
            if(string.IsNullOrEmpty(name))
            {
                  await _logger.LogApiCall($"GetPersonByName/{name}", true, errorLog: "Name cannot be null or empty");

                return this.GetResponse(new BaseResponse()
                {
                    Message = "Name cannot be null or empty",
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.BadRequest
                });
            }else{
                try
                {
                    var result = await _mediator.Send(new GetPersonByName()
                    {
                        Name = name
                    });
                    await _logger.LogApiCall($"GetPersonByName/{name}", true);
                    return this.GetResponse(result);
                }
                catch (Exception ex)
                {
                    await _logger.LogApiCall($"GetPersonByName/{name}", true, errorLog: ex.Message);
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
        public async Task<IActionResult> CreatePerson([FromBody] string name)
        {
            if(string.IsNullOrEmpty(name))
            {
                 await _logger.LogApiCall($"CreatePerson/{name}", false, errorLog: "Name cannot be null or empty");

                return this.GetResponse(new BaseResponse()
                {
                    Message = "Name cannot be null or empty",
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.BadRequest
                });
            }else{
                try
                {
                    var result = await _mediator.Send(new CreatePerson()
                    {
                        Name = name
                    });
                    await _logger.LogApiCall($"CreatePerson/{name}", true);
                    return this.GetResponse(result);
                }
                catch (Exception ex)
                {
                    await _logger.LogApiCall($"CreatePerson/{name}", false, errorLog: ex.Message);
                    return this.GetResponse(new BaseResponse()
                    {
                        Message = ex.Message,
                        Success = false,
                        ResponseCode = (int)HttpStatusCode.InternalServerError
                    });
                }

            }
            
        }


        [HttpPut("")]
        public async Task<IActionResult> UpdatePerson([FromBody] string name, string newName)
        {
            if(string.IsNullOrEmpty(name))
            {
                 await _logger.LogApiCall($"UpdatePerson/{name}", false, errorLog: "Name cannot be null or empty");

                return this.GetResponse(new BaseResponse()
                {
                    Message = "Name cannot be null or empty",
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.BadRequest
                });
            }else{
                try
                {
                    var result = await _mediator.Send(new CreatePerson()
                    {
                        Name = name
                    });
                    await _logger.LogApiCall($"UpdatePerson/{name}", true);
                    return this.GetResponse(result);
                }
                catch (Exception ex)
                {
                    await _logger.LogApiCall($"UpdatePerson/{name}", false, errorLog: ex.Message);
                    return this.GetResponse(new BaseResponse()
                    {
                        Message = ex.Message,
                        Success = false,
                        ResponseCode = (int)HttpStatusCode.InternalServerError
                    });
                }

            }
            
        }
    }
}