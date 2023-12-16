using Refit;

namespace Telemedicine.Common.Infrastructure.Refit
{
    public class RefitDefaultSettings
    {
        public static RefitSettings Settings => new RefitSettings
        {
            ContentSerializer = new NewtonsoftJsonContentSerializer(JsonSerializerUtility.CreateAndConfigureJsonSerializerSettings()),
            ExceptionFactory = httpResponse => new RefitExceptionFactory().HandleIntegrationException(httpResponse),
            // It change query string params from default to compatible with ASP.Net
            // example default: /users/list?ages=10%2C20%2C30 multi: /users/list?ages=10&ages=20&ages=30
            CollectionFormat = CollectionFormat.Multi
        };

    }
}
