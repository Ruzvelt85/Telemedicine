using JetBrains.Annotations;

namespace Telemedicine.Common.Contracts.GlobalContracts.ValueObjects
{
    /// <summary>
    /// Generic-row, which provide range with boundaries <see cref="From"/> and <see cref="To"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [PublicAPI]
    public record Range<T>
    {
        public Range(T from, T to)
        {
            From = from;
            To = to;
        }

        public Range()
        { }

        public T? From { get; set; }
        public T? To { get; set; }
    }
}
