using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RxFramework.Hosting.Plugins;
public interface IPlugin
{
    static abstract void RegisterServices(IServiceCollection services);

    void Initialize();
}
