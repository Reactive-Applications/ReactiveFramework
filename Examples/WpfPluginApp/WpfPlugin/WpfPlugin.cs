using ReactiveFramework;
using ReactiveFramework.Hosting.Plugins.Attributes;
using ReactiveFramework.WPF;
using WpfPlugin.ViewModels;

namespace WpfPlugin;
public class WpfPlugin : UiPlugin
{

    [InvokedAtPluginInitialization]
    public void InjectViews(IViewCompositionService viewCompositionService)
    {
        viewCompositionService.InsertView<ViewAViewModel>("PageViews");
        viewCompositionService.InsertView<ViewBViewModel>("PageViews");
    }
}
