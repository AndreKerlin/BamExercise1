using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;
using static StargateAPI.Business.Queries.GetPeopleHandler;

namespace StargateAPI.Business.Queries
{
    public class GetPeople : IRequest<GetPeopleResult>
    {

    }

    public class GetPeopleHandler : IRequestHandler<GetPeople, GetPeopleResult>
    {
        public readonly StargateContext _context;
        private readonly StarbaseApiCallLogger _apiLogger;

        public GetPeopleHandler(StargateContext context, StarbaseApiCallLogger apiLogger)
        {
            _context = context;
            _apiLogger = apiLogger;
        }
        public async Task<GetPeopleResult> Handle(GetPeople request, CancellationToken cancellationToken)
        {
            try{
                // get a list of all astronauts, filter out persons without astronaut details
                List<PersonAstronaut> AllPersons = (from person in _context.People.AsNoTracking()
                        join astronautDetail in _context.AstronautDetails.AsNoTracking()
                        on person.Id equals astronautDetail.PersonId into personDetails
                        from detail in personDetails.DefaultIfEmpty()
                        where detail != null
                        select new PersonAstronaut
                        {
                            PersonId = person.Id,
                            Name = person.Name,
                            CurrentRank = detail.CurrentRank,
                            CurrentDutyTitle = detail.CurrentDutyTitle,
                            CareerStartDate = detail.CareerStartDate,
                            CareerEndDate = detail.CareerEndDate
                        }).ToList();

                if(AllPersons is not null){
                    var personResult = new GetPeopleResult { People = AllPersons };
                    
                    await _apiLogger.LogApiCall($"GetPeople", true); // log success

                    return personResult;

                }
                else{
                    await _apiLogger.LogApiCall($"GetPeople", true); // log success

                    return new GetPeopleResult { People = null, Message = $"Astronauts not found" };

                }
            }catch(Exception ex){
                await _apiLogger.LogApiCall($"GetPeople", false, errorLog: ex.Message); // log error
                
                return new GetPeopleResult { People = null };
            }
        }
    }

    public class GetPeopleResult : BaseResponse
    {
        public List<PersonAstronaut> People { get; set; } = new List<PersonAstronaut> { };

    }
}
