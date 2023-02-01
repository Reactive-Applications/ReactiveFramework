using RxFramework.Hosting.Plugins;
using RxFramework.Hosting.Plugins.Attributes;
using RxFramework.WPF.ViewComposition;

namespace RxFramework.WPF;

internal interface IUiPlugin : IPlugin
{
    [InvokedAtPluginInitialization]
    void InitializeUIPlugin(IViewCollection viewCollection);
}