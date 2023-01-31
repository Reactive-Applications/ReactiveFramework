using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RxFramework.WPF.FluentControls.Windowing;
public interface IGlowWindow : IDisposable
{
    IntPtr Handle { get; }

    bool IsVisible { get; set; }

    bool IsActive { get; set; }

    Color ActiveGlowColor { get; set; }

    Color InactiveGlowColor { get; set; }

    int GlowDepth { get; set; }

    bool UseRadialGradientForCorners { get; set; }

    IntPtr EnsureHandle();

    void CommitChanges(IntPtr windowPosInfo);

    void UpdateWindowPos();
}