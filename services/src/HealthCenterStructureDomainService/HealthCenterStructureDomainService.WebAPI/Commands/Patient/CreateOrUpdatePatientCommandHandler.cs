using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Serilog;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.Common.Exceptions;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Repositories;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Specifications;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Commands.Patient
{
    public class CreateOrUpdatePatientCommandHandler : ICommandHandler<CreateOrUpdatePatientCommand, Guid?>
    {
        private readonly IMapper _mapper;
        private readonly IPatientReadRepository _patientReadRepository;
        private readonly IPatientWriteRepository _patientWriteRepository;
        private readonly IDoctorReadRepository _doctorReadRepository;
        private readonly IHealthCenterReadRepository _healthCenterReadRepository;
        private readonly ILogger _logger = Log.ForContext<CreateOrUpdatePatientCommandHandler>();

        public CreateOrUpdatePatientCommandHandler(IMapper mapper, IPatientReadRepository patientReadRepository, IPatientWriteRepository patientWriteRepository,
            IDoctorReadRepository doctorReadRepository, IHealthCenterReadRepository healthCenterReadRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _patientReadRepository = patientReadRepository ?? throw new ArgumentNullException(nameof(patientReadRepository));
            _patientWriteRepository = patientWriteRepository ?? throw new ArgumentNullException(nameof(patientWriteRepository));
            _doctorReadRepository = doctorReadRepository ?? throw new ArgumentNullException(nameof(doctorReadRepository));
            _healthCenterReadRepository = healthCenterReadRepository ?? throw new ArgumentNullException(nameof(healthCenterReadRepository));
        }

        /// <inheritdoc />
        public async Task<Guid?> HandleAsync(CreateOrUpdatePatientCommand command, CancellationToken cancellationToken = default)
        {
            Core.Entities.Patient? existingPatient = await _patientReadRepository.SingleOrDefaultWithDeletedAsync(new ByInnerIdSpecification<Core.Entities.Patient>(command.InnerId), cancellationToken);
            Core.Entities.HealthCenter assignedHealthCenter = await GetAssignedHealthCenter(command.HealthCenterInnerId, cancellationToken);

            Guid? assignedDoctorId = await GetDoctorIdByInnerIdAsync(command.PrimaryCareProviderInnerId, cancellationToken);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (existingPatient is not null)
            {
                var updateCommand = _mapper.Map<UpdatePatientCommand>(command) with { HealthCenterId = assignedHealthCenter.Id, PrimaryCareProviderId = assignedDoctorId };
                return await UpdateExistingPatientAsync(updateCommand, existingPatient, cancellationToken);
            }

            var createCommand = _mapper.Map<CreatePatientCommand>(command) with { HealthCenterId = assignedHealthCenter.Id, PrimaryCareProviderId = assignedDoctorId };
            return await CreateNewPatientAsync(createCommand, cancellationToken);
        }

        /// <summary>
        /// Creates a new Patient
        /// </summary>
        /// <returns>Id of created entity</returns>
        private async Task<Guid> CreateNewPatientAsync(CreatePatientCommand command, CancellationToken cancellationToken = default)
        {
            var patientToCreate = _mapper.Map<Core.Entities.Patient>(command);
            Core.Entities.Patient createdPatient = await _patientWriteRepository.AddAsync(patientToCreate, cancellationToken);

            _logger.Information("Entity 'Patient' with Id='{Id}' was created", createdPatient.Id);
            return createdPatient.Id;
        }

        /// <summary>
        /// Updates an existing Patient
        /// </summary>
        /// <returns>Id of updated entity</returns>
        private async Task<Guid?> UpdateExistingPatientAsync(UpdatePatientCommand command, Core.Entities.Patient existingPatient, CancellationToken cancellationToken = default)
        {
            if (!existingPatient.IsDeleted && command.IsDeleted)
            {
                _logger.Warning("We can not delete an existing patient. The functionality is not implemented");
                return null;
            }

            var patientToUpdate = _mapper.Map(command, existingPatient);
            Core.Entities.Patient updatedPatient = await _patientWriteRepository.UpdateAsync(patientToUpdate, cancellationToken);

            _logger.Information("Entity 'Patient' with Id='{Id}' was updated", updatedPatient.Id);
            return updatedPatient.Id;
        }

        private async Task<Guid?> GetDoctorIdByInnerIdAsync(string? innerId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(innerId))
            {
                return null;
            }

            Core.Entities.Doctor? assignedDoctor = await _doctorReadRepository.SingleOrDefaultAsync(new ByInnerIdSpecification<Core.Entities.Doctor>(innerId), cancellationToken);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (assignedDoctor is null)
            { _logger.Warning("Not found active doctor with specified InnerId='{InnerId}'", innerId); }

            return assignedDoctor?.Id;
        }

        private async Task<Core.Entities.HealthCenter> GetAssignedHealthCenter(string healthCenterInnerId, CancellationToken cancellationToken = default)
        {
            Core.Entities.HealthCenter? assignedHealthCenter = await _healthCenterReadRepository.SingleOrDefaultAsync(new ByInnerIdSpecification<Core.Entities.HealthCenter>(healthCenterInnerId), cancellationToken);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (assignedHealthCenter is null)
            {
                _logger.Error("Not found active health center with specified InnerId='{InnerId}'", healthCenterInnerId);
                throw new EntityNotFoundByInnerIdException(typeof(Core.Entities.HealthCenter), healthCenterInnerId);
            }

            return assignedHealthCenter;
        }
    }

}
