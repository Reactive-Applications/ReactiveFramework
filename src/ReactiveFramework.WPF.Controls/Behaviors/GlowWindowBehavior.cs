using Microsoft.Xaml.Behaviors;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.Graphics.Dwm;

using COLORREF = Windows.Win32.COLORREF;

using RxFramework.WPF.FluentControls.Windowing;
using RxFramework.WPF.FluentControls.Internal;
using RxFramework.WPF.FluentControls.Interop;

namespace RxFramework.WPF.FluentControls.Behaviors;
public class GlowWindowBehavior : Behavior<Window>
{
    private static readonly TimeSpan GlowTimerDelay = TimeSpan.FromMilliseconds(200); // 200 ms delay, the same as regular window animations
    private DispatcherTimer? _makeGlowVisibleTimer;
    private WindowInteropHelper? _windowHelper;
    private HWND _windowHandle;
    private HwndSource? _hwndSource;

    private readonly IGlowWindow?[] glowWindows = new IGlowWindow[4];

    /// <summary>
    /// <see cref="DependencyProperty"/> for <see cref="GlowColor"/>.
    /// </summary>
    public static readonly DependencyProperty GlowColorProperty = DependencyProperty.Register(nameof(GlowColor), typeof(Color?), typeof(GlowWindowBehavior), new PropertyMetadata(default(Color?), OnGlowColorChanged));

    private static void OnGlowColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((GlowWindowBehavior)d).UpdateGlowColors();
    }

    /// <summary>
    /// Gets or sets a brush which is used as the glow when the window is active.
    /// </summary>
    public Color? GlowColor
    {
        get => (Color?)GetValue(GlowColorProperty);
        set => SetValue(GlowColorProperty, value);
    }

    /// <summary>
    /// <see cref="DependencyProperty"/> for <see cref="NonActiveGlowColor"/>.
    /// </summary>
    public static readonly DependencyProperty NonActiveGlowColorProperty = DependencyProperty.Register(nameof(NonActiveGlowColor), typeof(Color?), typeof(GlowWindowBehavior), new PropertyMetadata(default(Color?), OnNonActiveGlowColorChanged));

    private static void OnNonActiveGlowColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((GlowWindowBehavior)d).UpdateGlowColors();
    }

    /// <summary>
    /// Gets or sets a brush which is used as the glow when the window is not active.
    /// </summary>
    public Color? NonActiveGlowColor
    {
        get => (Color?)GetValue(NonActiveGlowColorProperty);
        set => SetValue(NonActiveGlowColorProperty, value);
    }

    /// <summary>
    /// <see cref="DependencyProperty"/> for <see cref="IsGlowTransitionEnabled"/>.
    /// </summary>
    public static readonly DependencyProperty IsGlowTransitionEnabledProperty = DependencyProperty.Register(nameof(IsGlowTransitionEnabled), typeof(bool), typeof(GlowWindowBehavior), new PropertyMetadata(true));

    /// <summary>
    /// Defines whether glow transitions should be used or not.
    /// </summary>
    public bool IsGlowTransitionEnabled
    {
        get => (bool)GetValue(IsGlowTransitionEnabledProperty);
        set => SetValue(IsGlowTransitionEnabledProperty, value);
    }

    /// <summary>
    /// <see cref="DependencyProperty"/> for <see cref="GlowDepth"/>.
    /// </summary>
    public static readonly DependencyProperty GlowDepthProperty = DependencyProperty.Register(nameof(GlowDepth), typeof(int), typeof(GlowWindowBehavior), new PropertyMetadata(9, OnGlowDepthChanged));

    private static void OnGlowDepthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((GlowWindowBehavior)d).UpdateGlowDepth();
    }

    /// <summary>
    /// Gets or sets the glow depth.
    /// </summary>
    public int GlowDepth
    {
        get => (int)GetValue(GlowDepthProperty);
        set => SetValue(GlowDepthProperty, value);
    }

    /// <summary>Identifies the <see cref="UseRadialGradientForCorners"/> dependency property.</summary>
    public static readonly DependencyProperty UseRadialGradientForCornersProperty = DependencyProperty.Register(
        nameof(UseRadialGradientForCorners), typeof(bool), typeof(GlowWindowBehavior), new PropertyMetadata(true, OnUseRadialGradientForCornersChanged));

    /// <summary>
    /// Gets or sets whether to use a radial gradient for the corners or not.
    /// </summary>
    public bool UseRadialGradientForCorners
    {
        get => (bool)GetValue(UseRadialGradientForCornersProperty);
        set => SetValue(UseRadialGradientForCornersProperty, value);
    }

    private static void OnUseRadialGradientForCornersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((GlowWindowBehavior)d).UpdateUseRadialGradientForCorners();
    }

    /// <summary>Identifies the <see cref="PreferDWMBorderColor"/> dependency property.</summary>
    public static readonly DependencyProperty PreferDWMBorderColorProperty =
        DependencyProperty.Register(nameof(PreferDWMBorderColor), typeof(bool), typeof(GlowWindowBehavior), new PropertyMetadata(true, OnPreferDWMBorderColorChanged));

    private static void OnPreferDWMBorderColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var behavior = (GlowWindowBehavior)d;
        behavior.UpdateDWMBorder();

        if (behavior.IsUsingDWMBorder)
        {
            behavior.DestroyGlowWindows();
        }
        else
        {
            behavior.UpdateGlowVisibility(true);
        }
    }

    /// <summary>
    /// Gets or sets whether the DWM border should be preferred instead of showing glow windows.
    /// </summary>
    public bool PreferDWMBorderColor
    {
        get => (bool)GetValue(PreferDWMBorderColorProperty);
        set => SetValue(PreferDWMBorderColorProperty, value);
    }

    /// <summary>Identifies the <see cref="DWMSupportsBorderColor"/> dependency property key.</summary>
    // ReSharper disable once InconsistentNaming
    private static readonly DependencyPropertyKey DWMSupportsBorderColorPropertyKey =
        DependencyProperty.RegisterReadOnly(nameof(DWMSupportsBorderColor), typeof(bool), typeof(GlowWindowBehavior), new PropertyMetadata(false));

    /// <summary>Identifies the <see cref="DWMSupportsBorderColor"/> dependency property.</summary>
    public static readonly DependencyProperty DWMSupportsBorderColorProperty = DWMSupportsBorderColorPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets whether DWM supports a border color or not.
    /// </summary>
    public bool DWMSupportsBorderColor
    {
        get => (bool)GetValue(DWMSupportsBorderColorProperty);
        private set => SetValue(DWMSupportsBorderColorPropertyKey, value);
    }

    public bool IsUsingDWMBorder => DWMSupportsBorderColor
                                    && PreferDWMBorderColor;

    protected override void OnAttached()
    {
        base.OnAttached();

        Initialize();
        UpdateGlowWindowPositions(true);
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        _hwndSource?.RemoveHook(AssociatedObjectWindowProc);

        AssociatedObject.Closed -= AssociatedObjectOnClosed;

        AssociatedObject.Activated -= AssociatedObjectActivatedOrDeactivated;
        AssociatedObject.Deactivated -= AssociatedObjectActivatedOrDeactivated;

        StopTimer();

        DestroyGlowWindows();

        _windowHelper = null;
        _windowHandle = default;

        base.OnDetaching();
    }

    private void AssociatedObjectActivatedOrDeactivated(object? sender, EventArgs e)
    {
        UpdateGlowActiveState();
    }

    private void Initialize()
    {
        _windowHelper = new WindowInteropHelper(AssociatedObject);
        _windowHandle = new HWND(_windowHelper.EnsureHandle());
        _hwndSource = HwndSource.FromHwnd(_windowHandle);
        _hwndSource?.AddHook(AssociatedObjectWindowProc);

        AssociatedObject.Closed += AssociatedObjectOnClosed;

        AssociatedObject.Activated += AssociatedObjectActivatedOrDeactivated;
        AssociatedObject.Deactivated += AssociatedObjectActivatedOrDeactivated;

        UpdateDWMBorder();

        if (IsUsingDWMBorder == false)
        {
            CreateGlowWindowHandles();
        }
    }

    private void AssociatedObjectOnClosed(object? o, EventArgs args)
    {
        AssociatedObject.Closed -= AssociatedObjectOnClosed;
        parentWindowWasClosed = true;
        // todo: detach here????

        StopTimer();
        DestroyGlowWindows();
    }

#pragma warning disable SA1401
    public int DeferGlowChangesCount;
#pragma warning restore SA1401

    private bool positionUpdateRequired;

    private bool parentWindowWasClosed;

    private IntPtr AssociatedObjectWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        var message = (WM)msg;

        if (IsUsingDWMBorder
            || parentWindowWasClosed)
        {
            return IntPtr.Zero;
        }

        switch (message)
        {
            case WM.WINDOWPOSCHANGING:
            case WM.WINDOWPOSCHANGED:
                {
                    // If the owner is TopMost we don't receive the regular move message, so we must check that here...
                    var windowPos = Marshal.PtrToStructure<WINDOWPOS>(lParam);
                    if (windowPos.flags.HasFlag(SET_WINDOW_POS_FLAGS.SWP_HIDEWINDOW)
                        || windowPos.flags.HasFlag(SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW))
                    {
                        using (DeferGlowChanges())
                        {
                            UpdateGlowVisibility(PInvoke.IsWindowVisible(_windowHandle));
                        }
                    }

                    break;
                }

            case WM.MOVE:
            case WM.SIZE:
                {
                    if (positionUpdateRequired == false)
                    {
                        positionUpdateRequired = true;
                        PInvoke.PostMessage(hwnd, WM.USER, default, default);
                    }

                    break;
                }

            case WM.USER:
                {
                    if (positionUpdateRequired)
                    {
                        positionUpdateRequired = false;
                        UpdateGlowWindowPositions();
                    }

                    break;
                }
        }

        return IntPtr.Zero;
    }

    private void DestroyGlowWindows()
    {
        for (var i = 0; i < glowWindows.Length; i++)
        {
            glowWindows[i]?.Dispose();
            glowWindows[i] = null;
        }

        isGlowVisible = false;
    }

    public void EndDeferGlowChanges()
    {
        var windowPosInfo = PInvoke.BeginDeferWindowPos(glowWindows.Length);

        foreach (var glowWindow in glowWindows)
        {
            glowWindow?.CommitChanges(windowPosInfo);
        }

        PInvoke.EndDeferWindowPos(windowPosInfo);
    }

    private IGlowWindow GetOrCreateGlowWindow(int index)
    {
        return GetOrCreateGlowWindow((Dock)index);
    }

    private IGlowWindow GetOrCreateGlowWindow(Dock orientation)
    {
        var index = (int)orientation;
        glowWindows[index] ??= CreateGlowWindow(orientation);

        return glowWindows[index]!;
    }

    protected virtual IGlowWindow CreateGlowWindow(Dock orientation)
    {
        return SetupGlowWindow(new GlowWindow(AssociatedObject, this, orientation));
    }

    protected virtual IGlowWindow SetupGlowWindow(IGlowWindow glowWindow)
    {
        glowWindow.ActiveGlowColor = GlowColor ?? Colors.Transparent;
        glowWindow.InactiveGlowColor = NonActiveGlowColor ?? Colors.Transparent;
        glowWindow.IsActive = AssociatedObject.IsActive;
        glowWindow.GlowDepth = GlowDepth;
        glowWindow.UseRadialGradientForCorners = UseRadialGradientForCorners;

        return glowWindow;
    }

    private void CreateGlowWindowHandles()
    {
        for (var i = 0; i < glowWindows.Length; i++)
        {
            var gowWindow = GetOrCreateGlowWindow(i);
            gowWindow.EnsureHandle();
        }
    }

    private bool isGlowVisible;

    private bool IsGlowVisible
    {
        get => isGlowVisible;
        set
        {
            if (isGlowVisible != value)
            {
                isGlowVisible = value;

                for (var i = 0; i < glowWindows.Length; i++)
                {
                    GetOrCreateGlowWindow(i).IsVisible = value;
                }
            }
        }
    }

    protected virtual bool ShouldShowGlow
    {
        get
        {
            if (_windowHandle == IntPtr.Zero)
            {
                return false;
            }

            var handle = _windowHandle;
            if (PInvoke.IsWindowVisible(handle)
                && PInvoke.IsIconic(handle) == false
                && PInvoke.IsZoomed(handle) == false)
            {
                var result = AssociatedObject is not null
                       && AssociatedObject.ResizeMode != ResizeMode.NoResize
                       && GlowDepth > 0
                       && (GlowColor is not null && AssociatedObject.IsActive || NonActiveGlowColor is not null && AssociatedObject.IsActive == false);
                if (result == false)
                {
                    return false;
                }

                if (IsUsingDWMBorder)
                {
                    return false;
                }

                return true;
            }

            return false;
        }
    }

    private void UpdateGlowWindowPositions()
    {
        UpdateGlowWindowPositions(PInvoke.IsWindowVisible(_windowHandle));
    }

    private void UpdateGlowWindowPositions(bool delayIfNecessary)
    {
        using (DeferGlowChanges())
        {
            UpdateGlowVisibility(delayIfNecessary);

            foreach (var glowWindow in glowWindows)
            {
                glowWindow?.UpdateWindowPos();
            }
        }
    }

    private void UpdateGlowColors()
    {
        UpdateDWMBorder();

        using (DeferGlowChanges())
        {
            UpdateGlowVisibility(true);

            foreach (var glowWindow in glowWindows)
            {
                if (glowWindow is null)
                {
                    continue;
                }

                glowWindow.ActiveGlowColor = GlowColor ?? Colors.Transparent;
                glowWindow.InactiveGlowColor = NonActiveGlowColor ?? Colors.Transparent;
            }
        }
    }

    private void UpdateGlowActiveState()
    {
        if (AssociatedObject is null)
        {
            return;
        }

        UpdateDWMBorder();

        var isWindowActive = AssociatedObject.IsActive;

        using (DeferGlowChanges())
        {
            UpdateGlowVisibility(true);

            foreach (var glowWindow in glowWindows)
            {
                if (glowWindow is null)
                {
                    continue;
                }

                glowWindow.IsActive = isWindowActive;
            }
        }
    }

    private bool UpdateDWMBorder()
    {
        if (AssociatedObject is null
            || _windowHandle == IntPtr.Zero)
        {
            return false;
        }

        var isWindowActive = AssociatedObject.IsActive;
        var color = isWindowActive ? GlowColor : NonActiveGlowColor;
        var useColor = AssociatedObject.WindowState != WindowState.Maximized
                       && color.HasValue;
        var attrValue = useColor && PreferDWMBorderColor
                    ? (int)new COLORREF(color!.Value).dwColor
                    : -2 /* Disable DWM border */;
        DWMSupportsBorderColor = DwmHelper.SetWindowAttributeValue(_windowHandle, DWMWINDOWATTRIBUTE.DWMWA_BORDER_COLOR, attrValue);

        return DWMSupportsBorderColor;
    }

    private void UpdateGlowDepth()
    {
        using (DeferGlowChanges())
        {
            UpdateGlowVisibility(true);

            foreach (var glowWindow in glowWindows)
            {
                if (glowWindow is null)
                {
                    continue;
                }

                glowWindow.GlowDepth = GlowDepth;
                glowWindow.UpdateWindowPos();
            }
        }
    }

    private void UpdateUseRadialGradientForCorners()
    {
        using (DeferGlowChanges())
        {
            foreach (var glowWindow in glowWindows)
            {
                if (glowWindow is null)
                {
                    continue;
                }

                glowWindow.UseRadialGradientForCorners = UseRadialGradientForCorners;
            }
        }
    }

    private IDisposable DeferGlowChanges()
    {
        return new ChangeScope(this);
    }

    private void UpdateGlowVisibility(bool delayIfNecessary)
    {
        var shouldShowGlow = ShouldShowGlow;
        if (shouldShowGlow == IsGlowVisible)
        {
            return;
        }

        if ((shouldShowGlow && IsGlowTransitionEnabled && SystemParameters.MinimizeAnimation) & delayIfNecessary)
        {
            if (_makeGlowVisibleTimer is null)
            {
                _makeGlowVisibleTimer = new DispatcherTimer
                {
                    Interval = GlowTimerDelay
                };

                _makeGlowVisibleTimer.Tick += OnDelayedVisibilityTimerTick;

                // If we are early, wait for the window content to be rendered
                if (AssociatedObject.IsLoaded == false)
                {
                    AssociatedObject.ContentRendered += AssociatedObjectOnContentRendered;
                    return;
                }
            }

            // If we are early, wait for the window content to be rendered
            if (AssociatedObject.IsLoaded
                && _makeGlowVisibleTimer.IsEnabled == false)
            {
                _makeGlowVisibleTimer.Start();
            }
        }
        else
        {
            StopTimer();
            IsGlowVisible = shouldShowGlow;
        }
    }

    private void AssociatedObjectOnContentRendered(object? sender, EventArgs e)
    {
        AssociatedObject.ContentRendered -= AssociatedObjectOnContentRendered;
        UpdateGlowVisibility(true);
    }

    private void StopTimer()
    {
        _makeGlowVisibleTimer?.Stop();
    }

    private void OnDelayedVisibilityTimerTick(object? sender, EventArgs e)
    {
        StopTimer();
        UpdateGlowWindowPositions(false);
    }
}
