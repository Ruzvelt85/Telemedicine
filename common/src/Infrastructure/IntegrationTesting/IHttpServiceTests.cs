using Xunit;

namespace Telemedicine.Common.Infrastructure.IntegrationTesting
{
    public interface IHttpServiceTests<TService> : IClassFixture<HttpServiceFixture<TService>>
    {
        public HttpServiceFixture<TService> HttpServiceFixture { get; }
    }
}
