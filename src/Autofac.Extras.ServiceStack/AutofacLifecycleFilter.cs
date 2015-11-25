using System;
using Autofac.Core.Lifetime;
using Funq;
using ServiceStack.Common;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.WebHost.Endpoints.Support;

namespace Autofac.Extras.ServiceStack
{
    public static class ServiceStackAutofac
    {
        public static void UseAutofac(this AppHostBase appHost, IContainer container)
        {
            appHost.AddAutofacAdapter(container);
            appHost.AddPerRequestScope(container);
        }

        public static void UseAutofac(this HttpListenerBase appHost, IContainer container)
        {
            appHost.AddAutofacAdapter(container);
            appHost.AddPerRequestScope(container);
        }

        public static void AddAutofacAdapter(this IHasContainer appHost, IContainer container)
        {
            appHost.Container.Adapter = new AutofacIocAdapter(container);
        }

        public static void AddPerRequestScope(this IAppHost appHost, IContainer container)
        {
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