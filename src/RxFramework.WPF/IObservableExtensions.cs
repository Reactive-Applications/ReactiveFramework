using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;

namespace RxFramework.WPF;
public static class IObservableExtensions
{
    public static IObservable<TProperty> Observe<TComponent, TProperty>(this TComponent component, DependencyProperty dependencyProperty)
    where TComponent : DependencyObject
    {
        return Observable.Create<TProperty>(observer =>
        {
            EventHandler update = (sender, args) => observer.OnNext((TProperty)((TComponent)sender).GetValue(dependencyProperty));
            var property = DependencyPropertyDescriptor.FromProperty(dependencyProperty, typeof(TComponent));
            property.AddValueChanged(component, update);
            return Disposable.Create(() => property.RemoveValueChanged(component, update));
        });
    }
}
