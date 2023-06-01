using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace ReactiveFramework.WPF.ViewComposition;
public interface IViewProvider
{
    IReadOnlyViewCollection ViewCollection { get; }
    FrameworkElement GetViewWithViewModel(ViewDescriptor viewDescriptor);

    FrameworkElement GetViewWithViewModel(IViewModel viewModel);

    FrameworkElement GetView(ViewDescriptor viewDescriptor);

}
