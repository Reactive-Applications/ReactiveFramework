using System;

using Windows.Win32;
using Windows.Win32.Foundation;

namespace RxFramework.WPF.FluentControls.Internal;
internal static class WindowHelper
{
    public static bool IsWindowHandleValid(IntPtr windowHandle)
    {
        return windowHandle != IntPtr.Zero
               && PInvoke.IsWindow(new HWND(windowHandle));
    }
}