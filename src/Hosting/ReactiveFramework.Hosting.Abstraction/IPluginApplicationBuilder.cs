using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ReactiveFramework.Hosting.Abstraction;
public interface IPluginApplicationBuilder : IHostBuilder
{
    IPluginApplication Build();

    IPluginHostEnvironment Environment { get; }

    IServiceCollection InitializationServices { get; }

    ConfigurationManager Configuration { get; }

    ILoggingBuilder Logging { get; }
}
