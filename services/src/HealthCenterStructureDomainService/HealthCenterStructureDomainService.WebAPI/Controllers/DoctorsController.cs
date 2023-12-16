using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Controllers;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService.Dto;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Queries;
using Telemedicine.Common.Infrastructure.Patterns.Command;
using Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Filters;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsCommandService.Dto;
using Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Commands.Doctor;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class DoctorsController : DomainServiceBaseController, IDoctorsQueryService
    {
        private readonly IMapper _mapper;
        private readonly IQueryHandler<GetDoctorListQuery, PagedListResponseDto<DoctorResponseDto>> _getDoctorListQueryHandler;
        private readonly IQueryHandler<GetDoctorByInnerIdQuery, DoctorByInnerIdResponseDto> _getDoctorByIdQueryHandler;
        private readonly ICommandHandler<CreateOrUpdateDoctorCommand, Guid?> _createOrUpdateDoctorHandler;

        public DoctorsController(IMapper mapper,
            IQueryHandler<GetDoctorListQuery, PagedListResponseDto<DoctorResponseDto>> getDoctorListQueryHandler,
            IQueryHandler<GetDoctorByInnerIdQuery, DoctorByInnerIdResponseDto> getDoctorByIdQueryHandler,
            ICommandHandler<CreateOrUpdateDoctorCommand, Guid?> createOrUpdateDoctorHandler)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _getDoctorListQueryHandler = getDoctorListQueryHandler ?? throw new ArgumentNullException(nameof(getDoctorListQueryHandler));
            _getDoctorByIdQueryHandler = getDoctorByIdQueryHandler ?? throw new ArgumentNullException(nameof(getDoctorByIdQueryHandler));
            _createOrUpdateDoctorHandler = createOrUpdateDoctorHandler;
        }

        [HttpGet]
        //[Authorize(Policy = "Admin")] // TODO: Sapegin, JD-1374 - Add security policy for Admin
        public async Task<PagedListResponseDto<DoctorResponseDto>> GetDoctorListAsync([FromQuery] DoctorListRequestDto request, CancellationToken cancellationToken = default)
        {
            var query = _mapper.Map<GetDoctorListQuery>(request);
            var doctors = await _getDoctorListQueryHandler.HandleAsync(query, cancellationToken);
            return doctors;
        }

        [HttpGet("GetByInnerId/{InnerId}")]
        public async Task<DoctorByInnerIdResponseDto> GetDoctorByInnerIdAsync([FromRoute] DoctorByInnerIdRequestDto request, CancellationToken cancellationToken = default)
        {
            var query = _mapper.Map<GetDoctorByInnerIdQuery>(request);
            var doctorId = await _getDoctorByIdQueryHandler.HandleAsync(query, cancellationToken);
            return doctorId;
        }

        [HttpPost]
        [ServiceFilter(typeof(SaveChangesActionFilterAttribute))]
        public async Task<Guid?> CreateOrUpdateAsync([FromBody] CreateOrUpdateDoctorRequestDto request, CancellationToken cancellationToken = default)
        {
            var command = _mapper.Map<CreateOrUpdateDoctorCommand>(request);
            var result = await _createOrUpdateDoctorHandler.HandleAsync(command, cancellationToken);
            return result;
        }
    }
}
