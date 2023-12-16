using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Serilog;
using VidyoService;
using Telemedicine.Common.Infrastructure.VidyoClient.Exceptions;

namespace Telemedicine.Common.Infrastructure.VidyoClient
{
    public class VidyoClient : IVidyoClient, IDisposable
    {
        private readonly VidyoPortalUserServicePortTypeClient _vidyoClient;

        private readonly ILogger _logger = Log.ForContext<VidyoClient>();

        public VidyoClient(IVidyoClientFactory vidyoClientFactory, IOptionsSnapshot<VidyoServiceConnectionSettings> settings)
        {
            _vidyoClient = vidyoClientFactory.Create(settings.Value);
        }

        /// <summary>
        /// Creates new room in Vidyo service
        /// </summary>
        /// <exception cref="CreateConferenceException">Thrown when creating a room failed</exception>
        /// <exception cref="CreateConferenceWithInvalidParametersException">Thrown when creating a room failed due to invalid settings</exception>
        public async Task<CreateRoomResponse> CreateRoom(string roomExtension, string roomTitle)
        {
            var request = new CreateRoomRequest
            {
                extension = roomExtension,
                name = roomTitle
            };

            try
            {
                var response = await _vidyoClient.createRoomAsync(request);

                return response.CreateRoomResponse;
            }
            catch (FaultException<InvalidArgumentFault> ex)
            {
                _logger.Error(ex, "Error occurred while CreateRoom request to Vidyo service due to invalid parameters");
                throw new CreateConferenceWithInvalidParametersException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while CreateRoom request to Vidyo service");
                throw new CreateConferenceException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Sets PIN-code to the existing room in Vidyo service
        /// </summary>
        /// <exception cref="SetPinCodeException">Thrown when setting PIN-code failed</exception>
        public async Task SetPinCode(string roomId, string pinCode)
        {
            var request = new CreateRoomPINRequest
            {
                roomID = roomId,
                PIN = pinCode
            };

            try
            {
                await _vidyoClient.createRoomPINAsync(request);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while CreateRoomPIN request to Vidyo service");
                throw new SetPinCodeException(ex.Message, ex);
            }
        }

        public void Dispose()
        {
            try
            {
                _vidyoClient.Close();
            }
            catch (Exception exception)
            {
                Debug.Fail("Exception occurred during dispose");
                _logger.Warning(exception, "Exception occurred during dispose");
            }
        }
    }
}
