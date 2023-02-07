using System.ComponentModel;
using System.Linq.Expressions;

namespace ReactiveFramework.Extensions;

public static class INotifyPropertyChangedExtensions
{
    public static void Subscribe<T, TProperty>(this T notifier, Expression<Func<T, TProperty>> propertyExpression, Action<TProperty> callback)
        where T : INotifyPropertyChanged
    {
        var name = propertyExpression.GetPropertyName();
        var reader = propertyExpression.Compile();
        notifier.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == name)
            {
                callback(reader(notifier));
            }
        };
    }
}
