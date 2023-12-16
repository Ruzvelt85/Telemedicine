using System.Threading.Tasks;
using Telemedicine.Services.VideoConfIntegrService.Core.Entities;
using Telemedicine.Services.VideoConfIntegrService.WebAPI.Commands;

namespace Telemedicine.Services.VideoConfIntegrService.WebAPI.Services
{
    public interface IConferenceFactory
    {
        public Task<Conference> Create(CreateConferenceCommand command);
    }
}
