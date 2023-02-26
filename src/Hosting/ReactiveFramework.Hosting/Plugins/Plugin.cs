using Microsoft.Extensions.DependencyInjection;
using ReactiveFramework.Hosting.Plugins.Attributes;

namespace ReactiveFramework.Hosting.Plugins;
public abstract class Plugin : IPlugin
{
    public virtual void RegisterServices(IServiceCollection services) { }

    public virtual Version GetVersion()
    {
        return GetType().Assembly.GetName().Version ?? new();
    }

    public virtual string GetName()
    {
        return GetType().FullName ?? ToString()!;
    }
}
