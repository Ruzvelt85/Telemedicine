using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.HealthCenterStructureDomainService.API.Common.PatientsQueryService.Dto.GetPatientList;

namespace Telemedicine.Services.HealthCenterStructureDomainService.WebAPI.Queries
{
    public record GetPatientsQuery : IQuery
    {
        public GetPatientsQuery(PatientListFilterRequestDto filter, PagingRequestDto paging, SortingType firstNameSortingType)
        {
            Filter = filter;
            Paging = paging;
            FirstNameSortingType = firstNameSortingType;
        }

        public PatientListFilterRequestDto Filter { get; init; }

        public PagingRequestDto Paging { get; init; }

        public SortingType FirstNameSortingType { get; init; }
    }
}
