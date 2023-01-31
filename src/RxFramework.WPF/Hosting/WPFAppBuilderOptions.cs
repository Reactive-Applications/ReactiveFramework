using RxFramework.Hosting;

namespace RxFramework.WPF.Hosting;

public class WPFAppBuilderOptions : PluginAppBuilderOptions
{
    public Type? ShellType { get; set; }
}