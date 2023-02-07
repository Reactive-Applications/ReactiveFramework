using ReactiveFramework.WPF.ViewComposition;
using System.Windows;

namespace ReactiveFramework.WPF.ViewAdapters;
public abstract class ViewAdapterBase<T> : IViewAdapter<T>
    where T : FrameworkElement
{
    public Type ForType => typeof(T);

    public abstract bool Contains(T container, FrameworkElement view);

    public bool Contains(FrameworkElement container, FrameworkElement view)
    {
        return Contains((T)container, view);
    }

    public abstract FrameworkElement? GetActiveView(T container);

    public FrameworkElement? GetActiveView(FrameworkElement container)
    {
        return GetActiveView((T)container);
    }

    public IEnumerable<FrameworkElement> GetElements(FrameworkElement container)
    {
        return GetElements((T)container);
    }

    public abstract IEnumerable<FrameworkElement> GetElements(T container);

    public abstract void Insert(T container, FrameworkElement view);

    public void Insert(FrameworkElement container, FrameworkElement view)
    {
        Insert((T)container, view);
    }

    public abstract void Remove(T container, FrameworkElement view);

    public void Remove(FrameworkElement container, FrameworkElement view)
    {
        Remove((T)container, view);
    }

    public abstract void Select(T container, FrameworkElement view);

    public void Select(FrameworkElement container, FrameworkElement view)
    {
        Select((T)container, view);
    }
}
