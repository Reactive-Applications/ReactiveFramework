using RxFramework.WPF.ViewComposition;

namespace RxFramework.WPF.Internal;
internal class ViewAdapterCollection : IViewAdapterCollection
{
    private readonly Dictionary<Type, IViewAdapter> _viewAdapters = new();

    public void AddAdapter(IViewAdapter viewAdapter)
    {
        _viewAdapters.Add(viewAdapter.ForType, viewAdapter);
    }

    public IViewAdapter GetAdapterFor(Type viewType)
    {
        var baseType = viewType;
        while (baseType != null)
        {
            if (_viewAdapters.TryGetValue(baseType, out var adapter))
            {
                return adapter;
            }

            baseType = baseType.BaseType;
        }

        throw new KeyNotFoundException($"No adapter for view type {viewType.FullName}");
    }
}
