using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.Options;
using AutoMapper;
using VidyoService;
using Serilog;
using Telemedicine.Common.Infrastructure.VidyoClient;
using Telemedicine.Common.Infrastructure.VidyoClient.Exceptions;
using Telemedicine.Services.VideoConfIntegrService.Core.Entities;
using Telemedicine.Services.VideoConfIntegrService.WebAPI.Commands;
using CreateConferenceException = Telemedicine.Common.Infrastructure.VidyoClient.Exceptions.CreateConferenceException;

namespace Telemedicine.Services.VideoConfIntegrService.WebAPI.Services
{
    public class ConferenceFactory : IConferenceFactory
    {
        private readonly IMapper _mapper;
        private readonly ConferenceSettings _settings;
        private readonly IVidyoClient _vidyoClient;

        private readonly ILogger _logger = Log.ForContext<ConferenceFactory>();

        public ConferenceFactory(IMapper mapper, IOptionsSnapshot<ConferenceSettings> settings, IVidyoClient vidyoClient)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _vidyoClient = vidyoClient ?? throw new ArgumentNullException(nameof(vidyoClient));
        }

        /// <summary>
        /// Creates a new conference
        /// </summary>
        /// <exception cref="CreateConferenceException">Thrown when error occurred while creating a room</exception>
        public async Task<Conference> Create(CreateConferenceCommand command)
        {
            CreateRoomResponse serviceResponse = await CreateRoom(command);

            Conference conference = MapToConferenceEntity(command, serviceResponse);

            if (_settings.IsSetPinCode)
            {
                await SetPinCode(conference, serviceResponse.Entity.entityID);
            }

            // TODO: JD-585 - Delete the created room if saving in database failed
            return conference;
        }

        /// <summary>
        /// Creates a new room in external service Vidyo
        /// </summary>
        /// <exception cref="CreateConferenceException">Thrown when error occurred while creating a room</exception>
        private async Task<CreateRoomResponse> CreateRoom(CreateConferenceCommand command)
        {
            string roomExtension = ConferenceExtensionProvider.GenerateExtension(_settings.ExtensionPrefix);

            try
            {
                return await _vidyoClient.CreateRoom(roomExtension, command.AppointmentId.ToString());
            }
            catch (CreateConferenceWithInvalidParametersException ex)
            {
                _logger.Warning(ex, "Creating new Conference failed due to invalid parameters...");
                throw new API.Common.VideoConferenceCommandService.Exceptions.CreateConferenceException("Creating a conference failed due to invalid parameters", ex);
            }
            catch (CreateConferenceException e)
            {
                _logger.Error(e, "Creating new conference failed");
                throw new API.Common.VideoConferenceCommandService.Exceptions.CreateConferenceException("Creating a new conference failed", e);
            }
        }

        /// <summary>
        /// Make mapping to entity
        /// </summary>
        private Conference MapToConferenceEntity(CreateConferenceCommand command, CreateRoomResponse response)
        {
            var conferenceDto = _mapper.Map<CreateConferenceDto>(command);
            conferenceDto = _mapper.Map(response, conferenceDto) with { XmlResponse = GetXmlResponse(response) };
            var result = _mapper.Map<Conference>(conferenceDto);
            return result;
        }

        private string GetXmlResponse(CreateRoomResponse response)
        {
            XmlSerializer serializer = new(typeof(CreateRoomResponse));
            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    serializer.Serialize(writer, response);
                    return sww.ToString();
                }
            }
        }

        /// <summary>
        /// Sets PIN-code to created conference
        /// </summary>
        private async Task SetPinCode(Conference conference, string roomId)
        {
            var pinCode = ConferencePinCodeGenerator.Generate(_settings.PinCodeFormat);

            try
            {
                await _vidyoClient.SetPinCode(roomId, pinCode);
                conference.RoomPin = pinCode;
            }
            catch (SetPinCodeException ex)
            {
                _logger.Warning(ex, "Setting PIN-code to created conference failed, so the conference will be left without PIN-code...");
            }
        }
    }
}
