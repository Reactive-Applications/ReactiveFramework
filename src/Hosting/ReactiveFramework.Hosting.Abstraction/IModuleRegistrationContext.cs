using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Dynamic;

namespace ReactiveFramework.Modularity.Abstraction;

public interface IModuleRegistrationContext
{
    IServiceCollection Services { get; }

    IHostEnvironment Environment { get; }

    ConfigurationManager Configuration { get; }

    ILoggingBuilder Logging { get; }

    T GetContextObject<T>();
}

public interface IModuleRegistrationContextBuilder
{
    void Add<T>(T contextObject);
    IModuleRegistrationContext Build();
}