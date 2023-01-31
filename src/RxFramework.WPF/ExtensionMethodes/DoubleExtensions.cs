using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RxFramework.WPF.ExtensionMethodes;
public static class DoubleExtensions
{
    public static double Clip(this double value, double minValue, double maxValue)
    {
        return value > maxValue ? maxValue : value < minValue ? minValue : value;
    }
}
