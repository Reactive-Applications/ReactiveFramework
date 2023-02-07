using ReactiveFramework.WPF.Attributes;
using ReactiveFramework.WPF.Internal;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace ReactiveFramework.WPF.ViewComposition;
public static class ViewCollectionExtensions
{
    public static IViewCollection AddViewsFromAssembly(this IViewCollection viewCollection, Assembly assembly)
    {
        var possibleTypes = assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(FrameworkElement)) && t.GetCustomAttribute(typeof(ViewForAttribute<>)) != null);

        foreach (var type in possibleTypes)
        {
            viewCollection.AddView(type);
        }

        return viewCollection;
    }

    public static IViewCollection AddShell(this IViewCollection viewCollection, Type shellType)
    {
        var descriptor = viewCollection.AddView(shellType);
        viewCollection.AddLookupKey(descriptor, ViewDescriptorKeys.IsShellKey);

        return viewCollection;
    }

    public static IViewCollection AddShell<TShell>(this IViewCollection viewCollection)
        where TShell : Window
    {
        var shellType = typeof(TShell);

        return viewCollection.AddShell(shellType);
    }

    public static IViewCollection AddSplashScreen(this IViewCollection viewCollection, Type splashScreenType)
    {
        var descriptor = viewCollection.AddView(splashScreenType);
        viewCollection.AddLookupKey(descriptor, ViewDescriptorKeys.IsSplashScreenKey);

        return viewCollection;
    }

    public static IViewCollection AddSplashScreen<TSplashScreen>(this IViewCollection viewCollection)
        where TSplashScreen : Window
    {
        var splashScreenType = typeof(TSplashScreen);

        return viewCollection.AddSplashScreen(splashScreenType);
    }

    public static IViewCollection AddViewForNavigation<TView>(this IViewCollection viewCollection)
        where TView : Page
    {
        return viewCollection.AddViewForNavigation(typeof(TView));
    }

    public static IViewCollection AddViewForNavigation(this IViewCollection viewCollection, Type viewType)
    {
        if (!viewType.IsAssignableFrom(typeof(Page)))
        {
            throw new ArgumentException($"a view for navigation must be a {typeof(Page).FullName}");
        }

        var descriptor = viewCollection.AddView(viewType);
        viewCollection.AddLookupKey(descriptor, ViewDescriptorKeys.IsNavigableKey);

        return viewCollection;
    }
}
