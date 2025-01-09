using Dapper;
using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;
using System.Net;

namespace StargateAPI.Business.Commands
{
    public class CreateAstronautDuty : IRequest<CreateAstronautDutyResult>
    {
        public required string Name { get; set; }

        public required string Rank { get; set; }

        public required string DutyTitle { get; set; }

        public DateTime DutyStartDate { get; set; }
    }

    public class CreateAstronautDutyPreProcessor : IRequestPreProcessor<CreateAstronautDuty>
    {
        private readonly StargateContext _context;
        private readonly StarbaseApiCallLogger _apiLogger;


        public CreateAstronautDutyPreProcessor(StargateContext context, StarbaseApiCallLogger apiLogger)
        {
            _context = context;
            _apiLogger = apiLogger;
        }

        public Task Process(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public class CreateAstronautDutyHandler : IRequestHandler<CreateAstronautDuty, CreateAstronautDutyResult>
    {
        private readonly StargateContext _context;
        private readonly StarbaseApiCallLogger _apiLogger;
        public CreateAstronautDutyHandler(StargateContext context, StarbaseApiCallLogger apiLogger)
        {
            _context = context;
            _apiLogger = apiLogger;
        }
        public async Task<CreateAstronautDutyResult> Handle(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            try{
                var person = await _context.People.AsNoTracking().FirstOrDefaultAsync(z => z.Name == request.Name, cancellationToken);
                if(person is null)
                {
                    throw new BadHttpRequestException("Bad Request: Name does not exist");
                }
                else
                {
                    var verifyNoPreviousDuty = _context.AstronautDuties.AsNoTracking().FirstOrDefault(z => z.DutyTitle == request.DutyTitle && z.DutyStartDate == request.DutyStartDate && z.PersonId ==person.Id);
                    if(verifyNoPreviousDuty is not null)
                    {
                        throw new BadHttpRequestException("Bad Request:  that duty is already assigned to this person");
                    }else{
                        var astronautDetail = await _context.AstronautDetails.AsNoTracking().FirstOrDefaultAsync(z => z.PersonId == person.Id, cancellationToken);
                        
                        if(astronautDetail is null){
                            astronautDetail = new AstronautDetail();
                            astronautDetail.PersonId = person.Id;
                            astronautDetail.CurrentDutyTitle = request.DutyTitle;
                            astronautDetail.CurrentRank = request.Rank;
                            astronautDetail.CareerStartDate = request.DutyStartDate.Date;

                            if (string.Equals(request.DutyTitle, "RETIRED", StringComparison.OrdinalIgnoreCase))
                            {
                                astronautDetail.CareerEndDate = request.DutyStartDate.Date;
                            }

                            await _context.AstronautDetails.AddAsync(astronautDetail, cancellationToken);

                            }else{
                                astronautDetail.CurrentDutyTitle = request.DutyTitle;
                                astronautDetail.CurrentRank = request.Rank;
                                if (string.Equals(request.DutyTitle, "RETIRED", StringComparison.OrdinalIgnoreCase))
                                {
                                    astronautDetail.CareerEndDate = request.DutyStartDate.AddDays(-1).Date;
                                }
                                _context.AstronautDetails.Update(astronautDetail);
                            }

                            var astronautDuty = await _context.AstronautDuties.AsNoTracking().FirstOrDefaultAsync(z => z.PersonId == person.Id && z.CurrentDuty == true, cancellationToken);

                            if (astronautDuty != null)
                            {
                                astronautDuty.DutyEndDate = request.DutyStartDate.AddDays(-1).Date;
                                astronautDuty.CurrentDuty = false;
                                _context.AstronautDuties.Update(astronautDuty);
                            }

                            var newAstronautDuty = new AstronautDuty()
                            {
                                PersonId = person.Id,
                                Rank = request.Rank,
                                DutyTitle = request.DutyTitle,
                                DutyStartDate = request.DutyStartDate.Date,
                                DutyEndDate = null,
                                CurrentDuty = true
                            };

                            await _context.AstronautDuties.AddAsync(newAstronautDuty);

                            await _context.SaveChangesAsync();
                            await _apiLogger.LogApiCall($"CreateAstronautDuty", true); // Log the successful call with changed field

                            return new CreateAstronautDutyResult()
                            {
                                Id = newAstronautDuty.Id
                            };
                    }
                }
            }catch (Exception ex)
            {
                await _apiLogger.LogApiCall($"CreateAstronautDuty", false, errorLog: ex.Message); // Log the error

                return new CreateAstronautDutyResult()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                };
            }
            
        }
    }

    public class CreateAstronautDutyResult : BaseResponse
    {
        public int? Id { get; set; }
    }
}
