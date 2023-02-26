namespace ReactiveFramework.Hosting.Abstraction;
public class PluginApplicationOptions
{
    public string[]? Args { get; set; }
    public string? EnvironmentName { get; set; }
    public string? ApplicationName { get; set;}
    public string? ContentRootPath { get; set; }
}
