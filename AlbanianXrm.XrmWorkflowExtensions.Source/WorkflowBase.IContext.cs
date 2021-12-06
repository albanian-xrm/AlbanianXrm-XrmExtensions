using AlbanianXrm.XrmExtensions;
using Microsoft.Xrm.Sdk.Workflow;

namespace AlbanianXrm.XrmExtensions
{
    public partial class WorkflowBase
    {
        public interface IContext : IContextBase
        {
            IArguments Arguments { get; }
            IWorkflowContext WorkflowContext { get; }
        }
    }
}
