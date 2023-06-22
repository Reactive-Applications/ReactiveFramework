using Microsoft.Extensions.DependencyInjection;
using ReactiveFramework.Modularity;
using ReactiveFramework.Modularity.Abstraction;
using ReactiveFramework.WPF.ViewComposition;

namespace ReactiveFramework.WPF.Modules;
public abstract class UiModule : Module
{

    public override Task StartModuleAsync(IServiceProvider services, CancellationToken cancellation)
    {
        ComposeView(services.GetRequiredService<IViewCompositionService>());
        return Task.CompletedTask;
    }

    public override async Task RegisterModuleAsync(IModuleRegistrationContext context, CancellationToken cancellation)
    {
        await base.RegisterModuleAsync(context, cancellation).ConfigureAwait(false);
        RegisterViews(context.Get<IViewCollection>());
    }

    public virtual void RegisterViews(IViewCollection viewCollection)
    {
        viewCollection.AddViewsFromAssembly(GetType().Assembly);
    }

    public abstract void ComposeView(IViewCompositionService viewCompositionService);

}
