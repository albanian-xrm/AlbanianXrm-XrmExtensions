namespace AlbanianXrm.XrmExtensions.PluginsForTests
{
    public class UserTimezonePlugin : PluginBase
    {
        protected override void Execute(IContext context)
        {
            var userTimezone = context.GetTimeZone();
            context.TracingService.Trace("User Time Zone: {0}", userTimezone.StandardName);
        }
    }
}
