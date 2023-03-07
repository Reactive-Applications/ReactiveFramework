namespace ReactiveFramework.RxProperty;

public interface IRxProperty : IReadOnlyRxProperty
{
    new object? Value { get; set; }

    void NotifyChanged();
}

public interface IRxProperty<T> : IRxProperty, IReadOnlyRxProperty<T>, IDisposable
{
    new T Value { get; set; }
}

public interface IRxProperty<T, TTransformed> : IDisposable, IObservable<TTransformed>
{
    T Value { get; set; }

    TTransformed TransformedValue { get; set; }
}
