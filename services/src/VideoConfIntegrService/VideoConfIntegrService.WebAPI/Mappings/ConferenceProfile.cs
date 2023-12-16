using AutoMapper;
using VidyoService;
using Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceCommandService.Dto;
using Telemedicine.Services.VideoConfIntegrService.API.Common.VideoConferenceQueryService.Dto;
using Telemedicine.Services.VideoConfIntegrService.Core.Entities;
using Telemedicine.Services.VideoConfIntegrService.WebAPI.Commands;
using Telemedicine.Services.VideoConfIntegrService.WebAPI.Services;

namespace Telemedicine.Services.VideoConfIntegrService.WebAPI.Mappings
{
    public class ConferenceProfile : Profile
    {
        public ConferenceProfile()
        {
            CreateMap<CreateConferenceRequestDto, CreateConferenceCommand>();

            CreateMap<CreateConferenceCommand, CreateConferenceDto>(MemberList.Source);

            CreateMap<CreateRoomResponse, CreateConferenceDto>()
                .ForMember(dest => dest.RoomId,
                    opt => opt.MapFrom(_ => _.Entity.entityID))
                .ForMember(dest => dest.FullExtension,
                    opt => opt.MapFrom(_ => _.Entity.extension))
                .ForMember(dest => dest.RoomUrl,
                    opt => opt.MapFrom(_ => _.Entity.RoomMode.roomURL))
                .ForMember(dest => dest.RoomPin,
                    opt => opt.MapFrom(_ => _.Entity.RoomMode.hasPIN ? _.Entity.RoomMode.roomPIN : null))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<CreateConferenceDto, Conference>(MemberList.Source);

            CreateMap<Conference, ConnectionInfoResponseDto>(MemberList.Destination)
                .ForMember(dest => dest.Host, opt => opt.Ignore())
                .ForMember(dest => dest.RoomKey, opt => opt.Ignore());
        }
    }
}
