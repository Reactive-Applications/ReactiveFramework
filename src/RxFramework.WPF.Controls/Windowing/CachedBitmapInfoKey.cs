using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxFramework.WPF.FluentControls.Windowing;
internal readonly struct CachedBitmapInfoKey : IEquatable<CachedBitmapInfoKey>
{
    public CachedBitmapInfoKey(int glowDepth, bool useRadialGradientForCorners)
    {
        GlowDepth = glowDepth;
        UseRadialGradientForCorners = useRadialGradientForCorners;
    }

    public int GlowDepth { get; }

    public bool UseRadialGradientForCorners { get; }

    public bool Equals(CachedBitmapInfoKey other)
    {
        return GlowDepth == other.GlowDepth && UseRadialGradientForCorners == other.UseRadialGradientForCorners;
    }

    public override bool Equals(object? obj)
    {
        return obj is CachedBitmapInfoKey other
               && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return GlowDepth * 397 ^ UseRadialGradientForCorners.GetHashCode();
        }
    }

    public static bool operator ==(CachedBitmapInfoKey left, CachedBitmapInfoKey right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CachedBitmapInfoKey left, CachedBitmapInfoKey right)
    {
        return !left.Equals(right);
    }
}