using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ReactiveFramework.Hosting.Abstraction;
public interface IPluginApplicationBuilder : IHostBuilder
{
    new IPluginApplication Build();

    IPluginHostEnvironment Environment { get; }

    IServiceCollection InitializationServices { get; }

    IServiceCollection RunTimeServices { get; }

    ConfigurationManager Configuration { get; }

    ILoggingBuilder Logging { get; }
}
