using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace ReactiveFramework.WPF.Converters;
public abstract class BaseValueConverter<TConverter, TFor> : MarkupExtension, IValueConverter
        where TConverter : BaseValueConverter<TConverter, TFor>, new()
{

    private static readonly TConverter _converter;

    static BaseValueConverter()
    {
        _converter = new TConverter();
    }


    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return _converter;
    }

    public abstract object Convert(TFor value, Type targetType, object parameter, CultureInfo culture);


    public abstract object ConvertBack(TFor value, Type targetType, object parameter, CultureInfo culture);

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is TFor forValue
            ? Convert(forValue, targetType, parameter, culture)
            : throw new ArgumentException($"Value has to be an {typeof(TFor)}");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is TFor forValue
            ? Convert(forValue, targetType, parameter, culture)
            : throw new ArgumentException($"Value has to be an {typeof(TFor)}");
    }
}

public abstract class BaseValueConverter<TConverter> : MarkupExtension, IValueConverter
    where TConverter : BaseValueConverter<TConverter>, new()
{

    private static readonly TConverter _converter;

    static BaseValueConverter()
    {
        _converter = new TConverter();
    }

    public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

    public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return _converter;
    }

}
