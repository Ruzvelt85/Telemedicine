using System;
using System.Linq.Expressions;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Specifications
{
    /// <summary>
    /// Specification to get an entity by its InnerId
    /// </summary>
    public record ByInnerIdSpecification<TInnerIdSystemEntity> : Specification<TInnerIdSystemEntity>
        where TInnerIdSystemEntity : IInnerIdSystem
    {
        private readonly string _innerId;

        public ByInnerIdSpecification(string innerId)
        {
            _innerId = innerId;
        }

        /// <inheritdoc />
        protected override Expression<Func<TInnerIdSystemEntity, bool>> Predicate =>
            innerIdSystemEntity => innerIdSystemEntity.InnerId == _innerId;
    }
}
