using Microsoft.Xrm.Sdk;

namespace AlbanianXrm.XrmExtensions
{
    public partial class PluginBase
    {
        public interface IContext : IContextBase
        {
            Entity GetImage(string image = Constants.DefaultImage, bool postImage = false);
            T GetImage<T>(string image = Constants.DefaultImage, bool postImage = false) where T : Entity;
            IPluginExecutionContext PluginExecutionContext { get; }
        }
    }
}
