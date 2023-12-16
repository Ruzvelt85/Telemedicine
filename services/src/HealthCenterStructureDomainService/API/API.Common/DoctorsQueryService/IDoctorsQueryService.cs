using System.Threading;
using System.Threading.Tasks;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Refit;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.Common.Exceptions;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService.Dto;

namespace Telemedicine.Services.HealthCenterStructureDomainService.API.Common.DoctorsQueryService
{
    public interface IDoctorsQueryService
    {
        [Get("/api/Doctors")]
        Task<PagedListResponseDto<DoctorResponseDto>> GetDoctorListAsync(DoctorListRequestDto request, CancellationToken cancellationToken = default);

        /// <exception cref="EntityNotFoundByInnerIdException">Thrown when doctor was not found by innerId</exception>
        [Get("/api/Doctors/GetByInnerId/{request.InnerId}")]
        Task<DoctorByInnerIdResponseDto> GetDoctorByInnerIdAsync(DoctorByInnerIdRequestDto request, CancellationToken cancellationToken = default);
    }
}
