using Microsoft.Xrm.Sdk;
using System;

namespace AlbanianXrm.XrmExtensions
{
    public interface IContextBase
    {
        IOrganizationService GetOrganizationService();
        IOrganizationService GetOrganizationService(Guid systemuserid);
        IOrganizationService GetOrganizationServiceSystem();
        Entity GetTarget();
        T GetTarget<T>() where T : Entity;
        EntityReference GetTargetReference();
        TimeZoneInfo GetTimeZone(Guid? systemUserId = default);
        ITracingService TracingService { get; }
        bool TryGetSharedVariable<T>(string key, out T value);
    }
}
