using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.Patterns.Query;
using Telemedicine.Services.WebClientBffService.API.Patients.PatientQueryService.Dto.GetPatientList;

namespace Telemedicine.Services.WebClientBffService.WebAPI.Queries.Patients
{
    public record GetPatientListQuery : IQuery
    {
        public GetPatientListQuery(PatientListFilterRequestDto filter, PagingRequestDto paging, SortingType firstNameSortingType)
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
