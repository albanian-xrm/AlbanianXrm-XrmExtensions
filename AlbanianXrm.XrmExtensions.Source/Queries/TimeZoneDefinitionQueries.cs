using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;

namespace AlbanianXrm.XrmExtensions.Queries
{
    public static class TimeZoneDefinitionQueries
    {
        public const string USER_DOES_NOT_EXIST = "User with Id='{0}' does not exist";
        public static string GetTimezoneByUserId(Guid userId, IOrganizationService service)
        {
            var query = new QueryExpression("timezonedefinition")
            {
                NoLock = true,
                TopCount = 1,
                ColumnSet = new ColumnSet("standardname")
            };
            query.AddLink("usersettings", "timezonecode", "timezonecode")
                 .LinkCriteria
                 .AddCondition("systemuserid", ConditionOperator.Equal, userId);
            var result = service.RetrieveMultiple(query).Entities.FirstOrDefault();
            if (result == null)
            {
                throw new InvalidPluginExecutionException(string.Format(USER_DOES_NOT_EXIST, userId));
            }
            return result.GetAttributeValue<string>("standardname");
        }
    }
}
