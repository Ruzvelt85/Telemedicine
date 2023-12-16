using System;
using System.Linq.Expressions;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;
using Telemedicine.Services.HealthCenterStructureDomainService.Core.Entities;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Specifications
{
    /// <summary>
    /// Specification to get an entity by its InnerId
    /// </summary>
    public record ByPersonNameSpecification<TPersonNameEntity> : Specification<TPersonNameEntity>
        where TPersonNameEntity : IPersonName
    {
        private readonly string _nameFilter;

        public ByPersonNameSpecification(string nameFilter)
        {
            _nameFilter = nameFilter.ToLowerAndTrim().RemoveDuplicateSpaces();
        }

        /// <inheritdoc />
        protected override Expression<Func<TPersonNameEntity, bool>> Predicate
        {
            get
            {
                var nameParts = _nameFilter.Split(' ');
                return nameParts.Length switch
                {
                    > 1 => person => person.FirstName.ToLower().StartsWith(nameParts[0]) && person.LastName.ToLower().StartsWith(nameParts[1])
                                     || person.FirstName.ToLower().StartsWith(nameParts[1]) && person.LastName.ToLower().StartsWith(nameParts[0]),
                    > 0 => person => person.FirstName.ToLower().StartsWith(nameParts[0]) || person.LastName.ToLower().StartsWith(nameParts[0]),
                    _ => person => false
                };
            }
        }
    }
}
