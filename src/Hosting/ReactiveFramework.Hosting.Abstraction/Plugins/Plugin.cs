using Microsoft.Extensions.DependencyInjection;

namespace ReactiveFramework.Hosting.Abstraction.Plugins;
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
