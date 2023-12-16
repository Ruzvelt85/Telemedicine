using System;
using System.Collections.Generic;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities.Authorization;
// ReSharper disable UnusedMember.Global

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Settings
{
    /// <summary>
    /// Settings to be mapped into <see cref="Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions"/>
    /// Only primitive types properties are available. If you need complex types properties you may add them into the class and adding a mapping rule for them into <see cref="JwtBearerMapping"/>
    /// </summary>
    public record JwtBearerSettings : AuthenticationSchemeSettings
    {
        public bool? MapInboundClaims { get; set; }

        public bool? IncludeErrorDetails { get; set; }

        public bool? SaveToken { get; set; }

        public TokenValidationParameters? TokenValidationParameters { get; set; }

        public bool? RefreshOnIssuerKeyNotFound { get; set; }

        public TimeSpan? AutomaticRefreshInterval { get; set; }

        public string? Challenge { get; set; }

        public string? Audience { get; set; }

        /// <summary>
        /// If set, it will automatically get the data from the specified URL from .well-known/jwks.json, cache it.
        /// Then, from the JWK data ('e', 'n' in our case) with the matching "kid" from the token, it will generate a Public key and validate the signature.
        /// If you want to change the behaviour, leave the field 'null' and use <see cref="JwtBearerSettings.TokenValidationParameters"/> settings instead.
        /// To avoid sending the automatic request to the server and still to verify the signature, you'll need to have the Public Key locally and to verify it manually.
        /// </summary>
        public string? Authority { get; set; }

        public string? MetadataAddress { get; set; }

        public bool? RequireHttpsMetadata { get; set; }

        public TimeSpan? BackchannelTimeout { get; set; }

        public TimeSpan? RefreshInterval { get; set; }
    }

    /// <summary>
    /// Settings to be mapped into  <see cref="Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions"/>
    ///Only primitive types properties are available.If you need complex types properties you may add them into the class and adding a mapping rule for them into<see cref="JwtBearerMapping"/>
    /// </summary>
    public record AuthenticationSchemeSettings
    {
        public string? ClaimsIssuer { get; set; }

        public string? ForwardDefault { get; set; }

        public string? ForwardAuthenticate { get; set; }

        public string? ForwardChallenge { get; set; }

        public string? ForwardForbid { get; set; }

        public string? ForwardSignIn { get; set; }

        public string? ForwardSignOut { get; set; }
    }


    /// <summary>
    /// Settings to be mapped into  <see cref="Microsoft.IdentityModel.Tokens.TokenValidationParameters"/>
    /// Only primitive types properties are available. If you need complex types properties you may add them into the class and adding a mapping rule for them into <see cref="JwtBearerMapping"/>
    /// </summary>
    public record TokenValidationParameters
    {
        public bool? TryAllIssuerSigningKeys { get; set; }

        public bool? ValidateIssuer { get; set; }

        public bool? ValidateAudience { get; set; }

        public bool? SaveSigninToken { get; set; }

        public bool? ValidateIssuerSigningKey { get; set; }

        public bool? ValidateLifetime { get; set; }

        public bool? ValidateTokenReplay { get; set; }

        public IEnumerable<string>? ValidAlgorithms { get; set; }

        public string? ValidAudience { get; set; }

        public IEnumerable<string>? ValidAudiences { get; set; }

        public string? ValidIssuer { get; set; }

        public bool? ValidateActor { get; set; }

        public bool? RequireExpirationTime { get; set; }

        public bool? RequireSignedTokens { get; set; }

        public TokenValidationParameters? ActorValidationParameters { get; set; }

        public string? AuthenticationType { get; set; }

        public TimeSpan? ClockSkew { get; set; }

        public bool? IgnoreTrailingSlashWhenValidatingAudience { get; set; }

        public string? RoleClaimType { get; set; }

        public string? NameClaimType { get; set; }

        [Obsolete("use ValidateAudience instead")]
        public bool? RequireAudience { get; set; }

        public IEnumerable<string>? ValidIssuers { get; set; }

        public IEnumerable<string>? ValidTypes { get; set; }
    }
}
