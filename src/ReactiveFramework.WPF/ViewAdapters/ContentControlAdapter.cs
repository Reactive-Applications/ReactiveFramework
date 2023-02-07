using ReactiveFramework.WPF.ExtensionMethodes;
using System.Windows;
using System.Windows.Controls;

namespace ReactiveFramework.WPF.ViewAdapters;
public sealed class ContentControlAdapter : ViewAdapterBase<ContentControl>
{
    public override bool Contains(ContentControl container, FrameworkElement view)
    {
        return container.Content == view;
    }

    public override FrameworkElement? GetActiveView(ContentControl container)
    {
        return container.Content as FrameworkElement;
    }

    public override IEnumerable<FrameworkElement> GetElements(ContentControl container)
    {
        return container.Content.AsSingleEnumerable().OfType<FrameworkElement>();
    }

    public override void Insert(ContentControl container, FrameworkElement view)
    {
        container.Content = view;
    }

    public override void Remove(ContentControl container, FrameworkElement view)
    {
        container.Content = null;
    }

    public override void Select(ContentControl container, FrameworkElement view)
    {
        container.Content = view;
    }
}
