using System.Collections.Generic;
using Telemedicine.Common.Contracts.GlobalContracts.Exceptions;
using Telemedicine.Common.Infrastructure.CommonInfrastructure.Utilities;
using Telemedicine.Common.ServiceInfrastructure.ExceptionHandling.Service.Settings;

namespace Telemedicine.Common.ServiceInfrastructure.ExceptionHandling.Service.Utilities
{
    public static class ExceptionHandlingUtility
    {
        public static bool IsExceptionToLogAsInfo(IReadOnlyCollection<InfoLogExceptionFilterSettings> exceptionFilterSettings, string httpVerb, string path, ServiceLayerException exception)
        {
            //Checking the path
            foreach (InfoLogExceptionFilterSettings filter in exceptionFilterSettings)
            {
                //if the HttpVerb is matched or equal to includeAll, then check the specified path
                if (IsMatchingHttpVerb(filter, httpVerb))
                {
                    //if the Path is matched, then we need to check the specified exception type
                    if (IsMatchingPath(filter, path))
                    {
                        //if the Path is matched and exception type equal to include all, then we skip the exception codes matching
                        if (IsIncludeAll(filter.Type))
                        { return true; }

                        //If we match the exception type then we need to check the specified exception code as well
                        if (IsMatchingType(filter, exception))
                        {
                            if (IsMatchingCode(filter, exception))
                            { return true; }
                        }
                    }
                }
            }

            return false;
        }

        private static bool IsMatchingHttpVerb(InfoLogExceptionFilterSettings filter, string requestHttpVerb)
        {
            return IsMatchingOrEqualToIncludeAll(filter.HttpVerb, requestHttpVerb);
        }

        private static bool IsMatchingPath(InfoLogExceptionFilterSettings filter, string requestPath)
        {
            return IsMatchingOrEqualToIncludeAll(filter.Path, requestPath);
        }

        private static bool IsMatchingType(InfoLogExceptionFilterSettings filter, ServiceLayerException ex)
        {
            return IsMatchingOrEqualToIncludeAll(filter.Type, ex.GetType().FullName);
        }

        private static bool IsMatchingCode(InfoLogExceptionFilterSettings filter, ServiceLayerException ex)
        {
            return IsMatchingOrEqualToIncludeAll(filter.Code, ex.Code);
        }

        private static bool IsMatchingOrEqualToIncludeAll(string? filterValue, string? requestValue)
        {
            return IsIncludeAll(filterValue) || filterValue!.ToLowerAndTrim() == requestValue?.ToLower();
        }

        private static bool IsIncludeAll(string? value) => string.IsNullOrWhiteSpace(value);
    }
}