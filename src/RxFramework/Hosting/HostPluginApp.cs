namespace RxFramework.Hosting;
public static class HostedPluginApp
{

    public static IHostedPluginAppBuilder CreateDefaultBuilder()
    {
        return new HostedPluginAppBuilder();
    }

    public static IHostedPluginAppBuilder CreateDefaultBuilder(string[] args)
    {
        return new HostedPluginAppBuilder(args);
    }

    public static IHostedPluginAppBuilder CreateDefaultBuilder(PluginAppBuilderOptions options)
    {
        return new HostedPluginAppBuilder(options);
    }
}
