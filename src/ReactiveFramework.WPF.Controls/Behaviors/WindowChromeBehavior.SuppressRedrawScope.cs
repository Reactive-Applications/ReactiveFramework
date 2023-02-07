using RxFramework.WPF.FluentControls.Interop;
using System;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;

namespace RxFramework.WPF.FluentControls.Behaviors;
public partial class WindowChromeBehavior
{
    private class SuppressRedrawScope : IDisposable
    {
        private readonly HWND _hwnd;

        private readonly bool _suppressedRedraw;

        public SuppressRedrawScope(IntPtr hwnd)
        {
            _hwnd = new HWND(hwnd);

            if ((PInvoke.GetWindowStyle(_hwnd) & WINDOW_STYLE.WS_VISIBLE) != 0)
            {
                SetRedraw(state: false);
                _suppressedRedraw = true;
            }
        }

        public unsafe void Dispose()
        {
            if (_suppressedRedraw)
            {
                SetRedraw(state: true);
                const REDRAW_WINDOW_FLAGS FLAGS = REDRAW_WINDOW_FLAGS.RDW_INVALIDATE | REDRAW_WINDOW_FLAGS.RDW_ALLCHILDREN | REDRAW_WINDOW_FLAGS.RDW_FRAME;
                PInvoke.RedrawWindow(_hwnd, default, null, FLAGS);
            }
        }

        private void SetRedraw(bool state)
        {
            PInvoke.SendMessage(_hwnd, WM.SETREDRAW, (nuint)Convert.ToInt32(state), default);
        }
    }
}