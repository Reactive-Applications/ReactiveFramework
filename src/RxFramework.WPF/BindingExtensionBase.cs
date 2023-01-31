using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.Windows.Markup;

namespace RxFramework.WPF;
public abstract class BindingExtensionBase : MarkupExtension
{

    protected BindingExtensionBase()
    {
    }

    protected BindingExtensionBase(PropertyPath path)
    {
        Path = path;
    }

    [ConstructorArgument("path")]
    public PropertyPath Path { get; set; }
    public object? FallBackValue { get; set; }
    public IValueConverter? Converter { get; set; }
    public object? ConverterParameter { get; set; }
    public string? ElementName { get; set; }
    public RelativeSource? RelativeSource { get; set; }
    public object? Source { get; set; }
    public bool ValidatesOnDataErrors { get; set; }
    public bool ValidatesOnExceptions { get; set; }
    public BindingMode Mode { get; set; } = BindingMode.Default;
    public UpdateSourceTrigger UpdateSourceTrigger { get; set; } = UpdateSourceTrigger.Default;

    [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
    public CultureInfo ConverterCulture { get; set; } = CultureInfo.CurrentCulture;

    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        var pvt = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (pvt == null)
        {
            return null;
        }

        var targetObject = pvt.TargetObject as DependencyObject;
        if (targetObject == null)
        {
            return null;
        }

        var targetProperty = pvt.TargetProperty as DependencyProperty;
        if (targetProperty == null)
        {
            return null;
        }

        var binding = new Binding
        {
            Path = Path,
            Converter = Converter,
            ConverterCulture = ConverterCulture,
            ConverterParameter = ConverterParameter,
            ValidatesOnDataErrors = ValidatesOnDataErrors,
            ValidatesOnExceptions = ValidatesOnExceptions,
            Mode = Mode,
            UpdateSourceTrigger = UpdateSourceTrigger,
        };

        if (ElementName != null)
        {
            binding.ElementName = ElementName;
        }

        if (RelativeSource != null)
        {
            binding.RelativeSource = RelativeSource;
        }
        var dtSource = FindDataContextSource(targetObject);
        
        if (Source != null)
        {
            binding.Source = Source;
            dtSource = null;
        }

        if(FallBackValue != null)
        {
            binding.FallbackValue = FallBackValue;
        }

        var expression = BindingOperations.SetBinding(targetObject, targetProperty, binding);

        PostBinding(dtSource, targetObject, targetProperty, binding, expression);

        return targetObject.GetValue(targetProperty);
    }

    protected abstract void PostBinding(FrameworkElement? dataContextSrc, DependencyObject targetObject,
     DependencyProperty targetProperty, Binding binding,
     BindingExpressionBase expression);

    protected object? GetObjectFromPath(object dataContext, string path)
    {
        var properties = path.Split('.');
        var current = dataContext;

        foreach (var prop in properties)
        {
            if (current == null)
            {
                break;
            }

            current = current.GetType()?.GetProperty(prop)?.GetValue(current);
        }

        return current;
    }

    private FrameworkElement FindDataContextSource(DependencyObject d)
    {
        DependencyObject current = d;
        while (current is not FrameworkElement && current != null)
        {
            current = LogicalTreeHelper.GetParent(d);
        }

        return (FrameworkElement)current!;
    }
}
