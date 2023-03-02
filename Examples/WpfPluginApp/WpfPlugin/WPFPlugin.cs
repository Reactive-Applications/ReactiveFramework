using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReactiveFramework;
using ReactiveFramework.Hosting.Abstraction.Attributes;
using ReactiveFramework.WPF.Plugins;
using WpfPlugin.ViewModels;

namespace WpfPlugin;
public class WPFPlugin : UiPlugin
{
    [InvokedAtAppStart]
    public void InjectViews(IViewCompositionService viewCompositionService)
    {
        viewCompositionService.InsertView<ViewAViewModel>("PageViews");
        viewCompositionService.InsertView<ViewBViewModel>("PageViews");
    }

    [InvokedAtAppInitialization]
    public void ConfigreOptions(IServiceCollection services, IConfiguration configuration)
    {
        var coll = services;
        var conf = configuration;
    }
}
