using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;

namespace AlbanianXrm.XrmExtensions
{
    public abstract partial class PluginBase : IPlugin
    {  
        private readonly string pluginName;

        public PluginBase()
        {
            pluginName = GetType().FullName;
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            var context = new Context(serviceProvider);
            try
            {
                Execute(context);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new InvalidPluginExecutionException(
                    ex.Detail.ErrorCode == Constants.IsvCodeAbortedOperation ?
                        ex.Message :
                        $"An error occurred in the {pluginName} plug-in.",
                    ex);
            }
            catch (Exception ex)
            {
                context.TracingService.Trace("{0}: {1}", pluginName, ex.ToString());
                throw;
            }
        }

        protected abstract void Execute(IContext context);
    }
}
