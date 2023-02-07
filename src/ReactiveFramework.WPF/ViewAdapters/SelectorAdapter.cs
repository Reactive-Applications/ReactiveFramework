using System.Windows;
using System.Windows.Controls.Primitives;

namespace ReactiveFramework.WPF.ViewAdapters;
public class SelectorAdapter : ViewAdapterBase<Selector>
{
    public override bool Contains(Selector container, FrameworkElement view)
    {
        return container.Items.Contains(view);
    }

    public override FrameworkElement? GetActiveView(Selector container)
    {
        return container.SelectedItem as FrameworkElement;
    }

    public override IEnumerable<FrameworkElement> GetElements(Selector container)
    {
        return container.Items.OfType<FrameworkElement>();
    }

    public override void Insert(Selector container, FrameworkElement view)
    {
        if (!Contains(container, view))
        {
            container.Items.Add(view);
        }
    }

    public override void Remove(Selector container, FrameworkElement view)
    {
        if (Contains(container, view))
        {
            container.Items.Remove(view);
        }
    }

    public override void Select(Selector container, FrameworkElement view)
    {
        if (!Contains(container, view))
        {
            Insert(container, view);
        }

        container.SelectedItem = view;
    }
}
