using System;
using System.Linq.Expressions;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Contracts;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Specifications
{
    public record ClientDateRangeSpec : Specification<IHasClientDate>
    {
        private readonly DateTime? _from;

        private readonly DateTime? _to;

        public ClientDateRangeSpec(DateTime? from, DateTime? to)
        {
            _from = from;
            _to = to;
        }

        /// <inheritdoc />
        protected override Expression<Func<IHasClientDate, bool>> Predicate
        {
            get
            {
                if (_to == null && _from == null) return new TrueSpecification<IHasClientDate>();

                if (_to == null) return obj => obj.ClientDate >= _from;

                if (_from == null) return obj => obj.ClientDate < _to;

                return obj => obj.ClientDate >= _from && obj.ClientDate < _to;
            }
        }
    }
}
