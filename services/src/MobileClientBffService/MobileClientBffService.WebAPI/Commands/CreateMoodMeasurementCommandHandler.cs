using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood.Dto;

namespace Telemedicine.Services.MobileClientBffService.WebAPI.Commands
{
    public class CreateMoodMeasurementCommandHandler : ICommandHandler<CreateMoodMeasurementCommand, Guid>
    {
        private readonly IMapper _mapper;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IMoodMeasurementCommandService _moodMeasurementCommandService;

        public CreateMoodMeasurementCommandHandler(IMapper mapper, ICurrentUserProvider currentUserProvider, IMoodMeasurementCommandService moodMeasurementCommandService)
        {
            _mapper = mapper;
            _currentUserProvider = currentUserProvider;
            _moodMeasurementCommandService = moodMeasurementCommandService;
        }

        public Task<Guid> HandleAsync(CreateMoodMeasurementCommand command, CancellationToken cancellationToken = default)
        {
            var request = _mapper.Map<CreateMeasurementRequestDto<MoodMeasurementDto>>(command) with { PatientId = _currentUserProvider.GetId() };
            return _moodMeasurementCommandService.CreateAsync(request, cancellationToken);
        }
    }
}
