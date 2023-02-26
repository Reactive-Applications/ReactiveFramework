using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ReactiveFramework.Hosting.Abstraction;
public class PluginHostBuilderContext : HostBuilderContext
{
    public PluginHostBuilderContext(IDictionary<object, object> properties, ILoggingBuilder initializationLogging)
        :base(properties)
    {
        InitializationLogging = initializationLogging;
    }

    public ILoggingBuilder InitializationLogging { get; }

    public bool DisableDefaults { get; set; }

    public bool AutoInitialize { get; set; }
}
