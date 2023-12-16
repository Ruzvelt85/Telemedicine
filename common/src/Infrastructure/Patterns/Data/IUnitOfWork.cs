using System.Threading;
using System.Threading.Tasks;

namespace Telemedicine.Common.Infrastructure.Patterns.Data
{
    /// <summary>
    /// The only endpoint for saving data in EF Core context
    /// </summary>
    public interface IUnitOfWork
    {
        Task SaveAsync(CancellationToken cancellationToken = default);
    }
}
