namespace ReactiveFramework.ReactiveProperty;

public interface IRxProperty : IReadOnlyRxProperty
{
    new object? Value { get; set; }

    void ForceNotify();
}

public interface IRxProperty<T> : IRxProperty, IReadOnlyRxProperty<T>, IDisposable
{
    new T Value { get; set; }
}
