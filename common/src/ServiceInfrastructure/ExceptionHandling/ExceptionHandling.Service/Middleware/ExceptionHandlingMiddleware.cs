using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Serilog;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Unexpected;
using Telemedicine.Common.Contracts.GlobalContracts.ValueObjects;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Constants;
using Telemedicine.Common.ServiceInfrastructure.ExceptionHandling.Service.Settings;
using Telemedicine.Common.ServiceInfrastructure.ExceptionHandling.Service.Utilities;

namespace Telemedicine.Common.ServiceInfrastructure.ExceptionHandling.Service.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger = Log.ForContext<ExceptionHandlingMiddleware>();
        private readonly IOptionsMonitor<Dictionary<string, InfoLogExceptionFilterSettings>> _optionsMonitor;

        public ExceptionHandlingMiddleware(RequestDelegate next, IOptionsMonitor<Dictionary<string, InfoLogExceptionFilterSettings>> optionsMonitor)
        {
            _next = next;
            _optionsMonitor = optionsMonitor;
        }

        [PublicAPI]
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception exception)
            {
                ProcessException(httpContext, exception);
            }
        }

        protected virtual void ProcessException(HttpContext httpContext, Exception exception)
        {
            switch (exception)
            {
                case ServiceLayerException serviceLayerException:
                    LogException(httpContext, exception);
                    ProcessServiceLayerExceptionAsync(httpContext, serviceLayerException);
                    break;
                case AggregateException aggregateException:
                    ProcessAggregateException(httpContext, aggregateException);
                    break;
                default: //all other exceptions are transformed to UnexpectedException
                    LogException(httpContext, exception);
                    ProcessServiceLayerExceptionAsync(httpContext, CreateUnexpectedException($"Unexpected exception occurred: {exception.Message}", exception));
                    break;
            }
        }

        protected virtual Task ProcessServiceLayerExceptionAsync(HttpContext httpContext, ServiceLayerException exception)
        {
            var exceptionDto = CreateExceptionDto(httpContext, exception);
            return WriteExceptionDtoToResponseAsync(httpContext, exceptionDto);
        }

        private void ProcessAggregateException(HttpContext httpContext, AggregateException aggregateException) =>
            ProcessException(httpContext, aggregateException.Flatten().InnerException ??
                CreateUnexpectedException("AggregationException had empty inner exception", aggregateException)); // We suppose that this case is impossible, but decide to handle it

        protected virtual void LogException(HttpContext httpContext, Exception exception)
        {
            switch (exception)
            {
                case ServiceLayerException serviceLayerException: LogServiceLayerException(httpContext, serviceLayerException); break;
                default: _logger.Error(exception, "Unexpected exception occurred: {ExceptionMessage}", exception.Message); break;
            }
        }

        protected void LogServiceLayerException(HttpContext context, ServiceLayerException ex)
        {
            IReadOnlyCollection<InfoLogExceptionFilterSettings> filterSettings = _optionsMonitor.CurrentValue
                .Select(_ => _.Value)
                .ToArray();
            string? path = (context.GetEndpoint() as Microsoft.AspNetCore.Routing.RouteEndpoint)?.RoutePattern.RawText;

            if (path is null)
            { _logger.Warning("Route pattern is null"); }

            bool isLogAsInfo = false;

            if (!string.IsNullOrWhiteSpace(path) && filterSettings.Any())
            { isLogAsInfo = ExceptionHandlingUtility.IsExceptionToLogAsInfo(filterSettings, context.Request.Method, path, ex); }

            if (isLogAsInfo)
            { _logger.Information(ex, "Message: {Message}", ex.Message); }
            else
            { _logger.Error(ex, "Message: {Message}", ex.Message); }
        }

        protected virtual ExceptionDto CreateExceptionDto(HttpContext httpContext, ServiceLayerException exception) =>
            new() { Code = exception.Code, Message = exception.Message, Data = exception.Data, Type = exception.GetType().AssemblyQualifiedName };

        private Task WriteExceptionDtoToResponseAsync(HttpContext context, ExceptionDto exceptionDto, int statusCode = StatusCodes.Status400BadRequest)
        {
            context.Response.ContentType = CommonConstants.JsonMediaType;
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(JsonSerializer.Serialize(exceptionDto));
        }

        private UnexpectedException CreateUnexpectedException(string message, Exception innerException)
        {
            Debug.Fail("Unexpected exception occurred");
            return new UnexpectedException(message, UnexpectedException.ErrorType.UnexpectedException, innerException);
        }
    }
}
