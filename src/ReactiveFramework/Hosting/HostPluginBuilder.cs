using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReactiveFramework.Hosting.Plugins;
using ReactiveFramework.Hosting.Plugins.Internal;

namespace ReactiveFramework.Hosting;
public class HostedPluginAppBuilder : IHostedPluginAppBuilder
{

    private readonly HostApplicationBuilder _hostApplicationBuilder;
    private IPluginManager? _pluginManager;
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
        _pluginManager = options.PluginManager;
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
    public IServiceCollection AppServices => _hostApplicationBuilder.Services;
    public ILoggingBuilder Logging => _hostApplicationBuilder.Logging;
    public IPluginCollection Plugins { get; }
    public IServiceCollection RegistrationServices { get; } = new ServiceCollection();

    public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();

    public virtual IHost Build()
    {
        RegistrationServices.AddSingleton<IConfiguration>(Configuration);

        RegistrationServices.AddSingleton(AppServices);
        RegistrationServices.AddSingleton(Logging);
        RegistrationServices.AddSingleton(Environment);

        _pluginManager ??= new PluginManager(Plugins);
        var registrationServices = RegistrationServices.BuildServiceProvider();

        AppServices.AddSingleton(Plugins);
        AppServices.AddSingleton(_pluginManager);
        AppServices.AddSingleton(RegistrationServices);


        _pluginManager.RegisterPlugins(registrationServices);
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
        configureDelegate(_hostBuilderContext, AppServices);
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
