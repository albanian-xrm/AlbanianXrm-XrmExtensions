using System.Activities;

namespace AlbanianXrm.XrmExtensions
{
    public partial class WorkflowBase
    {
        private sealed class Arguments : IArguments
        {
            private readonly CodeActivityContext codeActivityContext;

            public Arguments(CodeActivityContext codeActivityContext)
            {
                this.codeActivityContext = codeActivityContext;
            }

            public T GetValue<T>(InArgument<T> @in)
            {
                return @in.Get(codeActivityContext);
            }

            public T GetValue<T>(InOutArgument<T> @in)
            {
                return @in.Get(codeActivityContext);
            }

            public void SetValue<T>(InOutArgument<T> @out, T value)
            {
                @out.Set(codeActivityContext, value);
            }

            public void SetValue<T>(OutArgument<T> @out, T value)
            {
                @out.Set(codeActivityContext, value);
            }
        }
    }
}
