using ReactiveFramework.WPF.ViewComposition;
using ReactiveFramework.Hosting.Plugins;
using ReactiveFramework.Hosting.Plugins.Attributes;

namespace ReactiveFramework.WPF;

internal interface IUiPlugin : IPlugin
{
    [InvokedAtAppStart]
    void InitializeUIPlugin(IViewCollection viewCollection);
}