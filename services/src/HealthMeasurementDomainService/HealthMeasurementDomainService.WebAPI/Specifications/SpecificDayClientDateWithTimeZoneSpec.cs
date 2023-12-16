using System;
using System.Linq.Expressions;
using Telemedicine.Common.Infrastructure.Patterns.Specifications;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Contracts;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Specifications
{
    public record SpecificDayClientDateWithTimeZoneSpec : Specification<IHasClientDate>
    {
        private readonly DateTime _dateTime;
        private readonly TimeSpan _timeOffset;

        public SpecificDayClientDateWithTimeZoneSpec(DateTime dateTime, TimeSpan timeZoneOffset)
        {
            _dateTime = dateTime;
            _timeOffset = timeZoneOffset;
        }

        /// <inheritdoc />
        protected override Expression<Func<IHasClientDate, bool>> Predicate =>
            el => (el.ClientDate + _timeOffset).Date == (_dateTime + _timeOffset).Date;
    }
}
