using ServiceStack.Common;
using ServiceStack.Configuration;

namespace Autofac.Extras.ServiceStack
{
    public class AutofacIocAdapter : IContainerAdapter
    {
        public AutofacIocAdapter(IContainer container)
        {
            Container = container;
        }

        public IContainer Container { get; }

        public T Resolve<T>()
        {
            return !HostContext.Instance.Items.Contains("AutofacScope")
                ? Container.Resolve<T>()
                : ((ILifetimeScope) HostContext.Instance.Items["AutofacScope"]).Resolve<T>();
        }

        public T TryResolve<T>()
        {
            T result;
            return Container.TryResolve(out result) ? result : default(T);
        }
    }
}