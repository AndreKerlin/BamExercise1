using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetAstronautDutiesByName : IRequest<GetAstronautDutiesByNameResult>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class GetAstronautDutiesByNameHandler : IRequestHandler<GetAstronautDutiesByName, GetAstronautDutiesByNameResult>
    {
        private readonly StargateContext _context;
        private readonly StarbaseApiCallLogger _apiLogger;
        public GetAstronautDutiesByNameHandler(StargateContext context, StarbaseApiCallLogger apiLogger)
        {
            _context = context;
            _apiLogger = apiLogger;
        }

        public async Task<GetAstronautDutiesByNameResult> Handle(GetAstronautDutiesByName request, CancellationToken cancellationToken)
        {
            try{
                var result = new GetAstronautDutiesByNameResult();

                PersonAstronaut? personRecord = await (from p in _context.People.AsNoTracking()
                                            join astronautDetail in _context.AstronautDetails.AsNoTracking()
                                            on p.Id equals astronautDetail.PersonId into personDetails
                                            from detail in personDetails.DefaultIfEmpty()
                                            where detail != null && p.Name == request.Name
                                            select new PersonAstronaut
                                            {
                                                PersonId = p.Id,
                                                Name = p.Name,
                                                CurrentRank = detail.CurrentRank,
                                                CurrentDutyTitle = detail.CurrentDutyTitle,
                                                CareerStartDate = detail.CareerStartDate,
                                                CareerEndDate = detail.CareerEndDate
                                            }).FirstOrDefaultAsync();
                if(personRecord is null)
                {
                    await _apiLogger.LogApiCall($"GetAstronautDutiesByName", true); // log success

                    return new GetAstronautDutiesByNameResult { Success = false, Message = "Astronaut not found" };
                }else{
                    result.Person = personRecord;

                    var duties = await (from ad in _context.AstronautDuties.AsNoTracking()
                                    where ad.PersonId == personRecord.PersonId
                                    orderby ad.DutyStartDate descending
                                    select ad).ToListAsync();
                    result.AstronautDuties = duties;

                    return result;
                }
                
            }
            catch(Exception ex)
            {
                await _apiLogger.LogApiCall($"GetAstronautDutiesByName", false, errorLog: ex.Message); // log error

                return new GetAstronautDutiesByNameResult { Success = false, Message = ex.Message };
            }

        }
    }

    public class GetAstronautDutiesByNameResult : BaseResponse
    {
        public PersonAstronaut Person { get; set; }
        public List<AstronautDuty> AstronautDuties { get; set; } = new List<AstronautDuty>();
    }
}
