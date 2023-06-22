using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReactiveFramework;
using ReactiveFramework.Modularity;
using ReactiveFramework.Modularity.Abstraction;
using ReactiveFramework.WPF.Modules;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WpfPlugin.ViewModels;

namespace WpfPlugin;
public class WPFModule : UiModule
{

    public override async Task RegisterModuleAsync(IModuleRegistrationContext context, CancellationToken cancellation)
    {
        await base.RegisterModuleAsync(context, cancellation).ConfigureAwait(false);
        ConfigureOptions(context.GetServiceCollection(), context.GetConfiguration());
    }

    public void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
    {
        var coll = services;
        var conf = configuration;
    }

    public override void ComposeView(IViewCompositionService viewCompositionService)
    {
        viewCompositionService.InsertView<ViewAViewModel>("PageViews");
        viewCompositionService.InsertView(typeof(ViewBViewModel), "PageViews");
    }

    protected override void RegisterServices(IServiceCollection services)
    {
        
    }
}
