namespace AlbanianXrm.XrmExtensions.WorkflowsForTests
{
    public class TraceMessageAction : WorkflowBase
    {
        protected override void Execute(IContext context)
        {
            context.TracingService.Trace("Executing");
            context.TracingService.Trace("Executed");
        }
    }
}
