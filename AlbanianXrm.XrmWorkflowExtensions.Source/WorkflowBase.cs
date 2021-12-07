using Microsoft.Xrm.Sdk;
using System;
using System.Activities;
using System.ServiceModel;

namespace AlbanianXrm.XrmExtensions
{
    public abstract partial class WorkflowBase : CodeActivity
    {
        private readonly string workflowName;
        public const string Target = "Target";

        protected WorkflowBase()
        {
            workflowName = GetType().Name;
        }

        protected override void Execute(CodeActivityContext context)
        {
            var codeActivityContext = new Context(context);
            try
            {
                Execute(codeActivityContext);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new InvalidPluginExecutionException($"An error occurred in the {workflowName} workflow step.", ex);
            }
            catch (Exception ex)
            {
                codeActivityContext.TracingService.Trace("{0}: {1}", workflowName, ex.ToString());
                throw;
            }
        }

        protected abstract void Execute(IContext context);
    }
}
