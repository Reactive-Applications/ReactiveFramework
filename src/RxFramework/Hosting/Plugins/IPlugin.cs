using Microsoft.Extensions.DependencyInjection;
using RxFramework.Hosting.Plugins.Attributes;

namespace RxFramework.Hosting.Plugins;

public interface IPlugin
{
    [InvokedAtPluginRegistration]
    void RegisterServices(IServiceCollection services);

    Version GetVersion();

    string GetName();
}