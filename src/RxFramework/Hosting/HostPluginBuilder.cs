using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RxFramework.Hosting.Plugins;
using RxFramework.Hosting.Plugins.Internal;

namespace RxFramework.Hosting;
public class HostedPluginAppBuilder : IHostedPluginAppBuilder
{

    private readonly HostApplicationBuilder _hostApplicationBuilder;
    private readonly IPluginManager _pluginManager;
    private readonly HostBuilderContext _hostBuilderContext;
    private readonly List<Action<object>> _configureContainerActions = new();

    public HostedPluginAppBuilder()
        : this(args: null)
    {

    }

    public HostedPluginAppBuilder(string[]? args)
        : this(new PluginAppBuilderOptions { Args = args })
    {

    }

    public HostedPluginAppBuilder(PluginAppBuilderOptions options)
    {
        Configuration = options.Configuration ?? new ConfigurationManager();
        Plugins = options.PluginCollection ?? new PluginCollection();
        PluginInitializers = options.PluginInitializers ?? new PluginInitializerCollection();
        _pluginManager = options.PluginManager ?? new PluginManager(Plugins, PluginInitializers);
        _hostApplicationBuilder = new HostApplicationBuilder(new HostApplicationBuilderSettings
        {
            Args = options.Args,
            ApplicationName = options.ApplicationName,
            EnvironmentName = options.EnvironmentName,
            ContentRootPath = options.ContentRootPath,
            Configuration = Configuration,
        });

        _hostBuilderContext = new HostBuilderContext(Properties)
        {
            HostingEnvironment = Environment,
            Configuration = Configuration,
        };
    }

    public IHostEnvironment Environment => _hostApplicationBuilder.Environment;
    public ConfigurationManager Configuration { get; }
    public IServiceCollection Services => _hostApplicationBuilder.Services;
    public ILoggingBuilder Logging => _hostApplicationBuilder.Logging;
    public IPluginCollection Plugins { get; }
    public IPluginInitializerCollection PluginInitializers { get; }

    public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();

    public virtual IHost Build()
    {
        Services.AddSingleton(Plugins);
        Services.AddSingleton(_pluginManager);
        Services.AddSingleton(PluginInitializers);
        _pluginManager.RegisterServices(Services);
        return _hostApplicationBuilder.Build();
    }

    public virtual IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
    {
        configureDelegate(_hostBuilderContext, Configuration);
        return this;
    }

    public virtual IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
    {
        _configureContainerActions.Add(b => configureDelegate(_hostBuilderContext, (TContainerBuilder)b));
        return this;
    }

    public virtual IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
    {
        configureDelegate(Configuration);
        return this;
    }

    public virtual IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
    {
        configureDelegate(_hostBuilderContext, Services);
        return this;
    }

    public virtual IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) where TContainerBuilder : notnull
    {
        _hostApplicationBuilder.ConfigureContainer(factory, b =>
        {
            foreach (var action in _configureContainerActions)
            {
                action(b);
            }
        });
        return this;
    }

    public virtual IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory) where TContainerBuilder : notnull
    {
        UseServiceProviderFactory(factory(_hostBuilderContext));
        return this;
    }
}
