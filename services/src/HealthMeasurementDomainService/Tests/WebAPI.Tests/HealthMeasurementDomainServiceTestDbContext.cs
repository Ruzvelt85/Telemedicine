using System.Collections.Generic;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.HealthMeasurementDomainService.Core.Entities;
using Telemedicine.Services.HealthMeasurementDomainService.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Telemedicine.Services.HealthMeasurementDomainService.Tests.WebAPI.Tests
{
    public class HealthMeasurementDomainServiceTestDbContext : HealthMeasurementDomainServiceDbContext
    {
        public HealthMeasurementDomainServiceTestDbContext(DbContextOptions options, IOptionsSnapshot<EfCoreDbContextSettings> settings, ILoggerFactory loggerFactory)
            : base(options, settings, loggerFactory)
        {
        }

        public HealthMeasurementDomainServiceTestDbContext(DbContextOptions options, EfCoreDbContextSettings settings, ILoggerFactory loggerFactory)
            : base(options, settings, loggerFactory)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // In memory database does not support jsonb column type
            builder.Entity<RawSaturationData>().Property(p => p.Items)
                .HasConversion(v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<ICollection<RawSaturationItem>>(v)!);
        }
    }
}
