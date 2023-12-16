namespace Telemedicine.Common.ServiceInfrastructure.ExceptionHandling.Service.Settings
{
    public record InfoLogExceptionFilterSettings
    {
        /// <summary>
        /// Http verb of the current request
        /// </summary>
        /// <example>GET</example>
        /// <example>POST</example>
        /// <example>PUT</example>
        public string? HttpVerb { get; init; }

        /// <summary>
        /// Path of the request
        /// </summary>
        /// <example>api/appointments</example>
        public string? Path { get; init; }

        /// <summary>
        /// Exception type
        /// </summary>
        /// <example>Telemedicine.Common.Contracts.GlobalContracts.Exceptions.Business.EntityNotFoundException</example>
        public string? Type { get; init; }

        /// <summary>
        /// Exception code
        /// </summary>
        /// <example>EntityNotFound</example>
        public string? Code { get; init; }

    }
}