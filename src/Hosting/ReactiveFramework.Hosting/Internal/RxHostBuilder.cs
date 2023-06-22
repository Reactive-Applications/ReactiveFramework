using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using ReactiveFramework.Hosting.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveFramework.Hosting.Internal;
internal partial class RxHostBuilder : IRxHostBuilder
{
    private readonly RxHostBuilderSettings _settings;
    private readonly HostBuilderContext _context;

    private Func<IServiceCollection, IServiceProvider> _createServiceProvider;
    private Action<object> _configureContainer = _ => { };

    private bool _hostBuilded;

    public IHostEnvironment Environment { get; }
    public IServiceCollection Services { get; }
    public ConfigurationManager Configuration { get; }
    public ILoggingBuilder Logging { get; }
    public IDictionary<object, object> Properties { get; }

    public RxHostBuilder(RxHostBuilderSettings settings)
    {
        _settings = settings;
        Configuration = settings.Configuration;
        Configuration ??= new();
        Services = new ServiceCollection();
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

        Logging = new LoggingBuilder(Services);

        _context = new(Properties)
        {
            HostingEnvironment = env,
            Configuration = Configuration
        };

        ServiceProviderOptions? providerOptions = null;
        PopulateServices();

        if (!settings.DisableDefaults)
        {
            HostBuilderExtensions.ApplyDefaultAppConfiguration(_context, Configuration, settings.Args);
            HostBuilderExtensions.AddDefaultLogging(_context, Logging);
            providerOptions = HostBuilderExtensions.CreateDefaultServiceProviderOptions(_context);
        }

        _createServiceProvider = services =>
        {
            _configureContainer(services);
            return providerOptions is null ? services.BuildServiceProvider() : services.BuildServiceProvider(providerOptions);
        };

    }

    public IHost Build()
    {
        if (_hostBuilded)
        {
            throw new InvalidOperationException("build method already called before");
        }
        _hostBuilded = true;

        var serviceProvider = Services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<IHost>();
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

    public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
    {
        configureDelegate(_context, Configuration);
        return this;
    }

    public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
    {
        _configureContainer = builder => configureDelegate(_context, (TContainerBuilder)builder);
        return this;
    }

    public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
    {
        configureDelegate(Configuration);
        return this;
    }

    public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
    {
        configureDelegate(_context, Services);
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


    protected virtual void PopulateServices()
    {
        Services.AddSingleton(Environment);
        Services.AddSingleton<IConfiguration>(Configuration);
        Services.AddSingleton<IConfigurationBuilder>(Configuration);
        Services.AddSingleton<IHostApplicationLifetime, ApplicationLifetime>();
        Services.AddSingleton<IHost, RxHost>();
    }

    private sealed record LoggingBuilder(IServiceCollection Services) : ILoggingBuilder;
}
