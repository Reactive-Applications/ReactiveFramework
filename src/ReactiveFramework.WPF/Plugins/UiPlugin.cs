using ReactiveFramework.Hosting.Abstraction.Attributes;
using ReactiveFramework.Hosting.Plugins;
using ReactiveFramework.WPF.ViewComposition;
using System.Reflection;

namespace ReactiveFramework.WPF.Plugins;
public abstract class UiPlugin : Plugin
{
    [InvokedAtAppInitialization]
    public void RegisterViews(IViewCollection viewCollection)
    {
        viewCollection.AddViewsFromAssembly(GetType().Assembly);
    }
}
