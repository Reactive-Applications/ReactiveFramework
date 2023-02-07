using RxFramework.WPF.FluentControls.Interop;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.Foundation;
using System.Windows.Media.Imaging;

namespace RxFramework.WPF.FluentControls.Windowing;

public class WindowHeader : Control
{


    public ImageSource IconSource
    {
        get { return (ImageSource)GetValue(IconSourceProperty); }
        set { SetValue(IconSourceProperty, value); }
    }

    // Using a DependencyProperty as the backing store for IconSource.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IconSourceProperty =
        DependencyProperty.Register("IconSource", typeof(ImageSource), typeof(WindowHeader), new PropertyMetadata(default));



    static WindowHeader()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowHeader), new FrameworkPropertyMetadata(typeof(WindowHeader)));
    }


    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        var owner = Window.GetWindow(this);
        IconSource ??= GetIcon(owner);
    }

    private ImageSource GetIcon(Window owner)
    {
        return owner.Icon ??= GetDefaultIcon(new Size(32, 32), owner);
    }

    private unsafe ImageSource GetDefaultIcon(Size size, Window owner)
    {
        // Retrieves the small icon provided by the application. If the application does not provide one, the system uses the system-generated icon for that window.
        const int ICON_SMALL2 = 2;

        const int IDI_APPLICATION = 0x7f00;

        IntPtr iconPtr = IntPtr.Zero;

        IntPtr hwnd = new WindowInteropHelper(owner).Handle;

        if (hwnd != IntPtr.Zero)
        {
            iconPtr = PInvoke.SendMessage(new HWND(hwnd), (uint)WM.GETICON, new(ICON_SMALL2), IntPtr.Zero);

            if (iconPtr == IntPtr.Zero)
            {
                iconPtr = new IntPtr(PInvoke.GetClassLong(new HWND(hwnd), GET_CLASS_LONG_INDEX.GCLP_HICONSM));
            }
        }

        if (iconPtr == IntPtr.Zero)
        {
            var lpNameLocal = (char*)IDI_APPLICATION;
            iconPtr = PInvoke.LoadImage(default, lpNameLocal, GDI_IMAGE_TYPE.IMAGE_ICON, (int)size.Width, (int)size.Height, IMAGE_FLAGS.LR_SHARED);
        }

        if (iconPtr == IntPtr.Zero)
        {
            throw new NullReferenceException("default icon can't be loaded");
        }

        var bitmapFrame = BitmapFrame.Create(Imaging.CreateBitmapSourceFromHIcon(iconPtr, Int32Rect.Empty,
            BitmapSizeOptions.FromWidthAndHeight((int)size.Width, (int)size.Height)));
        return bitmapFrame;
    }
}
