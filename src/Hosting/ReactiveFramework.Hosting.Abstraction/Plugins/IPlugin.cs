using Microsoft.Extensions.DependencyInjection;
using ReactiveFramework.Hosting.Abstraction.Attributes;

namespace ReactiveFramework.Hosting.Abstraction.Plugins;

public interface IPlugin
{
    [InvokedAtAppInitialization]
    void RegisterServices(IServiceCollection services);

    Version GetVersion();

    string GetName();
}