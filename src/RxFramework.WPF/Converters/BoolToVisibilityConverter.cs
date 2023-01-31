using System.Globalization;

namespace RxFramework.WPF.Converters;

public class BoolToVisibilityConverter : BaseValueConverter<BoolToVisibilityConverter, bool>
{
    public override object Convert(bool value, Type targetType, object parameter, CultureInfo culture)
    {
        var invert = false;
        if (parameter != null)
        {
            bool.TryParse(parameter.ToString(), out invert);
        }

        if (invert)
        {
            value = !value;
        }

        return value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

    }

    public override object ConvertBack(bool value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
