using System;
using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Services.VideoConfIntegrService.Core.Entities;
using Telemedicine.Services.VideoConfIntegrService.Core.Repositories;
using Telemedicine.Services.VideoConfIntegrService.WebAPI.Services;

namespace Telemedicine.Services.VideoConfIntegrService.WebAPI.Commands
{
    public class CreateConferenceCommandHandler : ICommandHandler<CreateConferenceCommand, Guid>
    {
        private readonly IConferenceFactory _conferenceFactory;
        private readonly IConferenceWriteRepository _conferenceWriteRepository;

        public CreateConferenceCommandHandler(IConferenceFactory conferenceFactory, IConferenceWriteRepository conferenceWriteRepository)
        {
            _conferenceFactory = conferenceFactory ?? throw new ArgumentNullException(nameof(conferenceFactory));
            _conferenceWriteRepository = conferenceWriteRepository ?? throw new ArgumentNullException(nameof(conferenceWriteRepository));
        }

        public async Task<Guid> HandleAsync(CreateConferenceCommand command, CancellationToken cancellationToken = default)
        {
            Conference conference = await _conferenceFactory.Create(command);

            Conference result = await _conferenceWriteRepository.AddAsync(conference, cancellationToken);
            return result.Id;
        }
    }
}
