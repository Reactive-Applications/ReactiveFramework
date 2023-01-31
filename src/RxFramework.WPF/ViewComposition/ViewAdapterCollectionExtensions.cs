using System.Reflection;

namespace RxFramework.WPF.ViewComposition;
public static class ViewAdapterCollectionExtensions
{
    public static void AddAdapter<T>(this IViewAdapterCollection viewAdapterCollection)
        where T : IViewAdapter
    {
        var adapter = Activator.CreateInstance<T>();
        viewAdapterCollection.AddAdapter(adapter);
    }

    public static void AddAdapter(this IViewAdapterCollection viewAdapterCollection,  Type adapterType)
    {
        if (!adapterType.IsAssignableTo(typeof(IViewAdapter)))
        {
            throw new ArgumentException($"adapter type {adapterType} must implement the {typeof(IViewAdapter)}");
        }

        if (adapterType.IsAbstract)
        {
            throw new ArgumentException($"adapter type {adapterType} cant be abstract");
        }

        var adapter = (IViewAdapter)Activator.CreateInstance(adapterType)!;

        viewAdapterCollection.AddAdapter(adapter);
    }

    public static void AddAdaptersFromAssembly(this IViewAdapterCollection viewAdapterCollection, Assembly assembly)
    {
        var types = assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(IViewAdapter)) && !t.IsAbstract);

        foreach (var adapterType in types)
        {
            viewAdapterCollection.AddAdapter(adapterType);
        }
    }
}
