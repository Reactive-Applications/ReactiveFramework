using Microsoft.Xaml.Behaviors;
using ReactiveFramework.WPF.Attributes;
using ReactiveFramework.WPF.ViewComposition;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ReactiveFramework.WPF.Controls;
public class ViewContainer : Control
{
    internal static Dictionary<object, Control> ViewContainers = new();
    internal static Dictionary<object, List<Action<Control>>> InitialInsertions = new();

    public static void ExecuteContainerAction(object containerKey, Action<Control> action)
    {
        if (!ViewContainers.TryGetValue(containerKey, out var container))
        {
            if (!InitialInsertions.TryGetValue(containerKey, out var initialInsertions))
            {
                initialInsertions = new();
                InitialInsertions[containerKey] = initialInsertions;
            }

            initialInsertions.Add(action);
            return;
        }
        action(container);
    }

    public object Key
    {
        get { return GetValue(KeyProperty); }
        set { SetValue(KeyProperty, value); }
    }

    public ViewContainer()
    {
        ContainerPanel = new ContentControl();
    }

    public Control ContainerPanel
    {
        get { return (Control)GetValue(ContainerElementProperty); }
        set { SetValue(ContainerElementProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ContainerElement.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ContainerElementProperty =
        DependencyProperty.Register(nameof(ContainerPanel), typeof(Control), typeof(ViewContainer), new FrameworkPropertyMetadata(OnContainerPanelChanged));

    private static void OnContainerPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var container = (ViewContainer)d;
        if (e.OldValue != null)
        {
            container.RemoveLogicalChild(e.OldValue);
            container.RemoveVisualChild((Control)e.OldValue);
        }

        container.AddLogicalChild(e.NewValue);
        container.AddVisualChild((Control)e.NewValue);
    }

    // Using a DependencyProperty as the backing store for Key.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty KeyProperty =
        DependencyProperty.Register("Key", typeof(object), typeof(ViewContainer), new PropertyMetadata(null));

    public override void EndInit()
    {
        base.EndInit();
        if (Key is null)
        {
            throw new NotSupportedException("the key was not set");
        }

        if (!ContainsAssociatedAttribute())
        {
            throw new NotSupportedException($"no {typeof(ContainsViewContainerAttribute).Name} attribute with the key {Key} found.");
        }

        ViewContainers.Add(Key, ContainerPanel);

        if (InitialInsertions.TryGetValue(Key, out var initialActions))
        {
            foreach (var action in initialActions)
            {
                action(ContainerPanel);
            }
        }
    }

    protected override int VisualChildrenCount => 1;

    protected override Visual GetVisualChild(int index)
    {
        return ContainerPanel;
    }

    private bool ContainsAssociatedAttribute()
    {
        if (DesignerProperties.GetIsInDesignMode(this))
        {
            return true;
        }

        DependencyObject current = Parent;

        while (current != null && current is FrameworkElement element)
        {
            if (current.GetType().GetCustomAttributes<ContainsViewContainerAttribute>().Any(a => a.Key.Equals(Key)))
            {
                return true;
            }

            current = element.Parent;
        }

        return false;
    }
}
