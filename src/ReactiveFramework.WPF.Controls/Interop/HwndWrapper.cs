using System;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace RxFramework.WPF.FluentControls.Interop;
public abstract class HwndWrapper : DisposableObject
{
    private HWND _hwnd;

    private bool _isHandleCreationAllowed = true;

    private WNDPROC? _wndProc;

    public abstract string ClassName { get; }

    public static int LastDestroyWindowError { get; private set; }

    protected ushort WindowClassAtom { get; private set; }

    public IntPtr Handle => Hwnd;

    internal HWND Hwnd
    {
        get
        {
            EnsureHandle();
            return _hwnd;
        }
    }

    protected virtual bool IsWindowSubClassed => false;

    protected virtual ushort CreateWindowClassCore()
    {
        return RegisterClass(ClassName);
    }

    protected virtual void DestroyWindowClassCore()
    {
        if (WindowClassAtom != 0)
        {
            var moduleHandle = PInvoke.GetModuleHandle((string?)null);
            PInvoke.UnregisterClass(ClassName, moduleHandle);
            WindowClassAtom = 0;
        }
    }

    protected unsafe ushort RegisterClass(string className)
    {
        _wndProc = WndProcWrapper;

        fixed (char* cls = className)
        {
            var lpWndClass = new WNDCLASSEXW
            {
                cbSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEXW)),
                hInstance = PInvoke.GetModuleHandle((PCWSTR)null),
                lpfnWndProc = _wndProc,
                lpszClassName = cls,
            };

            var atom = PInvoke.RegisterClassEx(lpWndClass);

            return atom;
        }
    }

    private void SubclassWndProc()
    {
        _wndProc = WndProcWrapper;
        PInvoke.SetWindowLongPtr(Hwnd, WINDOW_LONG_PTR_INDEX.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(_wndProc));
    }

    protected abstract IntPtr CreateWindowCore();

    protected virtual void DestroyWindowCore()
    {
        if (_hwnd != IntPtr.Zero)
        {
            if (PInvoke.DestroyWindow(_hwnd) == false)
            {
                LastDestroyWindowError = Marshal.GetLastWin32Error();
            }

            _hwnd = default;
        }
    }

    private LRESULT WndProcWrapper(HWND hwnd, uint msg, WPARAM wParam, LPARAM lParam)
    {
        return new LRESULT(WndProc(hwnd, msg, wParam, lParam));
    }

    protected virtual nint WndProc(nint hwnd, uint msg, nuint wParam, nint lParam)
    {
        return PInvoke.DefWindowProc(new HWND(hwnd), msg, wParam, lParam);
    }

    public IntPtr EnsureHandle()
    {
        if (_hwnd != IntPtr.Zero)
        {
            return _hwnd;
        }

        if (_isHandleCreationAllowed == false)
        {
            return IntPtr.Zero;
        }

        if (IsDisposed)
        {
            return IntPtr.Zero;
        }

        _isHandleCreationAllowed = false;
        WindowClassAtom = CreateWindowClassCore();
        _hwnd = new HWND(CreateWindowCore());

        if (IsWindowSubClassed)
        {
            SubclassWndProc();
        }

        return _hwnd;
    }

    protected override void DisposeNativeResources()
    {
        _isHandleCreationAllowed = false;
        DestroyWindowCore();
        DestroyWindowClassCore();
    }
}
