using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Windows.Win32;
using Windows.Win32.Graphics.Gdi;

namespace RxFramework.WPF.FluentControls.Windowing;


public sealed class GlowBitmap : DisposableObject
{
    private sealed class CachedBitmapInfo
    {
        public readonly int Width;

        public readonly int Height;

        public readonly byte[] DiBits;

        public CachedBitmapInfo(byte[] diBits, int width, int height)
        {
            Width = width;
            Height = height;
            DiBits = diBits;
        }
    }

    public const int GlowBitmapPartCount = 16;

    private const int BytesPerPixelBgra32 = 4;

    private static readonly Dictionary<CachedBitmapInfoKey, CachedBitmapInfo?[]> transparencyMasks = new();

    private IntPtr pbits;

    private readonly BITMAPINFO bitmapInfo;

    public SafeHandle Handle { get; }

    public IntPtr DiBits => pbits;

    public int Width => bitmapInfo.bmiHeader.biWidth;

    public int Height => -bitmapInfo.bmiHeader.biHeight;

    public unsafe GlowBitmap(SafeHandle hdcScreen, int width, int height)
    {
        bitmapInfo.bmiHeader.biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER));
        bitmapInfo.bmiHeader.biPlanes = 1;
        bitmapInfo.bmiHeader.biBitCount = 32;
        bitmapInfo.bmiHeader.biCompression = 0;
        bitmapInfo.bmiHeader.biXPelsPerMeter = 0;
        bitmapInfo.bmiHeader.biYPelsPerMeter = 0;
        bitmapInfo.bmiHeader.biWidth = width;
        bitmapInfo.bmiHeader.biHeight = -height;

        fixed (BITMAPINFO* pbitmapinfo = &bitmapInfo)
        {
            Handle = new DeleteObjectSafeHandle(PInvoke.CreateDIBSection(new HDC(hdcScreen.DangerousGetHandle()), pbitmapinfo, DIB_USAGE.DIB_RGB_COLORS, out var bits, default, 0));
            pbits = bits;
        }
    }

    protected override void DisposeNativeResources()
    {
        Handle.Dispose();
    }

    private static byte PremultiplyAlpha(byte channel, byte alpha)
    {
        return (byte)(channel * alpha / 255.0);
    }

    public static GlowBitmap? Create(GlowDrawingContext drawingContext, GlowBitmapPart bitmapPart, Color color, int glowDepth, bool useRadialGradientForCorners)
    {
        if (drawingContext.ScreenDc is null)
        {
            return null;
        }

        var alphaMask = GetOrCreateAlphaMask(bitmapPart, glowDepth, useRadialGradientForCorners);
        var glowBitmap = new GlowBitmap(drawingContext.ScreenDc, alphaMask.Width, alphaMask.Height);
        for (var i = 0; i < alphaMask.DiBits.Length; i += BytesPerPixelBgra32)
        {
            var b = alphaMask.DiBits[i + 3];
            var val = PremultiplyAlpha(color.R, b);
            var val2 = PremultiplyAlpha(color.G, b);
            var val3 = PremultiplyAlpha(color.B, b);
            Marshal.WriteByte(glowBitmap.DiBits, i, val3);
            Marshal.WriteByte(glowBitmap.DiBits, i + 1, val2);
            Marshal.WriteByte(glowBitmap.DiBits, i + 2, val);
            Marshal.WriteByte(glowBitmap.DiBits, i + 3, b);
        }

        return glowBitmap;
    }

    private static CachedBitmapInfo GetOrCreateAlphaMask(GlowBitmapPart bitmapPart, int glowDepth, bool useRadialGradientForCorners)
    {
        var cacheKey = new CachedBitmapInfoKey(glowDepth, useRadialGradientForCorners);
        if (transparencyMasks.TryGetValue(cacheKey, out var transparencyMasksForGlowDepth) == false)
        {
            transparencyMasksForGlowDepth = new CachedBitmapInfo?[GlowBitmapPartCount];
            transparencyMasks[cacheKey] = transparencyMasksForGlowDepth;
        }

        var num = (int)bitmapPart;
        if (transparencyMasksForGlowDepth[num] is { } transparencyMask)
        {
            return transparencyMask;
        }

        var bitmapImage = GlowWindowBitmapGenerator.GenerateBitmapSource(bitmapPart, glowDepth, useRadialGradientForCorners);
        var array = new byte[BytesPerPixelBgra32 * bitmapImage.PixelWidth * bitmapImage.PixelHeight];
        var stride = BytesPerPixelBgra32 * bitmapImage.PixelWidth;
        bitmapImage.CopyPixels(array, stride, 0);
        var cachedBitmapInfo = new CachedBitmapInfo(array, bitmapImage.PixelWidth, bitmapImage.PixelHeight);
        transparencyMasksForGlowDepth[num] = cachedBitmapInfo;

        return cachedBitmapInfo;
    }
}