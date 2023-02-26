using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReactiveFramework.Hosting.Plugins;

namespace ReactiveFramework.WPF.Hosting;

public interface IWPFAppBuilder
{
    IServiceCollection InitializationServices { get; }
    
    IServiceCollection RuntimeServices { get; }

    ILoggingBuilder Logging { get; }

    ConfigurationManager Configuration { get; }

    IHostEnvironment Environment { get; }

    IPluginCollection Plugins { get; }

    new IWpfHost Build();
}