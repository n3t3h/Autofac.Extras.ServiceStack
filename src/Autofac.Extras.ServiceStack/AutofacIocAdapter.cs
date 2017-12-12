using System.Runtime.Remoting.Messaging;
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
            => GetCurrentContext().Resolve<T>();

        public T TryResolve<T>()
        {
            var context = GetCurrentContext();

            return context.TryResolve(out T result) ? result : default(T);
        }

        private ILifetimeScope GetCurrentContext()
            => CallContext.LogicalGetData(Consts.AutofacScopeLogicalContextKey) as ILifetimeScope ?? Container;
    }
}