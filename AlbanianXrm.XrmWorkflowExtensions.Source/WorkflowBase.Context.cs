using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;

namespace AlbanianXrm.XrmExtensions
{
    public partial class WorkflowBase
    {
        public class Context : IContext
        {
            private IArguments arguments;
            private readonly CodeActivityContext codeActivityContext;
            private IOrganizationServiceFactory organizationServiceFactory;
            private Dictionary<Guid, IOrganizationService> organizationServices;
            private IOrganizationService organizationServiceSystem;
            private IOrganizationService organizationServiceUser;
            private ITracingService tracingService;
            private Dictionary<Guid, TimeZoneInfo> timeZones;
            private IWorkflowContext workflowContext;

            public Context(CodeActivityContext codeActivityContext)
            {
                this.codeActivityContext = codeActivityContext ?? throw new ArgumentNullException(nameof(codeActivityContext));
            }

            public IArguments Arguments
            {
                get
                {
                    if (arguments == null)
                    {
                        arguments = new Arguments(codeActivityContext);
                    }
                    return arguments;
                }
            }

            public IOrganizationService GetOrganizationService()
            {
                if (organizationServiceUser != null)
                {
                    return organizationServiceUser;
                }
                if (workflowContext == null)
                {
                    workflowContext = codeActivityContext.GetExtension<IWorkflowContext>();
                }
                if (organizationServiceFactory == null)
                {
                    organizationServiceFactory = codeActivityContext.GetExtension<IOrganizationServiceFactory>();
                }
                organizationServiceUser = organizationServiceFactory.CreateOrganizationService(workflowContext.UserId);
                return organizationServiceUser;
            }

            public IOrganizationService GetOrganizationService(Guid systemuserid)
            {
                IOrganizationService organizationService;
                if (organizationServices == null)
                {
                    organizationServices = new Dictionary<Guid, IOrganizationService>();
                }
                else if (organizationServices.TryGetValue(systemuserid, out organizationService))
                {
                    return organizationService;
                }
                if (workflowContext == null)
                {
                    workflowContext = codeActivityContext.GetExtension<IWorkflowContext>();
                }
                if (organizationServiceFactory == null)
                {
                    organizationServiceFactory = codeActivityContext.GetExtension<IOrganizationServiceFactory>();
                }
                if (systemuserid == workflowContext.UserId)
                {
                    if (organizationServiceUser == null)
                    {
                        organizationServiceUser = organizationServiceFactory.CreateOrganizationService(systemuserid);
                    }
                    organizationService = organizationServiceUser;
                }
                else
                {
                    organizationService = organizationServiceFactory.CreateOrganizationService(systemuserid);
                }

                organizationServices.Add(systemuserid, organizationService);
                return organizationService;
            }

            public IOrganizationService GetOrganizationServiceSystem()
            {
                if (organizationServiceSystem != null)
                {
                    return organizationServiceSystem;
                }
                if (organizationServiceFactory == null)
                {
                    organizationServiceFactory = codeActivityContext.GetExtension<IOrganizationServiceFactory>();
                }
                organizationServiceSystem = organizationServiceFactory.CreateOrganizationService(null);
                return organizationServiceSystem;
            }

            public Entity GetTarget()
            {
                return WorkflowContext.InputParameters.ContainsKey(Target) ?
                    WorkflowContext.InputParameters[Target] as Entity :
                    null;
            }

            public T GetTarget<T>() where T : Entity
            {
                return GetTarget()?.ToEntity<T>();
            }

            public EntityReference GetTargetReference()
            {
                return WorkflowContext.InputParameters.ContainsKey(Target) ?
                    WorkflowContext.InputParameters[Target] as EntityReference :
                    null;
            }

            public TimeZoneInfo GetTimeZone(Guid? systemUserId = default)
            {
                if (workflowContext == null)
                {
                    workflowContext = codeActivityContext.GetExtension<IWorkflowContext>();
                }
                var userId = systemUserId ?? workflowContext.UserId;
                if (timeZones == null)
                {
                    timeZones = new Dictionary<Guid, TimeZoneInfo>();
                }
                if (timeZones.TryGetValue(userId, out TimeZoneInfo timeZone))
                {
                    return timeZone;
                }
                if (organizationServiceFactory == null)
                {
                    organizationServiceFactory = codeActivityContext.GetExtension<IOrganizationServiceFactory>();
                }
                if (organizationServiceSystem == null)
                {
                    organizationServiceSystem = organizationServiceFactory.CreateOrganizationService(null);
                }
                if (!TryGetSharedVariable(Constants.VAR_TIMEZONES + userId.ToString(), out string timeZoneId))
                {
                    timeZoneId = Queries.TimeZoneDefinitionQueries.GetTimezoneByUserId(userId, organizationServiceSystem);
                    workflowContext.SharedVariables[Constants.VAR_TIMEZONES] = timeZoneId;
                }
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                timeZones.Add(userId, timeZone);
                return timeZone;
            }

            public ITracingService TracingService
            {
                get
                {
                    if (tracingService == null)
                    {
                        tracingService = codeActivityContext.GetExtension<ITracingService>();
                    }
                    return tracingService;
                }
            }

            public bool TryGetSharedVariable<T>(string key, out T value)
            {
                if (workflowContext == null)
                {
                    workflowContext = codeActivityContext.GetExtension<IWorkflowContext>();
                }
                return TryGetSharedVariable(workflowContext, key, out value);
            }

            private bool TryGetSharedVariable<T>(IWorkflowContext context, string key, out T value)
            {
                if (context == null)
                {
                    value = default;
                    return false;
                }
                if (context.SharedVariables.TryGetValue(key, out object objectValue))
                {
                    value = (T)objectValue;
                    return true;
                }
                return TryGetSharedVariable(context.ParentContext, key, out value);
            }

            public IWorkflowContext WorkflowContext
            {
                get
                {
                    if (workflowContext == null)
                    {
                        workflowContext = codeActivityContext.GetExtension<IWorkflowContext>();
                    }
                    return workflowContext;
                }
            }
        }
    }
}
