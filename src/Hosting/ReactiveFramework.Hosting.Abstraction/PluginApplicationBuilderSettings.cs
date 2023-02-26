using Microsoft.Extensions.Configuration;

namespace ReactiveFramework.Hosting.Abstraction;
public class PluginApplicationBuilderSettings
{
    public bool DisableDefaults { get; set; }

    public bool AutoInitialize { get; set; } = true;

    public string[]? Args { get; set; }
    public ConfigurationManager? Configuration { get; set; }
    public string? EnvironmentName { get; set; }
    public string? ApplicationName { get; set;}
    public string? ContentRootPath { get; set; }
}
