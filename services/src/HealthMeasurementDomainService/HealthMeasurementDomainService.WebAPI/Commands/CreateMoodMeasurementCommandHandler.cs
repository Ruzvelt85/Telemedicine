using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Business.BusinessLogic;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Exceptions;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common.Mood.Dto;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Repositories;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Specifications;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Commands
{
    public class CreateMoodMeasurementCommandHandler : ICommandHandler<CreateMeasurementCommand<MoodMeasurementDto>, Guid>
    {
        private readonly IMapper _mapper;
        private readonly IMoodMeasurementReadRepository _moodMeasurementReadRepository;
        private readonly IMoodMeasurementWriteRepository _moodMeasurementWriteRepository;
        private readonly ITimeZoneProvider _timeZoneProvider;

        public CreateMoodMeasurementCommandHandler(IMapper mapper, IMoodMeasurementReadRepository moodMeasurementReadRepository,
            IMoodMeasurementWriteRepository moodMeasurementWriteRepository, ITimeZoneProvider timeZoneProvider)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _moodMeasurementReadRepository = moodMeasurementReadRepository ?? throw new ArgumentNullException(nameof(moodMeasurementReadRepository));
            _moodMeasurementWriteRepository = moodMeasurementWriteRepository ?? throw new ArgumentNullException(nameof(moodMeasurementWriteRepository));
            _timeZoneProvider = timeZoneProvider ?? throw new ArgumentNullException(nameof(timeZoneProvider));
        }

        /// <inheritdoc />
        public async Task<Guid> HandleAsync(CreateMeasurementCommand<MoodMeasurementDto> command, CancellationToken cancellationToken = default)
        {
            var specification = await GetSpecification(command);
            var todaysMood = await _moodMeasurementReadRepository.FirstOrDefaultAsync(specification, cancellationToken);
            if (todaysMood is not null)
            {
                throw new MoodAlreadyCreatedTodayException(todaysMood.PatientId, todaysMood.ClientDate);
            }

            var moodMeasurement = _mapper.Map<MoodMeasurement>(command);
            MoodMeasurement result = await _moodMeasurementWriteRepository.AddAsync(moodMeasurement, cancellationToken);
            return result.Id;
        }

        private async Task<Specification<MoodMeasurement>> GetSpecification(CreateMeasurementCommand<MoodMeasurementDto> command)
        {
            var patientTimeZone = await _timeZoneProvider.GetPatientTimeOffset(command.PatientId);
            var patientIdSpecification = new PatientIdSpec(command.PatientId).As<MoodMeasurement>();
            var clientDateSpecification = new SpecificDayClientDateWithTimeZoneSpec(command.ClientDate, patientTimeZone).As<MoodMeasurement>();

            return patientIdSpecification & clientDateSpecification;
        }
    }
}
