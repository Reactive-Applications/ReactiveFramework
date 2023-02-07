using ReactiveFramework.WPF.ViewComposition;
using ReactiveFramework.Hosting.Plugins;
using ReactiveFramework.Hosting.Plugins.Attributes;

namespace ReactiveFramework.WPF;

internal interface IUiPlugin : IPlugin
{
    [InvokedAtPluginInitialization]
    void InitializeUIPlugin(IViewCollection viewCollection);
}