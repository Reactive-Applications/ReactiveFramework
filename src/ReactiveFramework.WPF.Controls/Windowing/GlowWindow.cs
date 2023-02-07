using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;


using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.Graphics.Gdi;
using Color = System.Windows.Media.Color;
using RxFramework.WPF.FluentControls.Behaviors;
using RxFramework.WPF.FluentControls.Interop;

namespace RxFramework.WPF.FluentControls.Windowing;
public sealed class GlowWindow : HwndWrapper, IGlowWindow
{
    [Flags]
    private enum FieldInvalidationTypes
    {
        None = 0,
        Location = 1 << 1,
        Size = 1 << 2,
        ActiveColor = 1 << 3,
        InactiveColor = 1 << 4,
        Render = 1 << 5,
        Visibility = 1 << 6,
        GlowDepth = 1 << 7
    }

    private readonly System.Windows.Window _targetWindow;
    private readonly GlowWindowBehavior _behavior;

    private readonly Dock _orientation;

    private readonly GlowBitmap[] _activeGlowBitmaps = new GlowBitmap[GlowBitmap.GlowBitmapPartCount];

    private readonly GlowBitmap[] _inactiveGlowBitmaps = new GlowBitmap[GlowBitmap.GlowBitmapPartCount];

    private static ushort SharedWindowClassAtom;

    // Member to keep reference alive
    // ReSharper disable NotAccessedField.Local
#pragma warning disable IDE0052 // Remove unread private members
    private static WNDPROC? SharedWndProc;
#pragma warning restore IDE0052 // Remove unread private members
    // ReSharper restore NotAccessedField.Local

    private int _left;

    private int _top;

    private int _width;

    private int _height;

    private int _glowDepth = 9;
    private readonly int _cornerGripThickness = 18;

    private bool _useRadialGradientForCorners = true;

    private bool _isVisible;

    private bool _isActive;

    private Color _activeGlowColor = Colors.Transparent;

    public GlowWindow(Color activeGlowColor)
    {
        _activeGlowColor = activeGlowColor;
    }

    private Color _inactiveGlowColor = Colors.Transparent;

    private FieldInvalidationTypes _invalidatedValues;

    private bool _pendingDelayRender;
    private string _title;

#pragma warning disable SA1310
    private static readonly LPARAM SW_PARENTCLOSING = new(1);
    private static readonly LPARAM SW_PARENTOPENING = new(3);
#pragma warning restore SA1310

    private bool IsDeferringChanges => _behavior.DeferGlowChangesCount > 0;

    public override string ClassName { get; } = "ControlzEx_GlowWindow";

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            UpdateProperty(ref _isVisible, value, FieldInvalidationTypes.Render | FieldInvalidationTypes.Visibility);

            if (value
                && InvalidatedValuesHasFlag(FieldInvalidationTypes.Visibility))
            {
                UpdateWindowPos();
            }
        }
    }

    public int Left
    {
        get => _left;
        set => UpdateProperty(ref _left, value, FieldInvalidationTypes.Location);
    }

    public int Top
    {
        get => _top;
        set => UpdateProperty(ref _top, value, FieldInvalidationTypes.Location);
    }

    public int Width
    {
        get => _width;
        set => UpdateProperty(ref _width, value, FieldInvalidationTypes.Size | FieldInvalidationTypes.Render);
    }

    public int Height
    {
        get => _height;
        set => UpdateProperty(ref _height, value, FieldInvalidationTypes.Size | FieldInvalidationTypes.Render);
    }

    public int GlowDepth
    {
        get => _glowDepth;
        set => UpdateProperty(ref _glowDepth, value, FieldInvalidationTypes.GlowDepth | FieldInvalidationTypes.Render | FieldInvalidationTypes.Location);
    }

    public bool UseRadialGradientForCorners
    {
        get => _useRadialGradientForCorners;
        set => UpdateProperty(ref _useRadialGradientForCorners, value, FieldInvalidationTypes.GlowDepth | FieldInvalidationTypes.Render | FieldInvalidationTypes.Location);
    }

    public bool IsActive
    {
        get => _isActive;
        set => UpdateProperty(ref _isActive, value, FieldInvalidationTypes.Render);
    }

    public Color ActiveGlowColor
    {
        get => _activeGlowColor;
        set => UpdateProperty(ref _activeGlowColor, value, FieldInvalidationTypes.ActiveColor | FieldInvalidationTypes.Render);
    }

    public Color InactiveGlowColor
    {
        get => _inactiveGlowColor;
        set => UpdateProperty(ref _inactiveGlowColor, value, FieldInvalidationTypes.InactiveColor | FieldInvalidationTypes.Render);
    }

    private HWND TargetWindowHandle { get; }

    protected override bool IsWindowSubClassed => true;

    private bool IsPositionValid => !InvalidatedValuesHasFlag(FieldInvalidationTypes.Location | FieldInvalidationTypes.Size | FieldInvalidationTypes.Visibility);

    public GlowWindow(System.Windows.Window owner, GlowWindowBehavior behavior, Dock orientation)
    {
        _targetWindow = owner ?? throw new ArgumentNullException(nameof(owner));
        behavior = behavior ?? throw new ArgumentNullException(nameof(behavior));
        _orientation = orientation;

        TargetWindowHandle = new(new WindowInteropHelper(_targetWindow).EnsureHandle());

        if (TargetWindowHandle == IntPtr.Zero
            || PInvoke.IsWindow(TargetWindowHandle) == false)
        {
            throw new Exception($"TargetWindowHandle {TargetWindowHandle} must be a window.");
        }

        _title = $"Glow_{orientation}";
    }

    private void UpdateProperty<T>(ref T field, T value, FieldInvalidationTypes invalidation)
        where T : struct, IEquatable<T>
    {
        if (field.Equals(value))
        {
            return;
        }

        field = value;
        _invalidatedValues |= invalidation;

        if (IsDeferringChanges == false)
        {
            CommitChanges(IntPtr.Zero);
        }
    }

    protected override ushort CreateWindowClassCore()
    {
        return SharedWindowClassAtom;
    }

    protected override void DestroyWindowClassCore()
    {
        // Do nothing here as we registered a shared class/atom
    }

    protected override unsafe IntPtr CreateWindowCore()
    {
        const WINDOW_EX_STYLE EX_STYLE = WINDOW_EX_STYLE.WS_EX_TOOLWINDOW | WINDOW_EX_STYLE.WS_EX_LAYERED;
        const WINDOW_STYLE STYLE = WINDOW_STYLE.WS_POPUP | WINDOW_STYLE.WS_CLIPSIBLINGS | WINDOW_STYLE.WS_CLIPCHILDREN;

        var windowHandle = PInvoke.CreateWindowEx(EX_STYLE, ClassName, _title, STYLE, 0, 0, 0, 0, TargetWindowHandle, null, null, null);

        return windowHandle;
    }

    protected override nint WndProc(nint hwnd, uint msg, nuint wParam, nint lParam)
    {
        var message = (WM)msg;
        //System.Diagnostics.Trace.WriteLine($"{DateTime.Now} {hwnd} {message} {wParam} {lParam}");

        switch (message)
        {
            case WM.DESTROY:
                Dispose();
                break;

            case WM.NCHITTEST:
                return (nint)WmNcHitTest(lParam);

            case WM.NCLBUTTONDOWN:
            case WM.NCLBUTTONDBLCLK:
            case WM.NCRBUTTONDOWN:
            case WM.NCRBUTTONDBLCLK:
            case WM.NCMBUTTONDOWN:
            case WM.NCMBUTTONDBLCLK:
            case WM.NCXBUTTONDOWN:
            case WM.NCXBUTTONDBLCLK:
                {
                    PInvoke.SendMessage(TargetWindowHandle, (uint)message, wParam, IntPtr.Zero);
                    return default;
                }

            case WM.WINDOWPOSCHANGED:
            case WM.WINDOWPOSCHANGING:
                {
                    var windowpos = Marshal.PtrToStructure<WINDOWPOS>(lParam);
                    windowpos.flags |= SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE;
                    Marshal.StructureToPtr(windowpos, lParam, true);
                    break;
                }

            case WM.SETFOCUS:
                // Move focus back as we don't want to get focused
                PInvoke.SetFocus(new HWND((nint)wParam));
                return default;

            case WM.ACTIVATE:
                return default;

            case WM.NCACTIVATE:
                PInvoke.SendMessage(TargetWindowHandle, (uint)message, wParam, lParam);
                // We have to return true according to https://docs.microsoft.com/en-us/windows/win32/winmsg/wm-ncactivate
                // If we don't do that here the owner window can't be activated.
                return 1;

            case WM.MOUSEACTIVATE:
                // WA_CLICKACTIVE = 2
                PInvoke.SendMessage(TargetWindowHandle, (uint)WM.ACTIVATE, new(2), IntPtr.Zero);

                return 3 /* MA_NOACTIVATE */;

            case WM.DISPLAYCHANGE:
                {
                    if (IsVisible)
                    {
                        RenderLayeredWindow();
                    }

                    break;
                }

            case WM.SHOWWINDOW:
                {
                    // Prevent glow from getting visible before the owner/parent is visible
                    if (lParam == SW_PARENTOPENING)
                    {
                        return default;
                    }

                    break;
                }
        }

        return base.WndProc(hwnd, msg, wParam, lParam);
    }

    private unsafe HT WmNcHitTest(IntPtr lParam)
    {
        if (IsDisposed)
        {
            return HT.NOWHERE;
        }

        var xLParam = PInvoke.GetXLParam(lParam.ToInt32());
        var yLParam = PInvoke.GetYLParam(lParam.ToInt32());
        RECT lpRect = default;
        PInvoke.GetWindowRect(Hwnd, &lpRect);

        switch (_orientation)
        {
            case Dock.Left:
                if (yLParam - _cornerGripThickness < lpRect.top)
                {
                    return HT.TOPLEFT;
                }

                if (yLParam + _cornerGripThickness > lpRect.bottom)
                {
                    return HT.BOTTOMLEFT;
                }

                return HT.LEFT;

            case Dock.Right:
                if (yLParam - _cornerGripThickness < lpRect.top)
                {
                    return HT.TOPRIGHT;
                }

                if (yLParam + _cornerGripThickness > lpRect.bottom)
                {
                    return HT.BOTTOMRIGHT;
                }

                return HT.RIGHT;

            case Dock.Top:
                if (xLParam - _cornerGripThickness < lpRect.left)
                {
                    return HT.TOPLEFT;
                }

                if (xLParam + _cornerGripThickness > lpRect.right)
                {
                    return HT.TOPRIGHT;
                }

                return HT.TOP;

            default:
                if (xLParam - _cornerGripThickness < lpRect.left)
                {
                    return HT.BOTTOMLEFT;
                }

                if (xLParam + _cornerGripThickness > lpRect.right)
                {
                    return HT.BOTTOMRIGHT;
                }

                return HT.BOTTOM;
        }
    }

    public void CommitChanges(IntPtr windowPosInfo)
    {
        InvalidateCachedBitmaps();
        UpdateWindowPosCore(windowPosInfo);
        UpdateLayeredWindowCore();
        _invalidatedValues = FieldInvalidationTypes.None;
    }

    private bool InvalidatedValuesHasFlag(FieldInvalidationTypes flag)
    {
        return (_invalidatedValues & flag) != 0;
    }

    private void InvalidateCachedBitmaps()
    {
        if (InvalidatedValuesHasFlag(FieldInvalidationTypes.ActiveColor)
            || InvalidatedValuesHasFlag(FieldInvalidationTypes.GlowDepth))
        {
            ClearCache(_activeGlowBitmaps);
        }

        if (InvalidatedValuesHasFlag(FieldInvalidationTypes.InactiveColor)
            || InvalidatedValuesHasFlag(FieldInvalidationTypes.GlowDepth))
        {
            ClearCache(_inactiveGlowBitmaps);
        }
    }

    private void UpdateWindowPosCore(IntPtr windowPosInfo)
    {
        if (IsDisposed)
        {
            return;
        }

        if (InvalidatedValuesHasFlag(FieldInvalidationTypes.Location | FieldInvalidationTypes.Size | FieldInvalidationTypes.Visibility))
        {
            var flags = SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE | SET_WINDOW_POS_FLAGS.SWP_NOOWNERZORDER;
            if (InvalidatedValuesHasFlag(FieldInvalidationTypes.Visibility))
            {
                flags = IsVisible
                    ? flags | SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW
                    : flags | SET_WINDOW_POS_FLAGS.SWP_HIDEWINDOW | SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOSIZE;
            }

            if (InvalidatedValuesHasFlag(FieldInvalidationTypes.Location) == false)
            {
                flags |= SET_WINDOW_POS_FLAGS.SWP_NOMOVE;
            }

            if (InvalidatedValuesHasFlag(FieldInvalidationTypes.Size) == false)
            {
                flags |= SET_WINDOW_POS_FLAGS.SWP_NOSIZE;
            }

            if (windowPosInfo == IntPtr.Zero)
            {
                PInvoke.SetWindowPos(Hwnd, default, Left, Top, Width, Height, flags);
            }
            else
            {
                PInvoke.DeferWindowPos(windowPosInfo, Hwnd, default, Left, Top, Width, Height, flags);
            }
        }
    }

    private void UpdateLayeredWindowCore()
    {
        if (IsVisible
            && IsDisposed == false
            && InvalidatedValuesHasFlag(FieldInvalidationTypes.Render))
        {
            if (IsPositionValid)
            {
                BeginDelayedRender();
                return;
            }

            CancelDelayedRender();
            RenderLayeredWindow();
        }
    }

    private void BeginDelayedRender()
    {
        if (_pendingDelayRender == false)
        {
            _pendingDelayRender = true;
            CompositionTarget.Rendering += CommitDelayedRender;
        }
    }

    private void CancelDelayedRender()
    {
        if (_pendingDelayRender)
        {
            _pendingDelayRender = false;
            CompositionTarget.Rendering -= CommitDelayedRender;
        }
    }

    private void CommitDelayedRender(object? sender, EventArgs e)
    {
        CancelDelayedRender();

        if (IsVisible
            && IsDisposed == false)
        {
            RenderLayeredWindow();
        }
    }

    private unsafe void RenderLayeredWindow()
    {
        if (IsDisposed
            || Width == 0
            || Height == 0)
        {
            return;
        }

        using var glowDrawingContext = new GlowDrawingContext(Width, Height);
        if (glowDrawingContext.IsInitialized == false)
        {
            return;
        }

        switch (_orientation)
        {
            case Dock.Left:
                DrawLeft(glowDrawingContext);
                break;
            case Dock.Right:
                DrawRight(glowDrawingContext);
                break;
            case Dock.Top:
                DrawTop(glowDrawingContext);
                break;
            case Dock.Bottom:
                DrawBottom(glowDrawingContext);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(_orientation), _orientation, null);
        }

        var pptDest = new Point { X = Left, Y = Top };
        var psize = new SIZE { cx = Width, cy = Height };
        var pptSrc = new Point { X = 0, Y = 0 };

        fixed (BLENDFUNCTION* blend = &glowDrawingContext._blend)
        {
            //PInvoke.UpdateLayeredWindow(Hwnd, glowDrawingContext.ScreenDc, pptDest, psize, glowDrawingContext.WindowDc, pptSrc, 0, glowDrawingContext.Blend, UPDATE_LAYERED_WINDOW_FLAGS.ULW_ALPHA);
            PInvoke.UpdateLayeredWindow(Hwnd, new HDC(glowDrawingContext.ScreenDc.DangerousGetHandle()), &pptDest, &psize, new HDC(glowDrawingContext.WindowDc.DangerousGetHandle()), &pptSrc, new Windows.Win32.Foundation.COLORREF(0), blend, UPDATE_LAYERED_WINDOW_FLAGS.ULW_ALPHA);
        }
    }

    private GlowBitmap? GetOrCreateBitmap(GlowDrawingContext drawingContext, GlowBitmapPart bitmapPart)
    {
        if (drawingContext.ScreenDc is null)
        {
            return null;
        }

        GlowBitmap?[] array;
        Color color;

        if (IsActive)
        {
            array = _activeGlowBitmaps;
            color = ActiveGlowColor;
        }
        else
        {
            array = _inactiveGlowBitmaps;
            color = InactiveGlowColor;
        }

        return array[(int)bitmapPart] ?? (array[(int)bitmapPart] = GlowBitmap.Create(drawingContext, bitmapPart, color, GlowDepth, UseRadialGradientForCorners));
    }

    private static void ClearCache(GlowBitmap?[] cache)
    {
        for (var i = 0; i < cache.Length; i++)
        {
            using (cache[i])
            {
                cache[i] = null;
            }
        }
    }

    protected override void DisposeManagedResources()
    {
        ClearCache(_activeGlowBitmaps);
        ClearCache(_inactiveGlowBitmaps);
    }

    private void DrawLeft(GlowDrawingContext drawingContext)
    {
        if (drawingContext.ScreenDc is null
            || drawingContext.WindowDc is null
            || drawingContext.BackgroundDc is null)
        {
            return;
        }

        var cornerTopLeftBitmap = GetOrCreateBitmap(drawingContext, GlowBitmapPart.CornerTopLeft)!;
        var leftTopBitmap = GetOrCreateBitmap(drawingContext, GlowBitmapPart.LeftTop)!;
        var leftBitmap = GetOrCreateBitmap(drawingContext, GlowBitmapPart.Left)!;
        var leftBottomBitmap = GetOrCreateBitmap(drawingContext, GlowBitmapPart.LeftBottom)!;
        var cornerBottomLeftBitmap = GetOrCreateBitmap(drawingContext, GlowBitmapPart.CornerBottomLeft)!;

        var bitmapHeight = cornerTopLeftBitmap.Height;
        var num = bitmapHeight + leftTopBitmap.Height;
        var num2 = drawingContext.Height - cornerBottomLeftBitmap.Height;
        var num3 = num2 - leftBottomBitmap.Height;
        var num4 = num3 - num;

        PInvoke.SelectObject(drawingContext.BackgroundDc, cornerTopLeftBitmap.Handle);
        PInvoke.AlphaBlend(drawingContext.WindowDc, 0, 0, cornerTopLeftBitmap.Width, cornerTopLeftBitmap.Height, drawingContext.BackgroundDc, 0, 0, cornerTopLeftBitmap.Width, cornerTopLeftBitmap.Height, drawingContext._blend);
        PInvoke.SelectObject(drawingContext.BackgroundDc, leftTopBitmap.Handle);
        PInvoke.AlphaBlend(drawingContext.WindowDc, 0, bitmapHeight, leftTopBitmap.Width, leftTopBitmap.Height, drawingContext.BackgroundDc, 0, 0, leftTopBitmap.Width, leftTopBitmap.Height, drawingContext._blend);

        if (num4 > 0)
        {
            PInvoke.SelectObject(drawingContext.BackgroundDc, leftBitmap.Handle);
            PInvoke.AlphaBlend(drawingContext.WindowDc, 0, num, leftBitmap.Width, num4, drawingContext.BackgroundDc, 0, 0, leftBitmap.Width, leftBitmap.Height, drawingContext._blend);
        }

        PInvoke.SelectObject(drawingContext.BackgroundDc, leftBottomBitmap.Handle);
        PInvoke.AlphaBlend(drawingContext.WindowDc, 0, num3, leftBottomBitmap.Width, leftBottomBitmap.Height, drawingContext.BackgroundDc, 0, 0, leftBottomBitmap.Width, leftBottomBitmap.Height, drawingContext._blend);
        PInvoke.SelectObject(drawingContext.BackgroundDc, cornerBottomLeftBitmap.Handle);
        PInvoke.AlphaBlend(drawingContext.WindowDc, 0, num2, cornerBottomLeftBitmap.Width, cornerBottomLeftBitmap.Height, drawingContext.BackgroundDc, 0, 0, cornerBottomLeftBitmap.Width, cornerBottomLeftBitmap.Height, drawingContext._blend);
    }

    private void DrawRight(GlowDrawingContext drawingContext)
    {
        if (drawingContext.ScreenDc is null
            || drawingContext.WindowDc is null
            || drawingContext.BackgroundDc is null)
        {
            return;
        }

        var cornerTopRightBitmap = GetOrCreateBitmap(drawingContext, GlowBitmapPart.CornerTopRight)!;
        var rightTopBitmap = GetOrCreateBitmap(drawingContext, GlowBitmapPart.RightTop)!;
        var rightBitmap = GetOrCreateBitmap(drawingContext, GlowBitmapPart.Right)!;
        var rightBottomBitmap = GetOrCreateBitmap(drawingContext, GlowBitmapPart.RightBottom)!;
        var cornerBottomRightBitmap = GetOrCreateBitmap(drawingContext, GlowBitmapPart.CornerBottomRight)!;

        var bitmapHeight = cornerTopRightBitmap.Height;
        var num = bitmapHeight + rightTopBitmap.Height;
        var num2 = drawingContext.Height - cornerBottomRightBitmap.Height;
        var num3 = num2 - rightBottomBitmap.Height;
        var num4 = num3 - num;

        PInvoke.SelectObject(drawingContext.BackgroundDc, cornerTopRightBitmap.Handle);
        PInvoke.AlphaBlend(drawingContext.WindowDc, 0, 0, cornerTopRightBitmap.Width, cornerTopRightBitmap.Height, drawingContext.BackgroundDc, 0, 0, cornerTopRightBitmap.Width, cornerTopRightBitmap.Height, drawingContext._blend);
        PInvoke.SelectObject(drawingContext.BackgroundDc, rightTopBitmap.Handle);
        PInvoke.AlphaBlend(drawingContext.WindowDc, 0, bitmapHeight, rightTopBitmap.Width, rightTopBitmap.Height, drawingContext.BackgroundDc, 0, 0, rightTopBitmap.Width, rightTopBitmap.Height, drawingContext._blend);

        if (num4 > 0)
        {
            PInvoke.SelectObject(drawingContext.BackgroundDc, rightBitmap.Handle);
            PInvoke.AlphaBlend(drawingContext.WindowDc, 0, num, rightBitmap.Width, num4, drawingContext.BackgroundDc, 0, 0, rightBitmap.Width, rightBitmap.Height, drawingContext._blend);
        }

        PInvoke.SelectObject(drawingContext.BackgroundDc, rightBottomBitmap.Handle);
        PInvoke.AlphaBlend(drawingContext.WindowDc, 0, num3, rightBottomBitmap.Width, rightBottomBitmap.Height, drawingContext.BackgroundDc, 0, 0, rightBottomBitmap.Width, rightBottomBitmap.Height, drawingContext._blend);
        PInvoke.SelectObject(drawingContext.BackgroundDc, cornerBottomRightBitmap.Handle);
        PInvoke.AlphaBlend(drawingContext.WindowDc, 0, num2, cornerBottomRightBitmap.Width, cornerBottomRightBitmap.Height, drawingContext.BackgroundDc, 0, 0, cornerBottomRightBitmap.Width, cornerBottomRightBitmap.Height, drawingContext._blend);
    }

    private void DrawTop(GlowDrawingContext drawingContext)
    {
        if (drawingContext.ScreenDc is null
            || drawingContext.WindowDc is null
            || drawingContext.BackgroundDc is null)
        {
            return;
        }

        var topLeftBitmap = GetOrCreateBitmap(drawingContext, GlowBitmapPart.TopLeft)!;
        var topBitmap = GetOrCreateBitmap(drawingContext, GlowBitmapPart.Top)!;
        var topRightBitmap = GetOrCreateBitmap(drawingContext, GlowBitmapPart.TopRight)!;

        var num = GlowDepth;
        var num2 = num + topLeftBitmap.Width;
        var num3 = drawingContext.Width - GlowDepth - topRightBitmap.Width;
        var num4 = num3 - num2;

        PInvoke.SelectObject(drawingContext.BackgroundDc, topLeftBitmap.Handle);
        PInvoke.AlphaBlend(drawingContext.WindowDc, num, 0, topLeftBitmap.Width, topLeftBitmap.Height, drawingContext.BackgroundDc, 0, 0, topLeftBitmap.Width, topLeftBitmap.Height, drawingContext._blend);

        if (num4 > 0)
        {
            PInvoke.SelectObject(drawingContext.BackgroundDc, topBitmap.Handle);
            PInvoke.AlphaBlend(drawingContext.WindowDc, num2, 0, num4, topBitmap.Height, drawingContext.BackgroundDc, 0, 0, topBitmap.Width, topBitmap.Height, drawingContext._blend);
        }

        PInvoke.SelectObject(drawingContext.BackgroundDc, topRightBitmap.Handle);
        PInvoke.AlphaBlend(drawingContext.WindowDc, num3, 0, topRightBitmap.Width, topRightBitmap.Height, drawingContext.BackgroundDc, 0, 0, topRightBitmap.Width, topRightBitmap.Height, drawingContext._blend);
    }

    private void DrawBottom(GlowDrawingContext drawingContext)
    {
        if (drawingContext.ScreenDc is null
            || drawingContext.WindowDc is null
            || drawingContext.BackgroundDc is null)
        {
            return;
        }

        var bottomLeftBitmap = GetOrCreateBitmap(drawingContext, GlowBitmapPart.BottomLeft)!;
        var bottomBitmap = GetOrCreateBitmap(drawingContext, GlowBitmapPart.Bottom)!;
        var bottomRightBitmap = GetOrCreateBitmap(drawingContext, GlowBitmapPart.BottomRight)!;

        var num = GlowDepth;
        var num2 = num + bottomLeftBitmap.Width;
        var num3 = drawingContext.Width - GlowDepth - bottomRightBitmap.Width;
        var num4 = num3 - num2;

        PInvoke.SelectObject(drawingContext.BackgroundDc, bottomLeftBitmap.Handle);
        PInvoke.AlphaBlend(drawingContext.WindowDc, num, 0, bottomLeftBitmap.Width, bottomLeftBitmap.Height, drawingContext.BackgroundDc, 0, 0, bottomLeftBitmap.Width, bottomLeftBitmap.Height, drawingContext._blend);

        if (num4 > 0)
        {
            PInvoke.SelectObject(drawingContext.BackgroundDc, bottomBitmap.Handle);
            PInvoke.AlphaBlend(drawingContext.WindowDc, num2, 0, num4, bottomBitmap.Height, drawingContext.BackgroundDc, 0, 0, bottomBitmap.Width, bottomBitmap.Height, drawingContext._blend);
        }

        PInvoke.SelectObject(drawingContext.BackgroundDc, bottomRightBitmap.Handle);
        PInvoke.AlphaBlend(drawingContext.WindowDc, num3, 0, bottomRightBitmap.Width, bottomRightBitmap.Height, drawingContext.BackgroundDc, 0, 0, bottomRightBitmap.Width, bottomRightBitmap.Height, drawingContext._blend);
    }

    public void UpdateWindowPos()
    {
        var targetWindowHandle = TargetWindowHandle;

        if (IsVisible == false
            || PInvoke.GetMappedClientRect(targetWindowHandle, out var lpRect) == false)
        {
            return;
        }

        switch (_orientation)
        {
            case Dock.Left:
                Left = lpRect.left - GlowDepth;
                Top = lpRect.top - GlowDepth;
                Width = GlowDepth;
                Height = lpRect.GetHeight() + GlowDepth + GlowDepth;
                break;

            case Dock.Top:
                Left = lpRect.left - GlowDepth;
                Top = lpRect.top - GlowDepth;
                Width = lpRect.GetWidth() + GlowDepth + GlowDepth;
                Height = GlowDepth;
                break;

            case Dock.Right:
                Left = lpRect.right;
                Top = lpRect.top - GlowDepth;
                Width = GlowDepth;
                Height = lpRect.GetHeight() + GlowDepth + GlowDepth;
                break;

            case Dock.Bottom:
                Left = lpRect.left - GlowDepth;
                Top = lpRect.bottom;
                Width = lpRect.GetWidth() + GlowDepth + GlowDepth;
                Height = GlowDepth;
                break;
        }
    }
}
