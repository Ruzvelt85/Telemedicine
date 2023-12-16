using System;
using System.Collections.Generic;
using Telemedicine.Common.Infrastructure.Patterns.DomainDrivenDesign;
using Newtonsoft.Json;

namespace Telemedicine.Services.HealthMeasurementDomainService.Core.Entities
{
    [ValueObject]
    public class RawSaturationData
    {
        [JsonConstructor]
        [Obsolete("Do not use this constructor for creating the entity")]
        public RawSaturationData(Guid id, ICollection<RawSaturationItem> items)
        {
            Id = id;
            Items = items;
        }

        public RawSaturationData(ICollection<RawSaturationItem> items)
        {
            Items = items;
        }

        public Guid Id { get; init; }

        public ICollection<RawSaturationItem> Items { get; init; }
    }
}
