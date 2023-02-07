using ReactiveFramework.WPF.Internal;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ReactiveFramework.WPF.ViewComposition;
public static class ViewProviderExtensions
{
    public static FrameworkElement GetView<TViewModel>(this IViewProvider viewProvider)
        where TViewModel : IViewModel
    {
        return viewProvider.GetViewWithViewModel(typeof(TViewModel));
    }

    public static Window GetShell(this IViewProvider viewProvider)
    {
        var descriptor = viewProvider.ViewCollection.GetDescriptorsByKey(ViewDescriptorKeys.IsShellKey).FirstOrDefault()
            ?? throw new InvalidOperationException("no shell registered");

        var shell = (Window)viewProvider.GetViewWithViewModel(descriptor);

        return shell;
    }

    public static bool IsSplashScreenRegistered(this IViewProvider viewProvider)
    {
        return viewProvider.ViewCollection.GetDescriptorsByKey(ViewDescriptorKeys.IsSplashScreenKey).Any();
    }

    public static Window GetSplashScreen(this IViewProvider viewProvider)
    {
        var descriptor = viewProvider.ViewCollection.GetDescriptorsByKey(ViewDescriptorKeys.IsSplashScreenKey).FirstOrDefault()
            ?? throw new InvalidOperationException("no splash screen registered");

        return (Window)viewProvider.GetViewWithViewModel(descriptor);
    }

    public static ViewDescriptor GetViewDescriptorForViewModel(this IViewProvider viewProvider, Type vmType)
    {
        return viewProvider.ViewCollection.GetDescriptorForViewModel(vmType);
    }

    public static bool IsNavigableView(this IViewProvider viewProvider, Type vmType)
    {
        var descriptor = viewProvider.GetViewDescriptorForViewModel(vmType);

        return descriptor?.LookupKeys.Contains(ViewDescriptorKeys.IsNavigableKey) ?? false;
    }

    public static FrameworkElement GetViewWithViewModel(this IViewProvider viewProvider, Type viewModelType)
    {
        return !viewProvider.ViewCollection.TryGetDescriptorForViewModel(viewModelType, out var viewDescriptor)
            ? throw new KeyNotFoundException($"no view for view model type \"{viewModelType.FullName}\" found")
            : viewProvider.GetViewWithViewModel(viewDescriptor);
    }

    public static FrameworkElement GetView(this IViewProvider viewProvider, Type viewModelType)
    {
        var descriptor = viewProvider.GetViewDescriptorForViewModel(viewModelType);

        return viewProvider.GetView(descriptor);
    }
}
