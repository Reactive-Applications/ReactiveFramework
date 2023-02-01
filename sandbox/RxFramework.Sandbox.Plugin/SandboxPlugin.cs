using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RxFramework.Hosting.Plugins;
using RxFramework.Hosting.Plugins.Attributes;

namespace RxFramework.Sandbox.TestPlugin;
public class SandboxPlugin : Plugin
{
    public override void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IService, Service>();
    }

    [InvokedAtPluginRegistration]
    public void ConfigureConfiguration(IConfiguration configuration, IServiceCollection services)
    {
    }

    [InvokedAtPluginInitialization]
    public void Initialize(IService testService)
    {
        testService.WriteToConsole("Initialisiert");
    }
}
