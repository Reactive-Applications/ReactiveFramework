using Microsoft.Extensions.Logging;
using ReactiveFramework.Hosting.Abstraction.Attributes;
using ReactiveFramework.Hosting.Plugins;

namespace WpfPlugin;
public class Testplugin : Plugin
{

    [InvokedAtAppInitialization]
    public void LogMessageInit(ILogger<Testplugin> logger)
    {
        logger.LogInformation("Test Message at Initialization");
    }

    [InvokedAtAppStart]
    public void LogMessage(ILogger<Testplugin> logger)
    {
        logger.LogDebug("Test Message at Stat");
    }
}
