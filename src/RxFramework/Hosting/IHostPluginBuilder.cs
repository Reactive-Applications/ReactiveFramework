using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RxFramework.Hosting.Plugins;

namespace RxFramework.Hosting;
public interface IHostedPluginAppBuilder : IHostBuilder
{
	public IHostEnvironment Environment { get; }

	public ConfigurationManager Configuration { get; }

	public IServiceCollection AppServices { get; }

	public ILoggingBuilder Logging { get; }

	public IPluginCollection Plugins { get; }

	public IServiceCollection RegistrationServices { get; }
}
