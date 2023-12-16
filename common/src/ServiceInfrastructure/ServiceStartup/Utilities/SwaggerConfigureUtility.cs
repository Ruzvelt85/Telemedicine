using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities
{
    public static class SwaggerConfigureUtility
    {
        public static void Configure(IApplicationBuilder app, Assembly executingAssembly, ServiceStartupSettings startupSettings)
        {
            if (!startupSettings.IsSwaggerEnabled)
            {
                Log.Logger.Debug("Swagger is disabled, so it won't be added into the pipeline");
                return;
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint((startupSettings.SwaggerPathPrefix is null ? null : '/' + startupSettings.SwaggerPathPrefix) + "/swagger/v1/swagger.json", $"{executingAssembly} v1");
            });

            Log.Logger.Information("Swagger is successfully added into the pipeline");
        }

        public static void ConfigureSwagger(this IServiceCollection services, string executingAssemblyName, ServiceStartupSettings startupSettings, string? authAuthority)
        {
            if (!startupSettings.IsSwaggerEnabled)
            {
                Log.Logger.Debug("Swagger is disabled, so it won't be configured");
                return;
            }

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = executingAssemblyName, Version = "v1" });
                c.MapType<TimeSpan>(() => new OpenApiSchema { Type = "number" });
                c.CustomSchemaIds(type => type.ToString());
                c.CustomOperationIds(e =>
                {
                    string actionName = e.ActionDescriptor.RouteValues["action"];
                    string camelCaseActionName = actionName[0].ToString().ToLower() + actionName.Substring(1);

                    return camelCaseActionName;
                });

                if (!String.IsNullOrWhiteSpace(startupSettings.SwaggerPathPrefix))
                    c.DocumentFilter<PathPrefixInsertDocumentFilter>(startupSettings.SwaggerPathPrefix);

                c.OperationFilter<EnumFlagsAttributeSupportOperationFilter>();

                c.ConfigureSwaggerAuthorization(authAuthority);
            });

            services.AddSwaggerGenNewtonsoftSupport(); //needs to be placed after AddSwaggerGen(). It's used to display enum as strings.

            Log.Logger.Information("Swagger is successfully configured");
        }

        private static void ConfigureSwaggerAuthorization(this SwaggerGenOptions options, string? authority)
        {
            if (String.IsNullOrWhiteSpace(authority))
                return;

            const string securitySchemeName = "openIdConnect";
            // Define the OAuth2.0 scheme that's in use (it'll take the available security schemes from .well-known, i.e. authorization code & implicit)
            options.AddSecurityDefinition(securitySchemeName, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OpenIdConnect,
                OpenIdConnectUrl = new Uri($"{authority}/.well-known/openid-configuration") //can it be different URL here?
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = securitySchemeName
                        }
                    },
                    new string[] { }
                }
            });
        }
    }

    public class PathPrefixInsertDocumentFilter : IDocumentFilter
    {
        private readonly string _pathPrefix;

        public PathPrefixInsertDocumentFilter(string prefix)
        {
            this._pathPrefix = prefix;
        }

        /// <summary>
        /// Add prefix to all swagger document paths. 
        /// </summary>
        /// <param name="swaggerDoc"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = swaggerDoc.Paths.Keys.ToList();
            foreach (var path in paths)
            {
                var pathToChange = swaggerDoc.Paths[path];
                swaggerDoc.Paths.Remove(path);
                swaggerDoc.Paths.Add($"/{_pathPrefix}{path}", pathToChange);
            }
        }
    }

    public class EnumFlagsAttributeSupportOperationFilter : IOperationFilter
    {
        /// <inheritdoc />
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null || !operation.Parameters.Any())
            { return; }

            string[] parameterNamesWithFlagsAttribute = context.ApiDescription.ParameterDescriptions
                .Where(_ => _.Type.CustomAttributes.Any(a => a.AttributeType == typeof(FlagsAttribute)))
                .Select(_ => _.Name).ToArray();

            if (!parameterNamesWithFlagsAttribute.Any())
            { return; }

            OpenApiParameter[] openApiParameters = operation.Parameters
                .Where(parameter => parameterNamesWithFlagsAttribute.Contains(parameter.Name)).ToArray();

            foreach (var openApiParameter in openApiParameters)
            {
                openApiParameter.Schema = new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Reference = openApiParameter.Schema.Reference
                    }
                };
                openApiParameter.Style = ParameterStyle.Form;
            }
        }
    }
}
