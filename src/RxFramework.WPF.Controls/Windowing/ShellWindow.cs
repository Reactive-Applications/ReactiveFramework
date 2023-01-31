using Microsoft.Xaml.Behaviors;
using RxFramework.WPF.FluentControls.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace RxFramework.WPF.FluentControls.Windowing;
public class FluentWindow : Window
{
    static FluentWindow()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(FluentWindow), new FrameworkPropertyMetadata(typeof(FluentWindow)));

        WindowStyleProperty.OverrideMetadata(typeof(FluentWindow), new FrameworkPropertyMetadata(WindowStyle.SingleBorderWindow));

        AllowsTransparencyProperty.OverrideMetadata(typeof(FluentWindow), new FrameworkPropertyMetadata(false));
    }

    public FluentWindow()
    {
        InitializeBehaviors();
    }


    private void InitializeBehaviors()
    {
        InitializeWindowChromeBehavior();

        InitializeGlowWindowBehavior();
    }

    /// <summary>
    /// Initializes the WindowChromeBehavior which is needed to render the custom WindowChrome.
    /// </summary>
    private void InitializeWindowChromeBehavior()
    {
        var behavior = new WindowChromeBehavior();
        BindingOperations.SetBinding(behavior, WindowChromeBehavior.ResizeBorderThicknessProperty, new Binding { Path = new PropertyPath(ResizeBorderThicknessProperty), Source = this });
        BindingOperations.SetBinding(behavior, WindowChromeBehavior.HideTaskbarOnMaximizeProperty, new Binding { Path = new PropertyPath(ShowTaskbarOnMaximizeProperty), Source = this });
        BindingOperations.SetBinding(behavior, WindowChromeBehavior.ShowBorderOnMaximizeProperty, new Binding { Path = new PropertyPath(ShowBorderOnMaximizeProperty), Source = this });
        BindingOperations.SetBinding(behavior, WindowChromeBehavior.EnableMinimizeProperty, new Binding { Path = new PropertyPath(ShowMinButtonProperty), Source = this });
        BindingOperations.SetBinding(behavior, WindowChromeBehavior.EnableMaxRestoreProperty, new Binding { Path = new PropertyPath(ShowMaxRestoreButtonProperty), Source = this });
        BindingOperations.SetBinding(behavior, WindowChromeBehavior.CornerPreferenceProperty, new Binding { Path = new PropertyPath(CornerPreferenceProperty), Source = this });

        SetBinding(IsNCActiveProperty, new Binding { Path = new PropertyPath(WindowChromeBehavior.IsNCActiveProperty), Source = behavior });

        Interaction.GetBehaviors(this).Add(behavior);
    }

    /// <summary>
    /// Initializes the WindowChromeBehavior which is needed to render the custom WindowChrome.
    /// </summary>
    private void InitializeGlowWindowBehavior()
    {
        var behavior = new GlowWindowBehavior();
        BindingOperations.SetBinding(behavior, GlowWindowBehavior.GlowDepthProperty, new Binding { Path = new PropertyPath(GlowDepthProperty), Source = this });
        BindingOperations.SetBinding(behavior, GlowWindowBehavior.GlowColorProperty, new Binding { Path = new PropertyPath(GlowColorProperty), Source = this });
        BindingOperations.SetBinding(behavior, GlowWindowBehavior.NonActiveGlowColorProperty, new Binding { Path = new PropertyPath(NonActiveGlowColorProperty), Source = this });
        BindingOperations.SetBinding(behavior, GlowWindowBehavior.UseRadialGradientForCornersProperty, new Binding { Path = new PropertyPath(UseRadialGradientForCornersProperty), Source = this });
        BindingOperations.SetBinding(behavior, GlowWindowBehavior.IsGlowTransitionEnabledProperty, new Binding { Path = new PropertyPath(IsGlowTransitionEnabledProperty), Source = this });
        BindingOperations.SetBinding(behavior, GlowWindowBehavior.PreferDWMBorderColorProperty, new Binding { Path = new PropertyPath(PreferDWMBorderColorProperty), Source = this });

        SetBinding(DWMSupportsBorderColorProperty, new Binding { Path = new PropertyPath(GlowWindowBehavior.DWMSupportsBorderColorProperty), Source = behavior });

        Interaction.GetBehaviors(this).Add(behavior);
    }

    public Thickness ResizeBorderThickness
    {
        get => (Thickness)GetValue(ResizeBorderThicknessProperty);
        set => SetValue(ResizeBorderThicknessProperty, value);
    }

    // Using a DependencyProperty as the backing store for ResizeBorderThickness.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ResizeBorderThicknessProperty =
        DependencyProperty.Register(nameof(ResizeBorderThickness), typeof(Thickness), typeof(FluentWindow), new PropertyMetadata(WindowChromeBehavior.ResizeBorderThicknessProperty.DefaultMetadata.DefaultValue));

    public int GlowDepth
    {
        get => (int)GetValue(GlowDepthProperty);
        set => SetValue(GlowDepthProperty, value);
    }

    // Using a DependencyProperty as the backing store for GlowDepth.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty GlowDepthProperty =
        DependencyProperty.Register(nameof(GlowDepth), typeof(int), typeof(FluentWindow), new PropertyMetadata(GlowWindowBehavior.GlowDepthProperty.DefaultMetadata.DefaultValue));

    public static readonly DependencyProperty UseRadialGradientForCornersProperty = DependencyProperty.Register(
        nameof(UseRadialGradientForCorners), typeof(bool), typeof(FluentWindow), new PropertyMetadata(GlowWindowBehavior.UseRadialGradientForCornersProperty.DefaultMetadata.DefaultValue));

    public bool UseRadialGradientForCorners
    {
        get => (bool)GetValue(UseRadialGradientForCornersProperty);
        set => SetValue(UseRadialGradientForCornersProperty, value);
    }

    public static readonly DependencyProperty IsGlowTransitionEnabledProperty = DependencyProperty.Register(
        nameof(IsGlowTransitionEnabled), typeof(bool), typeof(FluentWindow), new PropertyMetadata(GlowWindowBehavior.IsGlowTransitionEnabledProperty.DefaultMetadata.DefaultValue));

    public bool IsGlowTransitionEnabled
    {
        get => (bool)GetValue(IsGlowTransitionEnabledProperty);
        set => SetValue(IsGlowTransitionEnabledProperty, value);
    }

    public static readonly DependencyProperty ShowTaskbarOnMaximizeProperty = DependencyProperty.Register(nameof(ShowTaskbarOnMaximize), typeof(bool), typeof(FluentWindow), new PropertyMetadata(WindowChromeBehavior.HideTaskbarOnMaximizeProperty.DefaultMetadata.DefaultValue));

    public bool ShowTaskbarOnMaximize
    {
        get => (bool)GetValue(ShowTaskbarOnMaximizeProperty);
        set => SetValue(ShowTaskbarOnMaximizeProperty, value);
    }

    /// <summary>
    /// Gets/sets if the border thickness value should be kept on maximize
    /// if the MaxHeight/MaxWidth of the window is less than the monitor resolution.
    /// </summary>
    public bool ShowBorderOnMaximize
    {
        get => (bool)GetValue(ShowBorderOnMaximizeProperty);
        set => SetValue(ShowBorderOnMaximizeProperty, value);
    }

    /// <summary>
    /// <see cref="DependencyProperty"/> for <see cref="ShowBorderOnMaximize"/>.
    /// </summary>
    public static readonly DependencyProperty ShowBorderOnMaximizeProperty = DependencyProperty.Register(nameof(ShowBorderOnMaximize), typeof(bool), typeof(FluentWindow), new PropertyMetadata(true));

    public static readonly DependencyProperty ShowMinButtonProperty = DependencyProperty.Register(nameof(ShowMinButton), typeof(bool), typeof(FluentWindow), new PropertyMetadata(true));

    /// <summary>
    /// Gets or sets whether if the minimize button is visible.
    /// </summary>
    public bool ShowMinButton
    {
        get => (bool)GetValue(ShowMinButtonProperty);
        set => SetValue(ShowMinButtonProperty, value);
    }

    public static readonly DependencyProperty ShowMaxRestoreButtonProperty = DependencyProperty.Register(nameof(ShowMaxRestoreButton), typeof(bool), typeof(FluentWindow), new PropertyMetadata(true));

    /// <summary>
    /// Gets or sets whether if the Maximize/Restore button is visible.
    /// </summary>
    public bool ShowMaxRestoreButton
    {
        get => (bool)GetValue(ShowMaxRestoreButtonProperty);
        set => SetValue(ShowMaxRestoreButtonProperty, value);
    }

    /// <summary>
    /// <see cref="DependencyProperty"/> for <see cref="GlowColor"/>.
    /// </summary>
    public static readonly DependencyProperty GlowColorProperty = DependencyProperty.Register(nameof(GlowColor), typeof(Color?), typeof(FluentWindow), new PropertyMetadata(null));

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
    public static readonly DependencyProperty NonActiveGlowColorProperty = DependencyProperty.Register(nameof(NonActiveGlowColor), typeof(Color?), typeof(FluentWindow), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets a brush which is used as the glow when the window is not active.
    /// </summary>
    public Color? NonActiveGlowColor
    {
        get => (Color?)GetValue(NonActiveGlowColorProperty);
        set => SetValue(NonActiveGlowColorProperty, value);
    }

    /// <summary>
    /// <see cref="DependencyProperty"/> for <see cref="IsNCActive"/>.
    /// </summary>
    public static readonly DependencyProperty IsNCActiveProperty = DependencyProperty.Register(nameof(IsNCActive), typeof(bool), typeof(FluentWindow), new PropertyMetadata(false));

    /// <summary>
    /// Gets whether the non-client area is active or not.
    /// </summary>
    public bool IsNCActive
    {
        get => (bool)GetValue(IsNCActiveProperty);
        private set => SetValue(IsNCActiveProperty, value);
    }

    public static readonly DependencyProperty NCActiveBrushProperty = DependencyProperty.Register(nameof(NCActiveBrush), typeof(Brush), typeof(FluentWindow), new PropertyMetadata(default(Brush)));

    public Brush? NCActiveBrush
    {
        get => (Brush?)GetValue(NCActiveBrushProperty);
        set => SetValue(NCActiveBrushProperty, value);
    }

    public static readonly DependencyProperty NCNonActiveBrushProperty = DependencyProperty.Register(nameof(NCNonActiveBrush), typeof(Brush), typeof(FluentWindow), new PropertyMetadata(default(Brush)));

    public Brush? NCNonActiveBrush
    {
        get => (Brush?)GetValue(NCNonActiveBrushProperty);
        set => SetValue(NCNonActiveBrushProperty, value);
    }

    public static readonly DependencyProperty NCCurrentBrushProperty = DependencyProperty.Register(nameof(NCCurrentBrush), typeof(Brush), typeof(FluentWindow), new PropertyMetadata(default(Brush)));

    public Brush? NCCurrentBrush
    {
        get => (Brush?)GetValue(NCCurrentBrushProperty);
        set => SetValue(NCCurrentBrushProperty, value);
    }

    /// <summary>Identifies the <see cref="PreferDWMBorderColor"/> dependency property.</summary>
    public static readonly DependencyProperty PreferDWMBorderColorProperty =
        DependencyProperty.Register(nameof(PreferDWMBorderColor), typeof(bool), typeof(FluentWindow), new PropertyMetadata(true));

    /// <inheritdoc cref="GlowWindowBehavior.PreferDWMBorderColor"/>
    public bool PreferDWMBorderColor
    {
        get => (bool)GetValue(PreferDWMBorderColorProperty);
        set => SetValue(PreferDWMBorderColorProperty, value);
    }

    /// <summary>
    /// <see cref="DependencyProperty"/> for <see cref="DWMSupportsBorderColor"/>.
    /// </summary>
    public static readonly DependencyProperty DWMSupportsBorderColorProperty = DependencyProperty.Register(nameof(DWMSupportsBorderColor), typeof(bool), typeof(FluentWindow), new PropertyMetadata(false));

    /// <inheritdoc cref="GlowWindowBehavior.DWMSupportsBorderColor"/>
    public bool DWMSupportsBorderColor
    {
        get => (bool)GetValue(DWMSupportsBorderColorProperty);
        private set => SetValue(DWMSupportsBorderColorProperty, value);
    }

#pragma warning disable WPF0010
    public static readonly DependencyProperty CornerPreferenceProperty = DependencyProperty.Register(
        nameof(CornerPreference), typeof(WindowCornerPreference), typeof(FluentWindow), new PropertyMetadata(WindowChromeBehavior.CornerPreferenceProperty.DefaultMetadata.DefaultValue));
#pragma warning restore WPF0010

    public WindowCornerPreference CornerPreference
    {
        get => (WindowCornerPreference)GetValue(CornerPreferenceProperty);
        set => SetValue(CornerPreferenceProperty, value);
    }



    public object Header
    {
        get { return GetValue(HeaderProperty); }
        set { SetValue(HeaderProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register("Header", typeof(object), typeof(FluentWindow), new PropertyMetadata(null));



    public object WindowCommands
    {
        get { return GetValue(WindowCommandsProperty); }
        set { SetValue(WindowCommandsProperty, value); }
    }

    // Using a DependencyProperty as the backing store for WindowCommands.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty WindowCommandsProperty =
        DependencyProperty.Register("WindowCommands", typeof(object), typeof(FluentWindow), new PropertyMetadata(null));




    public double HeaderHeight
    {
        get { return (double)GetValue(HeaderHeightProperty); }
        set { SetValue(HeaderHeightProperty, value); }
    }

    // Using a DependencyProperty as the backing store for HeaderHeight.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty HeaderHeightProperty =
        DependencyProperty.Register("HeaderHeight", typeof(double), typeof(FluentWindow), new PropertyMetadata(double.NaN));



    public bool CanDragMove
    {
        get { return (bool)GetValue(CanDragMoveProperty); }
        set { SetValue(CanDragMoveProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CanDragMove.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CanDragMoveProperty =
        DependencyProperty.Register("CanDragMove", typeof(bool), typeof(FluentWindow), new PropertyMetadata(true));



    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        Header ??= new WindowHeader();

        WindowCommands ??= new WindowCommands();
    }
}