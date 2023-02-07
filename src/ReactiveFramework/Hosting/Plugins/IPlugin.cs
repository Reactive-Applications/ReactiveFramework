using Microsoft.Extensions.DependencyInjection;
using ReactiveFramework.Hosting.Plugins.Attributes;

namespace ReactiveFramework.Hosting.Plugins;

public interface IPlugin
{
    [InvokedAtPluginRegistration]
    void RegisterServices(IServiceCollection services);

    Version GetVersion();

    string GetName();
}