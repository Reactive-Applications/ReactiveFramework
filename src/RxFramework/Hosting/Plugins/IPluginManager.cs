using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RxFramework.Hosting.Plugins;

public interface IPluginManager : IDisposable
{
    bool AutoInitialize { get; set; }

    IObservable<PluginDescription> WhenPluginLoading { get; }
    IObservable<PluginDescription> WhenPluginLoaded { get; }

    IEnumerable<PluginDescription> LoadedPlugins { get; }
    IPluginInitializerCollection PluginInitializers { get; }
    void RegisterServices(IServiceCollection services);

    void InitializePlugins(IServiceProvider services);
}
