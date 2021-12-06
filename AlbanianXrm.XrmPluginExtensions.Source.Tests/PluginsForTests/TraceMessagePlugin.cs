namespace AlbanianXrm.XrmExtensions.PluginsForTests
{
    public class TraceMessagePlugin : PluginBase
    {
        protected override void Execute(IContext context)
        {
            context.TracingService.Trace("Executing");
            context.TracingService.Trace("Executed");
        }
    }
}
