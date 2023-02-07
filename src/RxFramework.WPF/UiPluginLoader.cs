using RxFramework.Hosting.Plugins;
using RxFramework.WPF.ViewComposition;

namespace RxFramework.WPF;
public abstract class UiPlugin : Plugin, IUiPlugin
{
    public virtual void InitializeUIPlugin(IViewCollection viewCollection)
    {
        viewCollection.AddViewsFromAssembly(GetType().Assembly);
    }
}
