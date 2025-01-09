using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
    public class CreatePerson : IRequest<CreatePersonResult>
    {
        public required string Name { get; set; } = string.Empty;
    }

    public class CreatePersonPreProcessor : IRequestPreProcessor<CreatePerson>
    {
        private readonly StargateContext _context;
        private readonly ILogger<CreatePersonPreProcessor> _logger;
        public CreatePersonPreProcessor(StargateContext context, ILogger<CreatePersonPreProcessor> logger)
        {
            _context = context;
            _logger = logger;
        }
        public Task Process(CreatePerson request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public class CreatePersonHandler : IRequestHandler<CreatePerson, CreatePersonResult>
    {
        private readonly StargateContext _context;
        private readonly StarbaseApiCallLogger _apiLogger;

         public CreatePersonHandler(StargateContext context, StarbaseApiCallLogger apiLogger)
        {
            _context = context;
            _apiLogger = apiLogger;
        }

        public async Task<CreatePersonResult> Handle(CreatePerson request, CancellationToken cancellationToken)
        {
            var person = await _context.People.FirstOrDefaultAsync(z => z.Name == request.Name, cancellationToken);
            if(person is not null){
                return new CreatePersonResult { Success = false, Message = $"An Astronaut with the name {request.Name} already exists" };            
                }
            else{
                try
                {
                    var newPerson = new Person()
                    {
                        Name = request.Name
                    };

                    await _context.People.AddAsync(newPerson, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);

                    await _apiLogger.LogApiCall($"CreatePerson/{request.Name}", true); 

                    return new CreatePersonResult()
                    {
                        Id = newPerson.Id,
                        Name = newPerson.Name
                    };
                }
                catch (Exception ex)
                {
                    await _apiLogger.LogApiCall($"CreatePerson/{request.Name}", false, errorLog: ex.Message); 

                    throw new ApplicationException("An error occurred while creating the person. Please try again later.");
                }
            }
        }
    }

    public class CreatePersonResult : BaseResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
