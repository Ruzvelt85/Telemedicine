using AutoMapper;
using Telemedicine.Services.HealthMeasurementDomainService.API.Common;
using Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Queries;

namespace Telemedicine.Services.HealthMeasurementDomainService.WebAPI.Mappings
{
    public class HealthMeasurementProfile : Profile
    {
        public HealthMeasurementProfile()
        {
            CreateMap<GetMeasurementListRequestDto, GetMeasurementListQuery>();
        }
    }
}
