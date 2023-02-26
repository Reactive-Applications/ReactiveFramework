using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using ReactiveFramework.Hosting.Abstraction;
using ReactiveFramework.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveFramework.Hosting;
public static class HostBuilderExtensions
{
    internal static void SetDefaultContentRoot(IConfigurationBuilder configurationBuilder)
    {
        var root = Environment.CurrentDirectory;

        configurationBuilder.AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string?>(HostDefaults.ContentRootKey, root)
        });
    }

    internal static void AddDefaultConfigurationSources(IConfigurationBuilder configurationBuilder, string[]? args)
    {
        configurationBuilder.AddEnvironmentVariables(prefix: "DOTNET_");
        configurationBuilder.AddEnvironmentVariables(prefix: "RXFRAMWEWORK_");
        if(args is not null && args.Length > 0)
        {
            configurationBuilder.AddCommandLine(args);
        }
    }

    public static (PluginHostEnvironment, IFileProvider) CreateHostEnvironment(IConfiguration configuration)
    {
        var env = new PluginHostEnvironment()
        {
            EnvironmentName = configuration[HostDefaults.EnvironmentKey] ?? Environments.Production,
            ContentRootPath = ResolveContentRootPath(configuration[HostDefaults.ContentRootKey], AppContext.BaseDirectory)
        };

        var appName = configuration[HostDefaults.ApplicationKey];

        if(string.IsNullOrEmpty(appName))
        {
            appName = Assembly.GetEntryAssembly()?.GetName().Name;
        }

        if(appName is not null)
        {
            env.ApplicationName = appName;
        }

        var fileProvider = new PhysicalFileProvider(env.ContentRootPath);
        env.ContentRootFileProvider = fileProvider;

        return (env, fileProvider);
    }

    internal static string ResolveContentRootPath(string? contentRootPath, string basePath)
    {
        if(string.IsNullOrEmpty(contentRootPath))
        {
            return basePath;
        }

        if(Path.IsPathRooted(contentRootPath))
        {
            return contentRootPath;
        }

        return Path.Combine(Path.GetFullPath(basePath), contentRootPath);
    }

    internal static void ApplyDefaultAppConfiguration(PluginHostBuilderContext context, IConfigurationBuilder configurationBuilder, string[]? args)
    {
        var env = context.HostingEnvironment;
        var reloadOnChange = context.Configuration.GetValue<bool>("hostBuilder:reloadConfigOnChange", true);
        configurationBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: reloadOnChange)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: reloadOnChange);

        if(env.IsDevelopment() && env.ApplicationName is { Length: > 0})
        {
            var ass = Assembly.Load(new AssemblyName(env.ApplicationName));
            if(ass is not null)
            {
                configurationBuilder.AddUserSecrets(ass, optional: true, reloadOnChange: reloadOnChange);
            }
        }

        configurationBuilder.AddEnvironmentVariables();

        if(args is { Length: > 0 })
        {
            configurationBuilder.AddCommandLine(args);
        }
    }

    internal static void AddDefaultLogging(PluginHostBuilderContext context, ILoggingBuilder logging)
    {
        bool isWindows = OperatingSystem.IsWindows();
        
        if(isWindows)
        {
            logging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Warning);
            logging.AddEventLog();
        }

        logging.AddConfiguration(context.Configuration.GetSection("Logging"));

        if(!OperatingSystem.IsBrowser())
        {
            logging.AddConsole();
        }

        logging.AddDebug();
        logging.AddEventSourceLogger();

        logging.Configure(options => 
            options.ActivityTrackingOptions =
                ActivityTrackingOptions.SpanId |
                ActivityTrackingOptions.TraceId |
                ActivityTrackingOptions.ParentId);
    }

    internal static ServiceProviderOptions? CreateDefaultServiceProviderOptions(PluginHostBuilderContext context)
    {
        bool isDevelopment = context.HostingEnvironment.IsDevelopment();
        return new()
        {
            ValidateScopes = isDevelopment,
            ValidateOnBuild = isDevelopment,
        };
    }
}
