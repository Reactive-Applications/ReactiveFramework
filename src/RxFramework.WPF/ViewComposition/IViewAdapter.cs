using System.Windows;

namespace RxFramework.WPF.ViewComposition;
public interface IViewAdapter
{
    Type ForType { get; }

    bool Contains(FrameworkElement container, FrameworkElement view);
    void Insert(FrameworkElement container, FrameworkElement view);
    void Remove(FrameworkElement container, FrameworkElement view);
    void Select(FrameworkElement container, FrameworkElement view);
    IEnumerable<FrameworkElement> GetElements(FrameworkElement container);
    FrameworkElement? GetActiveView(FrameworkElement container);
}

public interface IViewAdapter<T> : IViewAdapter
    where T : FrameworkElement
{
    bool Contains(T container, FrameworkElement view);
    void Insert(T container, FrameworkElement view);
    void Remove(T container, FrameworkElement view);
    void Select(T container, FrameworkElement view);
    IEnumerable<FrameworkElement> GetElements(T container);
    FrameworkElement? GetActiveView(T container);
}
