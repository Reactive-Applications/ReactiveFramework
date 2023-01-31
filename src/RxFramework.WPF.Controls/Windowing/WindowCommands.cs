using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RxFramework.WPF.FluentControls.Windowing;

[TemplatePart(Name = PART_CloseCommand, Type = typeof(Button))]
[TemplatePart(Name = PART_MinimizeCommand, Type = typeof(Button))]
[TemplatePart(Name = PART_MaximizeCommand, Type = typeof(Button))]
[TemplatePart(Name = PART_RestoreCommand, Type = typeof(Button))]
public sealed class WindowCommands : Control
{
    private const string PART_MinimizeCommand = nameof(PART_MinimizeCommand);
    private const string PART_MaximizeCommand = nameof(PART_MaximizeCommand);
    private const string PART_RestoreCommand = nameof(PART_RestoreCommand);
    private const string PART_CloseCommand = nameof(PART_CloseCommand);

    private SafeHandle? _user32;
    private Window _parentWindow;

    private Button? _minimizeButton;
    private Button? _maximizeButton;
    private Button? _restoreButton;
    private Button? _closeButton;

    static WindowCommands()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowCommands), new FrameworkPropertyMetadata(typeof(WindowCommands)));
    }

    public WindowCommands()
    {
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _parentWindow = GetParentWindow();

        _minimizeButton = Template.FindName(PART_MinimizeCommand, this) as Button;
        if (_minimizeButton is not null)
        {
            _minimizeButton.Click += MinimizeClick;
        }

        _maximizeButton = Template.FindName(PART_MaximizeCommand, this) as Button;
        if (_maximizeButton is not null)
        {
            _maximizeButton.Click += MaximiseClick;
        }

        _restoreButton = Template.FindName(PART_RestoreCommand, this) as Button;
        if (_restoreButton is not null)
        {
            _restoreButton.Click += RestoreClick;
        }

        _closeButton = GetTemplateChild(PART_CloseCommand) as Button;
        if (_closeButton is not null)
        {
            _closeButton.Click += CloseClick;
        }
    }

    private void CloseClick(object sender, RoutedEventArgs e)
    {
        if (_parentWindow is not null)
        {
            SystemCommands.CloseWindow(_parentWindow);
        }
    }

    private void RestoreClick(object sender, RoutedEventArgs e)
    {
        if (_parentWindow is not null)
        {
            SystemCommands.RestoreWindow(_parentWindow);
        }
    }

    private void MaximiseClick(object sender, RoutedEventArgs e)
    {
        if (_parentWindow is not null)
        {
            SystemCommands.MaximizeWindow(_parentWindow);
        }
    }

    private void MinimizeClick(object sender, RoutedEventArgs e)
    {
        if (_parentWindow is not null)
        {
            SystemCommands.MinimizeWindow(_parentWindow);
        }
    }

    private Window GetParentWindow()
    {
        var window = Window.GetWindow(this);

        if (window is not null)
        {
            return window;
        }

        var parent = VisualTreeHelper.GetParent(this);
        Window parentWindow = null!;

        while (parent is not null
            && (parentWindow = parent as Window) is null)
        {
            parent = VisualTreeHelper.GetParent(parent);
        }

        return parentWindow;
    }
}
