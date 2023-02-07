using System.Windows;
using System.Windows.Controls;

namespace ReactiveFramework.WPF.ViewAdapters;
internal class StackPanelAdapter : ViewAdapterBase<StackPanel>
{
    public override bool Contains(StackPanel container, FrameworkElement view)
    {
        return container.Children.Contains(view);
    }

    public override FrameworkElement? GetActiveView(StackPanel container)
    {
        throw new NotSupportedException();
    }

    public override IEnumerable<FrameworkElement> GetElements(StackPanel container)
    {
        return container.Children.OfType<FrameworkElement>();
    }

    public override void Insert(StackPanel container, FrameworkElement view)
    {
        container.Children.Add(view);
    }

    public override void Remove(StackPanel container, FrameworkElement view)
    {
        if (!Contains(container, view))
        {
            return;
        }

        container.Children.Remove(view);
    }

    public override void Select(StackPanel container, FrameworkElement view)
    {
        throw new NotSupportedException();
    }
}
