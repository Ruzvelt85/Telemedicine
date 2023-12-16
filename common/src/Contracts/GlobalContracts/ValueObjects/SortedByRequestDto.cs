namespace Telemedicine.Common.Contracts.GlobalContracts.ValueObjects
{
    public record SortedByRequestDto<T>
    {
        public T? SortedBy { get; init; }

        public SortingType SortingType { get; init; }
    }
}
