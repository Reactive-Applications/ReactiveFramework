using RxFramework.WPF.ViewComposition;
using System.Windows;

namespace RxFramework.WPF.Internal;
internal class ViewProvider : IViewProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IViewCollection _viewCollection;
    public IReadOnlyViewCollection ViewCollection => _viewCollection;

    public ViewProvider(IViewCollection viewCollection, IServiceProvider serviceProvider)
    {
        _viewCollection = viewCollection;
        _serviceProvider = serviceProvider;
    }

    public FrameworkElement GetViewWithViewModel(ViewDescriptor viewDescriptor)
    {
        var vm = (IViewModel)_serviceProvider.GetUnregisteredService(viewDescriptor.ViewModelType);

        FrameworkElement view = (FrameworkElement)_serviceProvider.GetUnregisteredService(viewDescriptor.ViewType);
        view.DataContext = vm;

        return view;
    }

    public FrameworkElement GetView(ViewDescriptor viewDescriptor)
    {
        FrameworkElement view = (FrameworkElement)_serviceProvider.GetUnregisteredService(viewDescriptor.ViewType);
        return view;
    }
}
