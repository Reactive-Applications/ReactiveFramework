using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using ReactiveFramework.Hosting.Abstraction;
using ReactiveFramework.Hosting.Abstraction.Plugins;
using ReactiveFramework.Hosting.Internal;
using System.Diagnostics;

namespace ReactiveFramework.Hosting;
public class PluginApplicationBuilder : IPluginApplicationBuilder
{
    private readonly PluginHostBuilderContext _context;
    private readonly ServiceCollection _serviceCollection;
    private readonly ServiceCollection _runTimeServices;
    private readonly PluginApplicationBuilderSettings _settings;

    private Func<IServiceCollection, IServiceProvider> _createServiceProvider;
    private Action<object> _configureContainer = _ => { };

    private bool _hostBuild;

    public IPluginHostEnvironment Environment { get; }
    public IServiceCollection InitializationServices => _serviceCollection;
    public IServiceCollection RunTimeServices => _runTimeServices;
    public ConfigurationManager Configuration { get; }
    public ILoggingBuilder Logging { get; }

    public IDictionary<object, object> Properties { get; }

    internal PluginApplicationBuilder(PluginApplicationBuilderSettings settings)
    {
        Configuration = settings.Configuration ??= new();
        _serviceCollection = new ServiceCollection();
        _settings = settings;
        _runTimeServices = new ServiceCollection();
        Properties = new Dictionary<object, object>();

        if (!settings.DisableDefaults)
        {
            if (settings.ContentRootPath is null && Configuration[HostDefaults.ContentRootKey] is null)
            {
                HostBuilderExtensions.SetDefaultContentRoot(Configuration);
            }

            HostBuilderExtensions.AddDefaultConfigurationSources(Configuration, settings.Args);
        }

        List<KeyValuePair<string, string?>> options = new();

        if (settings.ApplicationName is not null)
        {
            options.Add(new(HostDefaults.ApplicationKey, settings.ApplicationName));
        }

        if (settings.EnvironmentName is not null)
        {
            options.Add(new(HostDefaults.EnvironmentKey, settings.EnvironmentName));
        }

        if (settings.ContentRootPath is not null)
        {
            options.Add(new(HostDefaults.ContentRootKey, settings.ContentRootPath));
        }

        Configuration.AddInMemoryCollection(options);

        var (env, fileProvider) = HostBuilderExtensions.CreateHostEnvironment(Configuration);
        Environment = env;
        Configuration.SetFileProvider(fileProvider);

        Logging = new LoggingBuilder();

        _context = new(Properties, Logging)
        {
            HostingEnvironment = env,
            Configuration = Configuration,
            DisableDefaults = settings.DisableDefaults,
            AutoInitialize = settings.AutoInitialize,
        };

        ConfigureServices();

        ServiceProviderOptions? providerOptions = null;

        if (!settings.DisableDefaults)
        {
            HostBuilderExtensions.ApplyDefaultAppConfiguration(_context, Configuration, settings.Args);
            HostBuilderExtensions.AddDefaultLogging(_context, Logging);
            RegisterDefaultInitializationServices();
            providerOptions = HostBuilderExtensions.CreateDefaultServiceProviderOptions(_context);
        }

        _createServiceProvider = services =>
        {
            _configureContainer(services);
            return providerOptions is null ? services.BuildServiceProvider() : services.BuildServiceProvider(providerOptions);
        };
    }

    protected virtual void ConfigureServices()
    {
        InitializationServices.AddSingleton<IHostEnvironment>(Environment);
        InitializationServices.AddSingleton<IHostApplicationLifetime, ApplicationLifetime>();
        InitializationServices.AddSingleton<IHostLifetime, ConsoleLifetime>();
    }

    protected virtual void RegisterDefaultInitializationServices()
    {
        InitializationServices.TryAddSingleton(Configuration);
        InitializationServices.TryAddSingleton<IServiceCollection, ServiceCollection>();
        InitializationServices.TryAddSingleton<IPluginManager, PluginManager>();
        InitializationServices.TryAddSingleton<IPluginCollection, PluginCollection>();
        InitializationServices.TryAddSingleton<ILoggingBuilder>(provider =>
        {
            var services = provider.GetRequiredService<IServiceCollection>();
            if (!_settings.DisableDefaults)
            {
                services.Add(Logging.Services);
            }

            return new LoggingBuilder(services);
        });
    }

    public virtual IPluginApplication Build()
    {
        if (_hostBuild)
        {
            throw new InvalidOperationException("build method already called before");
        }

        _hostBuild = true;

        foreach (var loggingService in Logging.Services)
        {
            InitializationServices.Add(loggingService);
        }

        InitializationServices.AddSingleton(_configureContainer);
        InitializationServices.AddSingleton(_createServiceProvider);

        var services = _createServiceProvider(InitializationServices);
        _serviceCollection.MakeReadOnly();

        var runtimeServices = services.GetRequiredService<IServiceCollection>();

        runtimeServices.Add(RunTimeServices);

        return new PluginApplication(_context, services);
    }

    public void ConfigureServiceProvider<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder>? configure = null) where TContainerBuilder : notnull
    {
        _createServiceProvider = services =>
        {
            var builder = factory.CreateBuilder(services);
            _configureContainer(builder);
            return factory.CreateServiceProvider(builder);
        };
        _configureContainer = builder => configure?.Invoke((TContainerBuilder)builder);
    }

    public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
    {
        configureDelegate(Configuration);
        return this;
    }

    public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
    {
        configureDelegate(_context, Configuration);
        return this;
    }

    public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
    {
        configureDelegate(_context, RunTimeServices);
        return this;
    }

    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) where TContainerBuilder : notnull
    {
        _createServiceProvider = services =>
        {
            var builder = factory.CreateBuilder(services);
            _configureContainer(builder);
            return factory.CreateServiceProvider(builder);
        };

        return this;
    }

    public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory) where TContainerBuilder : notnull
        => UseServiceProviderFactory(factory(_context));

    public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
    {
        _configureContainer = builder => configureDelegate(_context, (TContainerBuilder)builder);
        return this;
    }

    IHost IHostBuilder.Build()
        => Build();

    private sealed class LoggingBuilder : ILoggingBuilder
    {
        public IServiceCollection Services { get; }

        public LoggingBuilder()
        {
            Services = new ServiceCollection();
        }

        public LoggingBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }

    private sealed class ServiceProviderFactoryAdapter<TContainerBuilder> : IServiceProviderFactory<object> where TContainerBuilder : notnull
    {
        private IServiceProviderFactory<TContainerBuilder>? _serviceProviderFactory;

        private readonly Func<HostBuilderContext>? _contextResolver;
        private Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>>? _factoryResolver;

        public ServiceProviderFactoryAdapter(IServiceProviderFactory<TContainerBuilder> serviceProviderFactory)
        {
            _serviceProviderFactory = serviceProviderFactory;
        }

        public ServiceProviderFactoryAdapter(Func<HostBuilderContext> contextResolver, Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factoryResolver)
        {
            _contextResolver = contextResolver;
            _factoryResolver = factoryResolver;
        }

        public object CreateBuilder(IServiceCollection services)
        {
            if (_serviceProviderFactory == null)
            {
                Debug.Assert(_factoryResolver != null && _contextResolver != null);
                _serviceProviderFactory = _factoryResolver(_contextResolver());

                if (_serviceProviderFactory == null)
                {
                    throw new InvalidOperationException("service provider factory returns null");
                }
            }
            return _serviceProviderFactory.CreateBuilder(services);
        }
        public IServiceProvider CreateServiceProvider(object containerBuilder)
        {
            if (_serviceProviderFactory == null)
            {
                throw new InvalidOperationException("call CreateBuilder first");
            }

            return _serviceProviderFactory.CreateServiceProvider((TContainerBuilder)containerBuilder);
        }
    }
}
