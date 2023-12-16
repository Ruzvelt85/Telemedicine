using System;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Unexpected;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Constants;
using Newtonsoft.Json;
using Serilog;

namespace Telemedicine.Common.Infrastructure.Refit
{
    /// <summary>
    /// Custom exception factory for handling exceptions received through Refit from domain services
    /// </summary>
    public class RefitExceptionFactory
    {
        private readonly ILogger _logger = Log.ForContext<RefitExceptionFactory>();

        /// <summary>
        /// Handles the exception received from HTTP-response
        /// </summary>
        public async Task<Exception?> HandleIntegrationException(HttpResponseMessage httpResponse)
        {
            if (httpResponse.IsSuccessStatusCode)
            {
                _logger.Debug("Http-response has success status code, no exception should be handled");
                return await Task.FromResult<Exception?>(null);
            }

            if (httpResponse.Content.Headers.ContentType?.MediaType != CommonConstants.JsonMediaType)
            {
                _logger.Warning("ContentType in http-response is not 'application/json', therefore it's impossible to restore ServiceLayerException and unexpected exception will be thrown");
                return GetUnexpectedException(httpResponse);
            }

            return await TryRestoreServiceLayerExceptionFromHttpResponse(httpResponse);
        }

        /// <summary>
        /// Tries to restore ServiceLayerException from http-response.
        /// In case of failure returns UnexpectedHttpIntegrationException
        /// </summary>
        private async Task<Exception?> TryRestoreServiceLayerExceptionFromHttpResponse(HttpResponseMessage httpResponse)
        {
            ExceptionDto? exceptionDto;

            try
            {
                string jsonString = await httpResponse.Content.ReadAsStringAsync();
                exceptionDto = JsonConvert.DeserializeObject<ExceptionDto>(jsonString);
                _logger.Information("ExceptionDto from http-response was deserialized successfully, Code='{Code}', Message='{Message}', Type='{Type}'", exceptionDto?.Code, exceptionDto?.Message, exceptionDto?.Type);
            }
            catch (JsonException ex)
            {
                _logger.Error("Impossible to restore ServiceLayerException from http-response: error while attempt to deserialize ExceptionDto, Exception: {ExceptionMessage}", ex.Message);
                Debug.Assert(false, "Error while deserialization ExceptionDto");
                return GetUnexpectedException(httpResponse, ex);
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected error occurred while attempt to get and deserialize content from http-response, Exception: {ExceptionMessage}", ex.Message);
                Debug.Assert(false, "Unexpected error while getting or deserialization http-response content");
                return GetUnexpectedException(httpResponse, ex);
            }

            if (string.IsNullOrWhiteSpace(exceptionDto?.Type))
            {
                const string message = "Impossible to restore ServiceLayerException from http-response: 'Type' parameter is NULL";
                _logger.Warning(message);
                Debug.Assert(false, message);
                return GetUnexpectedException(httpResponse);
            }

            var exceptionType = Type.GetType(exceptionDto.Type);

            if (exceptionType == null)
            {
                _logger.Warning("Impossible to restore ServiceLayerException from http-response: Exception Type cannot be determined, received value = '{Type}'", exceptionDto.Type);
                Debug.Assert(false, "Type of exception cannot be determined");
                return GetUnexpectedException(httpResponse);
            }

            try
            {
                var exception = Activator.CreateInstance(exceptionType, bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance, null, new object?[] { exceptionDto.Message, exceptionDto.Data, null }, CultureInfo.CurrentCulture);
                var resultException = exception as Exception;

                if (resultException == null)
                {
                    _logger.Warning("Type of restored object cannot be cast to 'Exception' Type, unexpected exception will be thrown");
                    Debug.Assert(false, "Type of restored object cannot be cast to 'Exception' Type");
                    return GetUnexpectedException(httpResponse);
                }

                _logger.Information("ServiceLayerException with type='{Type}' was successfully created", exceptionDto.Type);
                return resultException;
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while creating instance of ServiceLayerException with Type='{Type}', thrown exception: {ExceptionMessage}", exceptionDto.Type, ex.Message);
                Debug.Assert(false, "Error while creating instance of ServiceLayerException");
                return GetUnexpectedException(httpResponse, ex);
            }
        }

        /// <summary>
        /// Creates unexpected exception
        /// </summary>
        /// <returns></returns>
        private UnexpectedHttpIntegrationException GetUnexpectedException(HttpResponseMessage httpResponse, Exception? exception = null)
        {
            _logger.Information("Unexpected http-integration will be created");
            return new UnexpectedHttpIntegrationException(httpResponse.RequestMessage?.RequestUri?.AbsoluteUri,
                httpResponse.RequestMessage?.RequestUri?.LocalPath, httpResponse.StatusCode, exception);
        }
    }
}
