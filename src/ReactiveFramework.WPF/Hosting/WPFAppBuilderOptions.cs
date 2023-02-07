using ReactiveFramework.Hosting;

namespace ReactiveFramework.WPF.Hosting;

public class WPFAppBuilderOptions : PluginAppBuilderOptions
{
    public Type? ShellType { get; set; }
}