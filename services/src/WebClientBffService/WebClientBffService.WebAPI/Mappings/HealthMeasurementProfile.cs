using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Services.WebClientBffService.API.Common;
using Telemedicine.Services.WebClientBffService.API.HealthMeasurements.HealthMeasurementQueryService.Dto;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements.BloodPressure;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements.Mood;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements.PulseRate;
using Telemedicine.Services.WebClientBffService.WebAPI.Queries.HealthMeasurements.Saturation;
using HealthMeasurementDomain = Telemedicine.Services.HealthMeasurementDomainService.API.Common;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Mappings
{
    public class HealthMeasurementProfile : Profile
    {
        public HealthMeasurementProfile()
        {
            CreateMap<GetMeasurementListRequestDto, GetHealthMeasurementListQuery>();

            CreateMap<GetHealthMeasurementListQuery, GetMeasurementListQuery>();
            CreateMap<GetMeasurementListQuery, GetSaturationMeasurementListQuery>();
            CreateMap<GetMeasurementListQuery, GetMoodMeasurementListQuery>();
            CreateMap<GetMeasurementListQuery, GetBloodPressureMeasurementListQuery>();
            CreateMap<GetMeasurementListQuery, GetPulseRateMeasurementListQuery>();

            CreateMap<GetSaturationMeasurementListQuery, HealthMeasurementDomain.GetMeasurementListRequestDto>();
            CreateMap<GetMoodMeasurementListQuery, HealthMeasurementDomain.GetMeasurementListRequestDto>();
            CreateMap<GetBloodPressureMeasurementListQuery, HealthMeasurementDomain.GetMeasurementListRequestDto>();
            CreateMap<GetPulseRateMeasurementListQuery, HealthMeasurementDomain.GetMeasurementListRequestDto>();
            CreateMap<MeasurementListFilterRequestDto, HealthMeasurementDomain.MeasurementListFilterRequestDto>(MemberList.Destination);

            CreateMap<IReadOnlyCollection<IHasClientDate>, MeasurementListResponseDto>()
                .ForMember(dest => dest.BloodPressureItems, opt => opt.MapFrom(src => src.OfType<BloodPressureResponseDto>()))
                .ForMember(dest => dest.SaturationItems, opt => opt.MapFrom(src => src.OfType<SaturationResponseDto>()))
                .ForMember(dest => dest.MoodItems, opt => opt.MapFrom(src => src.OfType<MoodResponseDto>()))
                .ForMember(dest => dest.PulseRateItems, opt => opt.MapFrom(src => src.OfType<PulseRateResponseDto>()))
                .ForMember(dest => dest.Paging, opt => opt.Ignore());

            CreateMap<PagedListResponseDto<HealthMeasurementDomain.MeasurementResponseDto<HealthMeasurementDomain.Saturation.Dto.SaturationMeasurementDto>>, MeasurementListResponse>();
            CreateMap<PagedListResponseDto<HealthMeasurementDomain.MeasurementResponseDto<HealthMeasurementDomain.BloodPressure.Dto.BloodPressureMeasurementDto>>, MeasurementListResponse>();
            CreateMap<PagedListResponseDto<HealthMeasurementDomain.MeasurementResponseDto<HealthMeasurementDomain.PulseRate.Dto.PulseRateMeasurementDto>>, MeasurementListResponse>();
            CreateMap<PagedListResponseDto<HealthMeasurementDomain.MeasurementResponseDto<HealthMeasurementDomain.Mood.Dto.MoodMeasurementDto>>, MeasurementListResponse>();

            CreateMap<HealthMeasurementDomain.MeasurementResponseDto<HealthMeasurementDomain.Saturation.Dto.SaturationMeasurementDto>, IHasClientDate>()
                .As<SaturationResponseDto>();
            CreateMap<HealthMeasurementDomain.MeasurementResponseDto<HealthMeasurementDomain.BloodPressure.Dto.BloodPressureMeasurementDto>, IHasClientDate>()
                .As<BloodPressureResponseDto>();
            CreateMap<HealthMeasurementDomain.MeasurementResponseDto<HealthMeasurementDomain.PulseRate.Dto.PulseRateMeasurementDto>, IHasClientDate>()
                .As<PulseRateResponseDto>();
            CreateMap<HealthMeasurementDomain.MeasurementResponseDto<HealthMeasurementDomain.Mood.Dto.MoodMeasurementDto>, IHasClientDate>()
                .As<MoodResponseDto>();

            CreateMap<HealthMeasurementDomain.MeasurementResponseDto<HealthMeasurementDomain.Saturation.Dto.SaturationMeasurementDto>, SaturationResponseDto>(MemberList.Source)
                .IncludeMembers(src => src.Measure);
            CreateMap<HealthMeasurementDomain.MeasurementResponseDto<HealthMeasurementDomain.BloodPressure.Dto.BloodPressureMeasurementDto>, BloodPressureResponseDto>(MemberList.Source)
                .IncludeMembers(src => src.Measure);
            CreateMap<HealthMeasurementDomain.MeasurementResponseDto<HealthMeasurementDomain.PulseRate.Dto.PulseRateMeasurementDto>, PulseRateResponseDto>(MemberList.Source)
                .IncludeMembers(src => src.Measure);
            CreateMap<HealthMeasurementDomain.MeasurementResponseDto<HealthMeasurementDomain.Mood.Dto.MoodMeasurementDto>, MoodResponseDto>(MemberList.Source)
                .ForMember(dest => dest.Measure, opt => opt.MapFrom(src => src.Measure.Measure));

            CreateMap<HealthMeasurementDomain.Saturation.Dto.SaturationMeasurementDto, SaturationResponseDto>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ClientDate, opt => opt.Ignore());
            CreateMap<HealthMeasurementDomain.BloodPressure.Dto.BloodPressureMeasurementDto, BloodPressureResponseDto>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ClientDate, opt => opt.Ignore());
            CreateMap<HealthMeasurementDomain.PulseRate.Dto.PulseRateMeasurementDto, PulseRateResponseDto>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ClientDate, opt => opt.Ignore());
            CreateMap<HealthMeasurementDomain.Mood.Dto.MoodMeasurementDto, MoodResponseDto>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ClientDate, opt => opt.Ignore());
        }
    }
}
