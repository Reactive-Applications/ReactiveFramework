using ReactiveFramework.Hosting.Abstraction;
using ReactiveFramework.Modularity;
using ReactiveFramework.WPF.ViewComposition;
using System.CodeDom;

namespace ReactiveFramework.WPF.Hosting;

public class WpfModuleContextBuilder : ModuleRegistrationContextBuilder
{
    private readonly IViewCollection _viewCollection;
    private readonly IViewAdapterCollection _viewAdapterCollection;

    public WpfModuleContextBuilder(IViewCollection viewCollection, IViewAdapterCollection viewAdapterCollection)
    {
        _viewCollection = viewCollection;
        _viewAdapterCollection = viewAdapterCollection;
    }

    protected override void ProvidContextObjects(IRxHostBuilder builder)
    {
        base.ProvidContextObjects(builder);
        AddContextObject(_viewCollection);
        AddContextObject(_viewAdapterCollection);
    }
}