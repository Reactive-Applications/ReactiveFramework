using ReactiveFramework.WPF.ViewComposition;
using ReactiveFramework.Hosting.Plugins;

namespace ReactiveFramework.WPF;
public abstract class UiPlugin : Plugin, IUiPlugin
{
    public virtual void InitializeUIPlugin(IViewCollection viewCollection)
    {
        viewCollection.AddViewsFromAssembly(GetType().Assembly);
    }
}
