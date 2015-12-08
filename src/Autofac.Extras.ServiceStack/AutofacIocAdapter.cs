using ServiceStack;
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
            return !HostContext.RequestContext.Items.Contains("AutofacScope")
                ? Container.Resolve<T>()
                : ((ILifetimeScope) HostContext.RequestContext.Items["AutofacScope"]).Resolve<T>();
        }

        public T TryResolve<T>()
        {
            var context = HostContext.RequestContext.Items.Contains("AutofacScope")
                ? (ILifetimeScope) HostContext.RequestContext.Items["AutofacScope"]
                : Container;

            T result;
            return context.TryResolve(out result) ? result : default(T);
        }
    }
}