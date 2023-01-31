using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace RxFramework.WPF.ViewComposition;
public interface IViewProvider
{
    IReadOnlyViewCollection ViewCollection { get; }
    FrameworkElement GetViewWithViewModel(ViewDescriptor viewDescriptor); 
    FrameworkElement GetView(ViewDescriptor viewDescriptor);
}
