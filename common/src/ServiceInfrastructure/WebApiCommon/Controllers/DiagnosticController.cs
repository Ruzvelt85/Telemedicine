using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.Infrastructure.Logger.LoggerLibrarySerilogBase.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Logging;

namespace Telemedicine.Common.ServiceInfrastructure.WebApiCommon.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class DiagnosticController : ServiceBaseController
    {
        private readonly ILogger<DiagnosticController> _logger;
        private readonly IConfiguration _configuration;

        public DiagnosticController(IConfiguration configuration, ILogger<DiagnosticController> logger)
        {
            this._configuration = configuration;
            _logger = logger;
        }

        [HttpGet("CheckLog")]
        public void CheckLog() => LoggerUtilities.CheckLog(_logger);

        /// <summary>
        /// Returns information about service
        /// </summary>
        [HttpGet("About")]
        public string About() => $"{ReflectionUtilities.GetExecutingAssemblyName()} {ReflectionUtilities.GetExecutingAssemblyVersion()}";


        /// <summary>
        /// Returns all configuration of service 
        /// </summary>
        [HttpGet("GetAllConfigs")]
        public Dictionary<string, string> GetAllConfigs() => _configuration.AsEnumerable().ToDictionary(item => item.Key, item => item.Value);

        /// <summary>
        /// Returns value from configuration by key
        /// </summary>
        [HttpGet("GetConfigFromKey/{key}")]
        public string GetAllConfigs(string key) => _configuration.GetValue<string>(key);

        /// <summary>
        /// Returns keys and values from environment variables directly
        /// </summary>
        [HttpGet("GetConfigDataFromEnvironmentVariableProvider")]
        public Dictionary<string, string> GetConfigDataFromEnvironmentVariableProvider()
        {
            var dictionary = new Dictionary<string, string>();
            if (((IConfigurationRoot)_configuration).Providers.FirstOrDefault(prov => prov is EnvironmentVariablesConfigurationProvider) is
                EnvironmentVariablesConfigurationProvider provider)
            {
                foreach (var key in provider.GetChildKeys(Array.Empty<string>(), null))
                {
                    provider.TryGet(key, out var value);
                    if (!dictionary.ContainsKey(key))
                    {
                        dictionary.Add(key, value);
                    }
                    else
                    {
                        dictionary[key] = value;
                    }
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Returns environment variables
        /// </summary>
        [HttpGet("GetEnvVariable")]
        public IDictionary GetEnvVariable() => Environment.GetEnvironmentVariables();

        /// <summary>
        /// Throws exception for diagnostic purposes
        /// </summary>
        [HttpGet("CheckError")]
        public void CheckExceptionHandling() => throw new Exception("Test exception handling");
    }
}
