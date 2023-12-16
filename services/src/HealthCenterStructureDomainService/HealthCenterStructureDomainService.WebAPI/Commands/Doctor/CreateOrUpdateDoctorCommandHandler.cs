using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Repositories;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Specifications;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Commands.Doctor
{
    public class CreateOrUpdateDoctorCommandHandler : ICommandHandler<CreateOrUpdateDoctorCommand, Guid?>
    {
        private readonly IMapper _mapper;
        private readonly IDoctorReadRepository _doctorReadRepository;
        private readonly IDoctorWriteRepository _doctorWriteRepository;
        private readonly IHealthCenterReadRepository _healthCenterReadRepository;

        private readonly ILogger _logger = Log.ForContext<CreateOrUpdateDoctorCommandHandler>();

        public CreateOrUpdateDoctorCommandHandler(IMapper mapper, IDoctorReadRepository doctorReadRepository,
            IDoctorWriteRepository doctorWriteRepository,
            IHealthCenterReadRepository healthCenterReadRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _doctorReadRepository = doctorReadRepository ?? throw new ArgumentNullException(nameof(doctorReadRepository));
            _doctorWriteRepository = doctorWriteRepository ?? throw new ArgumentNullException(nameof(doctorWriteRepository));
            _healthCenterReadRepository = healthCenterReadRepository ?? throw new ArgumentNullException(nameof(healthCenterReadRepository));
        }

        /// <inheritdoc />
        public async Task<Guid?> HandleAsync(CreateOrUpdateDoctorCommand command, CancellationToken cancellationToken = default)
        {
            var existingDoctor = await _doctorReadRepository.FindWithDeleted(new ByInnerIdSpecification<Core.Entities.Doctor>(command.InnerId))
                .AsTracking().Include(doctor => doctor.HealthCenters).SingleOrDefaultAsync(cancellationToken);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (existingDoctor is not null)
            {
                var updateCommand = _mapper.Map<UpdateDoctorCommand>(command);
                return await UpdateExistingDoctorAsync(updateCommand, command.HealthCenterInnerIds, existingDoctor, cancellationToken);
            }

            var createCommand = _mapper.Map<CreateDoctorCommand>(command);
            return await CreateNewDoctorAsync(createCommand, command.HealthCenterInnerIds, cancellationToken);
        }

        /// <summary>
        /// Creates a new Doctor
        /// </summary>
        /// <returns>Id of created entity</returns>
        private async Task<Guid?> CreateNewDoctorAsync(CreateDoctorCommand command, string[] healthCenters, CancellationToken cancellationToken = default)
        {
            if (command.IsDeleted)
            {
                _logger.Information("We will not create a deleted user. Just skipping the action.");
                return null;
            }

            var doctorToCreate = _mapper.Map<Core.Entities.Doctor>(command);
            await UpdateDoctorHealthCenters(doctorToCreate, healthCenters, cancellationToken);
            Core.Entities.Doctor createdDoctor = await _doctorWriteRepository.AddAsync(doctorToCreate, cancellationToken);

            _logger.Information("Entity 'Doctor' with Id='{Id}' was created", createdDoctor.Id);
            return createdDoctor.Id;
        }

        /// <summary>
        /// Updates an existing Doctor
        /// </summary>
        /// <returns>Id of updated entity</returns>
        private async Task<Guid?> UpdateExistingDoctorAsync(UpdateDoctorCommand command, string[] healthCenters, Core.Entities.Doctor existingDoctor, CancellationToken cancellationToken = default)
        {
            if (!existingDoctor.IsDeleted && command.IsDeleted)
            {
                _logger.Warning("We cannot delete an existing doctor. The functionality is not implemented");
                return null;
            }

            Core.Entities.Doctor doctorToUpdate = _mapper.Map(command, existingDoctor);
            await UpdateDoctorHealthCenters(doctorToUpdate, healthCenters, cancellationToken);
            Core.Entities.Doctor updatedDoctor = await _doctorWriteRepository.UpdateAsync(doctorToUpdate, cancellationToken);

            _logger.Information("Entity 'Doctor' with Id='{Id}' was updated", updatedDoctor.Id);
            return updatedDoctor.Id;
        }

        /// <summary>
        /// Remove old health centers if it doesn't exist in new collection and add new ones for doctor
        /// </summary>
        private async Task UpdateDoctorHealthCenters(Core.Entities.Doctor doctor, string[] healthCenters, CancellationToken cancellationToken)
        {
            var healthCenterList = new List<Core.Entities.HealthCenter>();

            foreach (var healthCenterInnerId in healthCenters)
            {
                var healthCenter = await _healthCenterReadRepository
                    .Find(new ByInnerIdSpecification<Core.Entities.HealthCenter>(healthCenterInnerId))
                    .AsTracking()
                    .SingleOrDefaultAsync(cancellationToken);

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (healthCenter is null)
                {
                    _logger.Warning("Not found active health center with specified InnerId='{InnerId}'", healthCenterInnerId);
                    continue;
                }

                healthCenterList.Add(healthCenter);
            }

            doctor.SetHealthCenters(healthCenterList);
        }
    }
}
