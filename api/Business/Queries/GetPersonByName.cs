using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetPersonByName : IRequest<GetPersonByNameResult>
    {
        public required string Name { get; set; } = string.Empty;
    }

    public class GetPersonByNameHandler : IRequestHandler<GetPersonByName, GetPersonByNameResult>
    {
        private readonly StargateContext _context;
        private readonly StarbaseApiCallLogger _apiLogger;
        public GetPersonByNameHandler(StargateContext context, StarbaseApiCallLogger apiLogger)
        {
            _context = context;
            _apiLogger = apiLogger;
        }

        public async Task<GetPersonByNameResult> Handle(GetPersonByName request, CancellationToken cancellationToken)
        {
            try{
                // only grab a persons record if they have astronaut details associated with them
                var PersonRecord = (from persons in _context.People.AsNoTracking()
                        join astronautDetail in _context.AstronautDetails.AsNoTracking()
                        on persons.Id equals astronautDetail.PersonId into personDetails
                        from detail in personDetails.DefaultIfEmpty()
                        where persons.Name == request.Name && detail != null
                        select new PersonAstronaut
                        {
                            PersonId = persons.Id,
                            Name = persons.Name,
                            CurrentRank = detail.CurrentRank,
                            CurrentDutyTitle = detail.CurrentDutyTitle,
                            CareerStartDate = detail.CareerStartDate,
                            CareerEndDate = detail.CareerEndDate
                        }).FirstOrDefault(); 

                if(PersonRecord is not null){
                    var personResult = new GetPersonByNameResult { Person = PersonRecord };
                    
                    await _apiLogger.LogApiCall($"GetPersonByName/{request.Name}", true); // log success

                    return personResult;

                }
                else{
                    await _apiLogger.LogApiCall($"GetPersonByName/{request.Name}", true); // log success

                    return new GetPersonByNameResult { Person = null, Message = $"User with name {request.Name} not found" };

                }
            }catch(Exception ex){
                await _apiLogger.LogApiCall($"GetPersonByName/{request.Name}", false, errorLog: ex.Message); // log error
                return new GetPersonByNameResult { Person = null };
            }
        }
    }

    public class GetPersonByNameResult : BaseResponse
    {
        public PersonAstronaut? Person { get; set; }
    }
}
