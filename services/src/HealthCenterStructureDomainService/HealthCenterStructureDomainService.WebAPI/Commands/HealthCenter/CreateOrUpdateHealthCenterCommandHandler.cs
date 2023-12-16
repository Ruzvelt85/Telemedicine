using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Repositories;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Specifications;
using Serilog;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Commands.HealthCenter
{
    public class CreateOrUpdateHealthCenterCommandHandler : ICommandHandler<CreateOrUpdateHealthCenterCommand, Guid?>
    {
        private readonly IMapper _mapper;
        private readonly IHealthCenterReadRepository _healthCenterReadRepository;
        private readonly IHealthCenterWriteRepository _healthCenterWriteRepository;
        private readonly ILogger _logger = Log.ForContext<CreateOrUpdateHealthCenterCommandHandler>();

        public CreateOrUpdateHealthCenterCommandHandler(IMapper mapper, IHealthCenterReadRepository healthCenterReadRepository, IHealthCenterWriteRepository healthCenterWriteRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _healthCenterReadRepository = healthCenterReadRepository ?? throw new ArgumentNullException(nameof(healthCenterReadRepository));
            _healthCenterWriteRepository = healthCenterWriteRepository ?? throw new ArgumentNullException(nameof(healthCenterWriteRepository));
        }

        /// <inheritdoc />
        public async Task<Guid?> HandleAsync(CreateOrUpdateHealthCenterCommand command, CancellationToken cancellationToken = default)
        {
            Core.Entities.HealthCenter? existingHealthCenter = await _healthCenterReadRepository.SingleOrDefaultWithDeletedAsync(new ByInnerIdSpecification<Core.Entities.HealthCenter>(command.InnerId), cancellationToken);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (existingHealthCenter is not null)
            {
                return await UpdateExistingEntityAsync(_mapper.Map<UpdateHealthCenterCommand>(command), existingHealthCenter, cancellationToken);
            }

            return await CreateNewEntityAsync(_mapper.Map<CreateHealthCenterCommand>(command), cancellationToken);
        }

        /// <summary>
        /// Creates a new Health Center
        /// </summary>
        /// <returns>Id of created entity</returns>
        private async Task<Guid> CreateNewEntityAsync(CreateHealthCenterCommand command, CancellationToken cancellationToken = default)
        {
            var healthCenterToCreate = _mapper.Map<Core.Entities.HealthCenter>(command);
            Core.Entities.HealthCenter createdHealthCenter = await _healthCenterWriteRepository.AddAsync(healthCenterToCreate, cancellationToken);

            _logger.Information("Entity HealthCenter with Id='{Id}' was created", createdHealthCenter.Id);
            return createdHealthCenter.Id;
        }

        /// <summary>
        /// Updates an existing Health Center
        /// </summary>
        /// <returns>Id of updated entity</returns>
        private async Task<Guid?> UpdateExistingEntityAsync(UpdateHealthCenterCommand command, Core.Entities.HealthCenter existingHealthCenter, CancellationToken cancellationToken = default)
        {
            if (!existingHealthCenter.IsDeleted && command.IsDeleted)
            {
                _logger.Warning("We can not delete an existing health center. The functionality is not implemented");
                return null;
            }

            var healthCenterToUpdate = _mapper.Map(command, existingHealthCenter);
            Core.Entities.HealthCenter updatedHealthCenter = await _healthCenterWriteRepository.UpdateAsync(healthCenterToUpdate, cancellationToken);

            _logger.Information("Entity HealthCenter with Id='{Id}' was updated", updatedHealthCenter.Id);
            return updatedHealthCenter.Id;
        }
    }
}
