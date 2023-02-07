using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ReactiveFramework.WPF.Converters;

public class WindowIconConverter : BaseValueConverter<WindowIconConverter, ImageSource>
{
    public override object Convert(ImageSource value, Type targetType, object parameter, CultureInfo culture)
    {
        var size = Size.Empty;

        if (parameter is double || parameter is int || parameter is string)
        {
            var sizeVal = System.Convert.ToDouble(parameter);
            size = new Size(sizeVal, sizeVal);
        }
        else if (parameter is Size sizeVal)
        {
            size = sizeVal;
        }

        return CreateIcon(value, size, targetType);
    }

    private object CreateIcon(ImageSource value, Size size, Type targetType)
    {
        var img = new Image
        {
            Source = value,
            Stretch = Stretch.Uniform
        };

        if (!size.IsEmpty)
        {
            img.Width = size.Width;
            img.Height = size.Height;
        }

        return img;
    }

    public override object ConvertBack(ImageSource value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}
