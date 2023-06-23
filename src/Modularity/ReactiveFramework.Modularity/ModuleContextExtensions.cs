using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReactiveFramework.Modularity.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveFramework.Modularity;
public static class ModuleContextExtensions
{
    public static IServiceCollection GetServiceCollection(this IModuleRegistrationContext registrationContext)
        => registrationContext.Get<IServiceCollection>();

    public static IHostEnvironment GetEnvironment(this IModuleRegistrationContext registrationContext)
        => registrationContext.Get<IHostEnvironment>();

    public static ConfigurationManager GetConfiguration(this IModuleRegistrationContext registrationContext)
        => registrationContext.Get<ConfigurationManager>();

    public static ILoggingBuilder GetLogging(this IModuleRegistrationContext registrationContext)
        => registrationContext.Get<ILoggingBuilder>();
}
