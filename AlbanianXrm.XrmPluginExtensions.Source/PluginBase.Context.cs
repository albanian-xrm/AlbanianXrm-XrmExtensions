using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace AlbanianXrm.XrmExtensions
{
    public partial class PluginBase
    {
        public class Context : IContext
        {
            private IOrganizationServiceFactory organizationServiceFactory;
            private Dictionary<Guid, IOrganizationService> organizationServices;
            private IOrganizationService organizationServiceSystem;
            private IOrganizationService organizationServiceUser;
            private IPluginExecutionContext pluginExecutionContext;
            private readonly IServiceProvider serviceProvider;
            private Dictionary<Guid, TimeZoneInfo> timeZones;
            private ITracingService tracingService;

            public Context(IServiceProvider serviceProvider)
            {
                this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            }

            public Entity GetImage(string image = Constants.DefaultImage, bool postImage = false)
            {
                var images = postImage ?
                    PluginExecutionContext.PostEntityImages :
                    PluginExecutionContext.PreEntityImages;
                return images.ContainsKey(image) ?
                    images[image] :
                    null;
            }

            public T GetImage<T>(string image = Constants.DefaultImage, bool postImage = false) where T : Entity
            {
                return GetImage(image, postImage)?.ToEntity<T>();
            }

            public IOrganizationService GetOrganizationService()
            {
                if (organizationServiceUser != null)
                {
                    return organizationServiceUser;
                }
                if (pluginExecutionContext == null)
                {
                    pluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                }
                if (organizationServiceFactory == null)
                {
                    organizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                }
                organizationServiceUser = organizationServiceFactory.CreateOrganizationService(pluginExecutionContext.UserId);
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
                if (pluginExecutionContext == null)
                {
                    pluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                }
                if (organizationServiceFactory == null)
                {
                    organizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                }
                if (systemuserid == pluginExecutionContext.UserId)
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
                    organizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                }
                organizationServiceSystem = organizationServiceFactory.CreateOrganizationService(null);
                return organizationServiceSystem;
            }

            public Entity GetTarget()
            {
                return PluginExecutionContext.InputParameters.ContainsKey(Constants.Target) ?
                    PluginExecutionContext.InputParameters[Constants.Target] as Entity :
                    null;
            }

            public T GetTarget<T>() where T : Entity
            {
                return GetTarget()?.ToEntity<T>();
            }

            public EntityReference GetTargetReference()
            {
                return PluginExecutionContext.InputParameters.ContainsKey(Constants.Target) ?
                    PluginExecutionContext.InputParameters[Constants.Target] as EntityReference :
                    null;
            }

            public TimeZoneInfo GetTimeZone(Guid? systemUserId = default)
            {
                if (pluginExecutionContext == null)
                {
                    pluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                }
                var userId = systemUserId ?? pluginExecutionContext.UserId;
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
                    organizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                }
                if (organizationServiceSystem == null)
                {
                    organizationServiceSystem = organizationServiceFactory.CreateOrganizationService(null);
                }
                if (!TryGetSharedVariable(Constants.VAR_TIMEZONES + userId.ToString(), out string timeZoneId))
                {
                    timeZoneId = Queries.TimeZoneDefinitionQueries.GetTimezoneByUserId(userId, organizationServiceSystem);
                    pluginExecutionContext.SharedVariables[Constants.VAR_TIMEZONES + userId] = timeZoneId;
                }
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                timeZones.Add(userId, timeZone);
                return timeZone;
            }

            public IPluginExecutionContext PluginExecutionContext
            {
                get
                {
                    if (pluginExecutionContext == null)
                    {
                        pluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                    }
                    return pluginExecutionContext;
                }
            }

            public ITracingService TracingService
            {
                get
                {
                    if (tracingService == null)
                    {
                        tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
                    }
                    return tracingService;
                }
            }

            public bool TryGetSharedVariable<T>(string key, out T value)
            {
                if (pluginExecutionContext == null)
                {
                    pluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                }
                return TryGetSharedVariable(pluginExecutionContext, key, out value);
            }

            private bool TryGetSharedVariable<T>(IPluginExecutionContext context, string key, out T value)
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
        }
    }
}
