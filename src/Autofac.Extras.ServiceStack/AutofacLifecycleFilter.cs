using System;
using Autofac.Core.Lifetime;
using ServiceStack;

namespace Autofac.Extras.ServiceStack
{
    public static class ServiceStackAutofac
    {
        public static ServiceStackHost UseAutofac(this ServiceStackHost appHost, IContainer container)
        {
            appHost.Container.Adapter = new AutofacIocAdapter(container);

            appHost.GlobalRequestFilters.Add((req, resp, dto) => CreateScope(container));
            appHost.GlobalResponseFilters.Add((req, resp, dto) => DisposeScope());

            appHost.GlobalMessageRequestFilters.Add((req, resp, dto) => CreateScope(container));
            appHost.GlobalMessageRequestFilters.Add((req, resp, dto) => DisposeScope());
            
            return appHost;
        }

        private static void CreateScope(IContainer container)
        {
            var scope = container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
            HostContext.RequestContext.Items["AutofacScope"] = scope;
        }

        private static void DisposeScope()
        {
            var scope = HostContext.RequestContext.Items["AutofacScope"] as IDisposable;scope?.Dispose();
        }
    }
}