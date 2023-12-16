using System;
using System.Collections.Generic;
using AutoMapper;
using Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
// ReSharper disable UnusedParameter.Local

namespace Telemedicine.Common.ServiceInfrastructure.ServiceStartup.Utilities.Authorization
{
    internal class JwtBearerSettingsMappingProfile : Profile
    {
        public JwtBearerSettingsMappingProfile()
        {
            //TechDebt: maybe, these mapping lines should be moved to a base class. They are already used in multiple places (like SqsSettings) in a similar way. But you need to think of it carefully because the mapping profiles are unique for a particular Setting and moving it to a base class might be redundant or even cause unexpected behaviour.
            /* we need these mappings to preserve source value if the destination value == null, otherwise it will be set to default: 0, false */
            CreateMap<int?, int>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<long?, long>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<bool?, bool>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<TimeSpan?, TimeSpan>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<string?, string>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<IEnumerable<string>?, IEnumerable<string>>().ConvertUsing((src, dest) => src ?? dest); //This is also required because autoMapper will set an empty collection instead of null 

            CreateMap<JwtBearerSettings, JwtBearerOptions>()
                .ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null)); //For reference types properties, we want to ignore null values
            CreateMap<TokenValidationParameters, Microsoft.IdentityModel.Tokens.TokenValidationParameters>()
                .ForAllMembers(opt => opt.Condition((src, dest, sourceMember) => sourceMember != null)); //For reference types properties, we want to ignore null values
        }
    }
}
