using Microsoft.Xaml.Behaviors;
using ReactiveFramework.WPF.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ReactiveFramework.WPF.Behaviors;
public class ViewContainer : Behavior<FrameworkElement>
{

    internal static Dictionary<object, FrameworkElement> ViewContainers = new();
    internal static Dictionary<object, List<Action<FrameworkElement>>> InitialInsertions = new();

    public static void ExecuteContainerAction(object containerKey, Action<FrameworkElement> action)
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

    public static readonly DependencyProperty KeyProperty =
        DependencyProperty.Register("Key", typeof(object), typeof(ViewContainer), new PropertyMetadata(null));

    public object Key
    {
        get { return GetValue(KeyProperty); }
        set { SetValue(KeyProperty, value); }
    }

    protected override void OnAttached()
    {
        base.OnAttached();

        if (Key is null)
        {
            throw new NotSupportedException("the key was not set");
        }

        if (!ContainsAssociatedAttribute())
        {
            throw new NotSupportedException($"no {typeof(ContainsViewContainerAttribute).Name} attribute with the key {Key} found.");
        }

        ViewContainers.Add(Key, AssociatedObject);

        if (InitialInsertions.TryGetValue(Key, out var initialActions))
        {
            foreach (var action in initialActions)
            {
                action(AssociatedObject);
            }
        }
    }

    private bool ContainsAssociatedAttribute()
    {
        if (DesignerProperties.GetIsInDesignMode(this))
        {
            return true;
        }

        DependencyObject current = AssociatedObject;

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
