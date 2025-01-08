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

        public UpdatePersonHandler(StargateContext context)
        {
            _context = context;
        }
        public async Task<UpdatePersonResult> Handle(UpdatePerson request, CancellationToken cancellationToken)
        {

            var person = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name);
            if(person is null){
                throw new BadHttpRequestException("Bad Request: Name does not exist");
            }
            else{
                try
                {
                    person.Name = request.NewName;
                    await _context.SaveChangesAsync();

                    return new UpdatePersonResult()
                    {
                        Name = person.Name
                    };
                }catch (Exception ex)
                {
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
