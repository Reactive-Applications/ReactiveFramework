using System.ComponentModel;

namespace ReactiveFramework.RxProperty;

public interface IReadOnlyRxProperty : INotifyPropertyChanged
{
    object? Value { get; }

    RxPropertyOptions Options { get; }
}

public interface IReadOnlyRxProperty<T> : IReadOnlyRxProperty, IObservable<T>, IDisposable
{
    new T Value { get; }

    bool IsDisposed { get; }
}
