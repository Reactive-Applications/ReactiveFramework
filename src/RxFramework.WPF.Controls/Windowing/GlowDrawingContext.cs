using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using Windows.Win32;
using Windows.Win32.Graphics.Gdi;

namespace RxFramework.WPF.FluentControls.Windowing;

public sealed class GlowDrawingContext : DisposableObject
{
    internal BLENDFUNCTION _blend;

    private readonly GlowBitmap? _windowBitmap;

    [MemberNotNullWhen(true, nameof(ScreenDc))]
    [MemberNotNullWhen(true, nameof(WindowDc))]
    [MemberNotNullWhen(true, nameof(BackgroundDc))]
    [MemberNotNullWhen(true, nameof(_windowBitmap))]
    public bool IsInitialized
    {
        get
        {
            if (ScreenDc is null
                || WindowDc is null
                || BackgroundDc is null
                || _windowBitmap is null)
            {
                return false;
            }

            if (ScreenDc.DangerousGetHandle() != IntPtr.Zero
                && WindowDc.DangerousGetHandle() != IntPtr.Zero
                && BackgroundDc.DangerousGetHandle() != IntPtr.Zero)
            {
                return _windowBitmap is not null;
            }

            return false;
        }
    }

    public SafeHandle? ScreenDc { get; private set; }

    public SafeHandle? WindowDc { get; }

    public SafeHandle? BackgroundDc { get; }

    public int Width => _windowBitmap?.Width ?? 0;

    public int Height => _windowBitmap?.Height ?? 0;

    private static SafeHandle? DesktopDC;

    public GlowDrawingContext(int width, int height)
    {
        SetupDesktopDC();

        if (ScreenDc is null)
        {
            return;
        }

        try
        {
            WindowDc = PInvoke.CreateCompatibleDC(ScreenDc);
        }
        catch
        {
            DesktopDC?.Dispose();
            DesktopDC = null;
            SetupDesktopDC();

            WindowDc = PInvoke.CreateCompatibleDC(ScreenDc);
        }

        if (WindowDc.DangerousGetHandle() == IntPtr.Zero)
        {
            return;
        }

        BackgroundDc = PInvoke.CreateCompatibleDC(ScreenDc);

        if (BackgroundDc.DangerousGetHandle() == IntPtr.Zero)
        {
            return;
        }

        _blend.BlendOp = 0;
        _blend.BlendFlags = 0;
        _blend.SourceConstantAlpha = byte.MaxValue;
        _blend.AlphaFormat = 0x01; // AC_SRC_ALPHA;
        _windowBitmap = new GlowBitmap(ScreenDc, width, height);
        PInvoke.SelectObject(WindowDc, _windowBitmap.Handle);
    }

    private void SetupDesktopDC()
    {
        DesktopDC ??= new DeleteDCSafeHandle(PInvoke.GetDC(default));

        ScreenDc = DesktopDC;
        if (ScreenDc.DangerousGetHandle() == IntPtr.Zero)
        {
            ScreenDc?.Dispose();
            ScreenDc = null;
        }
    }

    protected override void DisposeManagedResources()
    {
        _windowBitmap?.Dispose();
    }

    protected override void DisposeNativeResources()
    {
        WindowDc?.Dispose();

        BackgroundDc?.Dispose();
    }
}