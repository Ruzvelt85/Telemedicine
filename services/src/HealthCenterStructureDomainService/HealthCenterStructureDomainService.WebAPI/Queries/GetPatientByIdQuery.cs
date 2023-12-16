﻿using System;
using Telemedicine.Common.Infrastructure.Patterns.Query;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Queries
{
    public record GetPatientByIdQuery : IQuery
    {
        public GetPatientByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; init; }
    }
}
