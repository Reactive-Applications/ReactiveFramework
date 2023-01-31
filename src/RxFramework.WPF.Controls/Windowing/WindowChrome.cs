using System;
using System.Windows;

namespace RxFramework.WPF.FluentControls.Windowing;
public static class WindowChrome
{
    #region Attached Properties

    public static readonly DependencyProperty IsHitTestVisibleInChromeProperty = DependencyProperty.RegisterAttached(
        "IsHitTestVisibleInChrome",
        typeof(bool),
        typeof(WindowChrome),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

    public static bool GetIsHitTestVisibleInChrome(IInputElement inputElement)
    {
        if (inputElement is not DependencyObject dependencyObject)
        {
            throw new ArgumentException("The element must be a DependencyObject", nameof(inputElement));
        }

        return (bool)dependencyObject.GetValue(IsHitTestVisibleInChromeProperty);
    }

    public static void SetIsHitTestVisibleInChrome(IInputElement inputElement, bool hitTestVisible)
    {
        if (inputElement is not DependencyObject dependencyObject)
        {
            throw new ArgumentException("The element must be a DependencyObject", nameof(inputElement));
        }

        dependencyObject.SetValue(IsHitTestVisibleInChromeProperty, hitTestVisible);
    }

    public static readonly DependencyProperty ResizeGripDirectionProperty = DependencyProperty.RegisterAttached(
        "ResizeGripDirection",
        typeof(ResizeGripDirection),
        typeof(WindowChrome),
        new FrameworkPropertyMetadata(ResizeGripDirection.None, FrameworkPropertyMetadataOptions.Inherits));

    public static ResizeGripDirection GetResizeGripDirection(IInputElement inputElement)
    {
        if (inputElement is not DependencyObject dependencyObject)
        {
            throw new ArgumentException("The element must be a DependencyObject", nameof(inputElement));
        }

        return (ResizeGripDirection)dependencyObject.GetValue(ResizeGripDirectionProperty);
    }

    public static void SetResizeGripDirection(IInputElement inputElement, ResizeGripDirection direction)
    {
        if (inputElement is not DependencyObject dependencyObject)
        {
            throw new ArgumentException("The element must be a DependencyObject", nameof(inputElement));
        }

        dependencyObject.SetValue(ResizeGripDirectionProperty, direction);
    }

    #endregion
}