using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService.Dto.GetPatientList;
using Telemedicine.Services.HealthCenterStructureDomainService.DAL;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI
{
    public class Startup : ServiceStartupBase
    {
        protected override Type EfCoreContextType => typeof(HealthCenterStructureDomainServiceDbContext);

        /// <inheritdoc />
        protected override Assembly[] ValidationAssemblies => new[]
        {
            typeof(PatientListRequestDto).Assembly,
            typeof(PagingRequestDtoValidator).Assembly
        };

        public Startup(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
