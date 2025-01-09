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
        private readonly ILogger<PersonController> _logger;
        public PersonController(IMediator mediator, StarbaseApiCallLogger apiCallLogger,ILogger<PersonController> logger)
        {
            _mediator = mediator;
            _apiLogger = apiCallLogger;
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
                await _apiLogger.LogApiCall("GetPeople", true);

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                await _apiLogger.LogApiCall("GetPeople", false, errorLog: ex.Message);
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
                  await _apiLogger.LogApiCall($"GetPersonByName/{name}", true, errorLog: "Name cannot be null or empty");

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
                    await _apiLogger.LogApiCall($"GetPersonByName/{name}", true);
                    return this.GetResponse(result);
                }
                catch (Exception ex)
                {
                    await _apiLogger.LogApiCall($"GetPersonByName/{name}", true, errorLog: ex.Message);
                    return this.GetResponse(new BaseResponse()
                    {
                        Message = ex.Message,
                        Success = false,
                        ResponseCode = (int)HttpStatusCode.InternalServerError
                    });
                }
            }
            
        }

        // [HttpPost("")]
        // public async Task<IActionResult> CreatePerson([FromBody] CreatePerson request)
        // {
        //         try
        //         {
        //             var result = await _mediator.Send(request);
        //         return this.GetResponse(result);    
        //         }
        //         catch (Exception ex)
        //         {
                    
        //             return this.GetResponse(new BaseResponse()
        //             {
        //                 Message = ex.Message,
        //                 Success = false,
        //                 ResponseCode = (int)HttpStatusCode.InternalServerError
        //             });
        //         }

            
            
        // }

        [HttpPost]
        public async Task<IActionResult> CreatePerson(CreatePerson command)
        {
            var validationResult = await ValidationHelper.ValidateNameAsync(command.Name, "UpdatePerson", "Name", _apiLogger, this);
            if (validationResult != null)
            {
                return validationResult;
            }
            else
            {
                try
                {
                    var result = await _mediator.Send(command);
                    await _apiLogger.LogApiCall($"CreatePerson/{command.Name}", true);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    await _apiLogger.LogApiCall($"CreatePerson/{command.Name}", false, errorLog: ex.Message);
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
        public async Task<IActionResult> UpdatePerson(UpdatePerson command)
        {
            if(string.IsNullOrEmpty(command.Name))
            {
                 await _apiLogger.LogApiCall($"UpdatePerson/{command.Name}", false, errorLog: "Name cannot be null or empty");

                return this.GetResponse(new BaseResponse()
                {
                    Message = "Name cannot be null or empty",
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.BadRequest
                });
            }else{
                try
                {
                    var result = await _mediator.Send(command);
                    await _apiLogger.LogApiCall($"UpdatePerson/{command.Name}", true, "Name", command.Name, command.NewName);
                    return this.GetResponse(result);
                }
                catch (Exception ex)
                {
                    await _apiLogger.LogApiCall($"UpdatePerson/{command.Name}", false, errorLog: ex.Message);
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