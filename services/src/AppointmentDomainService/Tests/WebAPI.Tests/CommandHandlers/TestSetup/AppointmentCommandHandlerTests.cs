using System.Threading.Tasks;
using Telemedicine.Common.Infrastructure.DAL.EfCoreDal;
using Telemedicine.Services.AppointmentDomainService.Core.DAL;
using Telemedicine.Services.AppointmentDomainService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace Telemedicine.Services.AppointmentDomainService.Tests.WebAPI.Tests.CommandHandlers.TestSetup
{
    public abstract class AppointmentCommandHandlerTests
    {
        private readonly string _databaseName;

        private readonly IOptionsSnapshot<EfCoreDbContextSettings> _options;

        protected AppointmentCommandHandlerTests(string databaseName)
        {
            _databaseName = databaseName;

            var mockOptions = new Mock<IOptionsSnapshot<EfCoreDbContextSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new EfCoreDbContextSettings());
            _options = mockOptions.Object;
        }

        public async Task<AppointmentDomainServiceDbContext> GetDbContext()
        {
            var dbContextOptions = new DbContextOptionsBuilder<AppointmentDomainServiceDbContext>()
                .UseInMemoryDatabase(_databaseName);

            var context = new AppointmentDomainServiceDbContext(dbContextOptions.Options, _options, new NullLoggerFactory());
            await context.Database.EnsureCreatedAsync();

            return context;
        }

        public async Task AddAsync(Appointment appointment)
        {
            using var context = await GetDbContext();
            context.Add(appointment);
            await context.SaveChangesAsync();
        }

        public async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues) where TEntity : class
        {
            using var context = await GetDbContext();
            return await context.FindAsync<TEntity>(keyValues);
        }
    }
}
