namespace Telemedicine.Common.Contracts.GlobalContracts.ValueObjects
{
    /// <summary>
    /// Defines dto to use with request dto in case of using pagination
    /// </summary>
    public record PagingRequestDto(int Take = 100, int Skip = 0)
    {
        private static readonly PagingRequestDto _firstItemInstance = new(1);

        public static PagingRequestDto FirstItem => _firstItemInstance;
    }
}
