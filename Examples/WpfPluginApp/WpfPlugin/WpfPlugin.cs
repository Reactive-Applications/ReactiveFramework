using ReactiveFramework;
using ReactiveFramework.Hosting.Plugins.Attributes;
using ReactiveFramework.WPF;
using WpfPlugin.ViewModels;

namespace WpfPlugin;
public class WpfPlugin : UiPlugin
{
    // All Services that are available in the registration serviceProvider can be injected as parameter
    // At the moment these services are:
    // - IConfiguration
    // - ILoggingBuilder
    // - IHostEnvironment
    // - IServiceCollection for runtime Services
    //[InvokedAtPluginRegistration]
    //public void RunOnRegistration()
    //{

    //}

    // All Services that are available at runtime can be injected as Parameter
    [InvokedAtPluginInitialization]
    public void InjectViews(IViewCompositionService viewCompositionService)
    {
        viewCompositionService.InsertView<ViewAViewModel>("PageViews");
        viewCompositionService.InsertView<ViewBViewModel>("PageViews");
    }
}
