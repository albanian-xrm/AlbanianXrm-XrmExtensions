using System.Activities;

namespace AlbanianXrm.XrmExtensions.WorkflowsForTests
{
    public class UserTimezoneAction : WorkflowBase
    {
        public OutArgument<string> UserTimeZone { get; set; }
     
        protected override void Execute(IContext context)
        {
            var userTimezone = context.GetTimeZone();
            context.Arguments.SetValue(UserTimeZone, userTimezone.StandardName);
        }
    }
}
