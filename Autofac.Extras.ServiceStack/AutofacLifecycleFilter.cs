using System;
using Autofac.Core.Lifetime;
using ServiceStack.Common;
using ServiceStack.WebHost.Endpoints;

namespace Autofac.Extras.ServiceStack
{
    public static class ServiceStackAutofac
    {
        public static void UseAutofac(this AppHostBase appHost, IContainer container)
        {
            appHost.Container.Adapter = new AutofacIocAdapter(container);
            appHost.RequestFilters.Add((req, resp, dto) =>
            {
                var scope = container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
                HostContext.Instance.Items["AutofacScope"] = scope;
            });
            appHost.ResponseFilters.Add((req, resp, dto) =>
            {
                var scope = HostContext.Instance.Items["AutofacScope"] as IDisposable;
                scope?.Dispose();
            });
        }
    }
}