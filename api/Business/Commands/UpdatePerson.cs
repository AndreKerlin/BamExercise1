using System.Net;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
    public class UpdatePerson : IRequest<UpdatePersonResult>
    {
        public required string Name { get; set; } = string.Empty;
        public required string NewName { get; set; } = string.Empty;
    }

    public class UpdatePersonPreProcessor : IRequestPreProcessor<UpdatePerson>
    {
        private readonly StargateContext _context;
        public UpdatePersonPreProcessor(StargateContext context)
        {
            _context = context;
        }
        public Task Process(UpdatePerson request, CancellationToken cancellationToken)
        {
            var person = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name);

            if (person is null) throw new BadHttpRequestException("Bad Request: Name does not exist");

            return Task.CompletedTask;
        }
    }
 
    public class UpdatePersonHandler : IRequestHandler<UpdatePerson, UpdatePersonResult>
    {
        private readonly StargateContext _context;
        private readonly StarbaseApiCallLogger _apiLogger;
        
        public UpdatePersonHandler(StargateContext context, StarbaseApiCallLogger apiLogger)
        {
            _context = context;
            _apiLogger = apiLogger;
        }
        public async Task<UpdatePersonResult> Handle(UpdatePerson request, CancellationToken cancellationToken)
        {
            var person = await _context.People.FirstOrDefaultAsync(z => z.Name == request.Name, cancellationToken);
            if(person is null){
                return new UpdatePersonResult()
                    {
                        Message = $"Person with name {request.Name} not found",
                        Success = false,
                        ResponseCode = (int)HttpStatusCode.InternalServerError
                    };            
                    }
            else{
                try
                {
                    person.Name = request.NewName;
                    await _context.SaveChangesAsync(cancellationToken);

                    await _apiLogger.LogApiCall($"UpdatePerson/{request.Name}", true, "Name", request.Name, request.NewName); // Log the successful call with changed field
                    return new UpdatePersonResult()
                    {
                        Name = person.Name
                    };
                }catch (Exception ex)
                {
                    await _apiLogger.LogApiCall($"UpdatePerson/{request.Name}", false, errorLog: ex.Message); // Log the error

                    return new UpdatePersonResult()
                    {
                        Message = ex.Message,
                        Success = false,
                        ResponseCode = (int)HttpStatusCode.InternalServerError
                    };
                } 
            }
        }

    }

    public class UpdatePersonResult : BaseResponse
    {
        public string? Name{ get; set; }
    }
}
