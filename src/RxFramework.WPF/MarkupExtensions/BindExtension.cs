using RxFramework.ReactiveProperty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RxFramework.WPF.MarkupExtensions;
public class BindExtension : BindingExtensionBase
{    
    public BindExtension()
    {
        
    }

    public BindExtension(PropertyPath path)
        :base(path)
    {
        
    }

    protected override void PostBinding(FrameworkElement? dataContextSource, DependencyObject targetObject, DependencyProperty targetProperty, Binding binding, BindingExpressionBase expression)
    {
        if(dataContextSource is null)
        {
            return;
        }

        dataContextSource.DataContextChanged += (s,e) => DataContextChanged(dataContextSource.DataContext, targetObject, targetProperty, binding);
    }

    private void DataContextChanged(object dataContext, DependencyObject targetObject, DependencyProperty targetProperty, Binding binding)
    {
        var path = Path.Path;
        if(GetObjectFromPath(dataContext, Path.Path) is IRxProperty)
        {
            path = Path.Path + ".Value";
        }

        BindingOperations.ClearBinding(targetObject, targetProperty);
        var newBinding = new Binding(path)
        {
            Converter = Converter,
            ConverterCulture = ConverterCulture,
            ConverterParameter = ConverterParameter,
            ValidatesOnDataErrors = ValidatesOnDataErrors,
            ValidatesOnExceptions = ValidatesOnExceptions,
            Mode = Mode,
            UpdateSourceTrigger = UpdateSourceTrigger,
            FallbackValue = FallBackValue
        };

        BindingOperations.SetBinding(targetObject, targetProperty, newBinding);
    }
}
