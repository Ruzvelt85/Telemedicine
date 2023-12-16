using System.Threading.Tasks;
using VidyoService;

namespace Telemedicine.Common.Infrastructure.VidyoClient
{
    public interface IVidyoClient
    {
        public Task<CreateRoomResponse> CreateRoom(string roomExtension, string roomTitle);

        public Task SetPinCode(string roomId, string pinCode);
    }
}
