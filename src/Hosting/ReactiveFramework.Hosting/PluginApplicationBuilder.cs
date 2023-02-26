using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReactiveFramework.Hosting.Abstraction;
using ReactiveFramework.Hosting.Abstraction.Plugins;
using ReactiveFramework.Hosting.Internal;
using ReactiveFramework.Hosting.Plugins;

namespace ReactiveFramework.Hosting;
public class PluginApplicationBuilder : IPluginApplicationBuilder
{
    private readonly HostApplicationBuilder _hostApplicationBuilder;
    private readonly PluginHostBuilderContext _pluginHostContext;
    private readonly List<Action<PluginHostBuilderContext, object>> _configureContainerActions = new();

    private ILoggingBuilder? _logging;
    private IServiceProviderFactory<object>? _serviceProviderFactory;

    public IPluginHostEnvironment Environment { get; }
    public IServiceCollection RuntimeServices { get; }
    public IServiceCollection InitializationServices { get; }
    public ConfigurationManager Configuration { get; }
    public ILoggingBuilder Logging
    {
        get
        {
            return _logging ??= InitializeLogging();

            ILoggingBuilder InitializeLogging()
            {
                // if someone accesses the Logging builder, ensure Logging has been initialized.
                RuntimeServices.AddLogging();
                return new LoggingBuilder(RuntimeServices);
            }
        }
    }
    public IPluginCollection Plugins { get; }
    public IDictionary<object, object> Properties { get; }

    internal PluginApplicationBuilder(PluginApplicationOptions options)
    {
        Configuration = new();
        RuntimeServices = new ServiceCollection();
        InitializationServices = new ServiceCollection();
        Plugins = IPluginCollection.NewDefaultCollection;
        Properties = new Dictionary<object, object>();

        _hostApplicationBuilder = new(new HostApplicationBuilderSettings
        {
            Args = options.Args,
            ApplicationName = options.ApplicationName,
            ContentRootPath = options.ContentRootPath,
            EnvironmentName = options.EnvironmentName,
            Configuration = Configuration
        });

        Environment = new PluginHostEnvironment(_hostApplicationBuilder.Environment);

        _pluginHostContext = new()
        {
            Configuration = Configuration,
            Environment = Environment
        };
    }

    public IPluginApplication Build()
    {
        throw new NotImplementedException();
    }

    public IPluginApplicationBuilder ConfigureAppConfiguration(Action<PluginHostBuilderContext, IConfigurationBuilder> configureDelegate)
    {
        configureDelegate(_pluginHostContext, Configuration);
        return this;
    }

    public IPluginApplicationBuilder ConfigureContainer<TContainerBuilder>(Action<PluginHostBuilderContext, TContainerBuilder> configureDelegate)
    {
        _configureContainerActions.Add((context, containerBuilder) => configureDelegate(context, (TContainerBuilder)containerBuilder));
        return this;
    }

    public IPluginApplicationBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
    {
        configureDelegate(Configuration);
        return this;
    }

    public IPluginApplicationBuilder UseRuntimeServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) where TContainerBuilder : notnull
    {
        _serviceProviderFactory = new ServiceProviderFactoryAdapter<TContainerBuilder>(factory);
        return this;
    }

    public IPluginApplicationBuilder UseRuntimeServiceProviderFactory<TContainerBuilder>(Func<PluginHostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory) where TContainerBuilder : notnull
    {
        UseRuntimeServiceProviderFactory(factory(_pluginHostContext));
        return this;
    }

    private sealed class LoggingBuilder : ILoggingBuilder
    {
        public IServiceCollection Services { get; }

        public LoggingBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }

    private sealed class ServiceProviderFactoryAdapter<TContainerBuilder> : IServiceProviderFactory<object> where TContainerBuilder : notnull
    {
        private readonly IServiceProviderFactory<TContainerBuilder> _serviceProviderFactory;

        public ServiceProviderFactoryAdapter(IServiceProviderFactory<TContainerBuilder> serviceProviderFactory)
        {
            _serviceProviderFactory = serviceProviderFactory;
        }

        public object CreateBuilder(IServiceCollection services) => _serviceProviderFactory.CreateBuilder(services);
        public IServiceProvider CreateServiceProvider(object containerBuilder) => _serviceProviderFactory.CreateServiceProvider((TContainerBuilder)containerBuilder);
    }
}
