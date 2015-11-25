# Autofac.Extras.ServiceStack

Autofac container adapter for ServiceStack with per-request scope

Usage:
```c#
var container = builder.Build();

var appHost = new MyAppHost();
appHost.UseAutofac(container);
```
