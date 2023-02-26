using ReactiveFramework.Hosting;

namespace ReactiveFramework.WPF.Hosting;

public class WPFAppOptions : PluginAppBuilderOptions
{
    public Type? ShellType { get; set; }
}