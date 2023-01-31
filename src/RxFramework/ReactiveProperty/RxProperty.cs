using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace RxFramework.ReactiveProperty;
public class RxProperty<T> : IRxProperty<T>
{
    private T _value = default;
    private List<IObserver<T>> _observers = new();
    private IEqualityComparer<T> _comparer;
    private IDisposable _sourceDisposable;

    private bool RaiseLatestValueOnSubscribe => (Settings & RxPropertySettings.RaiseLatestValueOnSubscribe) == RxPropertySettings.RaiseLatestValueOnSubscribe;
    private bool DistinctUntilChanged => (Settings & RxPropertySettings.DistinctUntilChanged) == RxPropertySettings.DistinctUntilChanged;

    public event PropertyChangedEventHandler? PropertyChanged;

    public T Value
    {
        get => _value;
        set
        {
            if (DistinctUntilChanged && _comparer.Equals(_value, value))
            {
                return;
            }

            SetValue(value);
        }
    }
    public bool IsDisposed { get; private set; }

    public IScheduler EventScheduler { get; }

    object? IRxProperty.Value
    {
        get => Value;
        set => Value = (T)value!;
    }

    T IReadOnlyRxProperty<T>.Value => Value;

    object? IReadOnlyRxProperty.Value => Value;
    public RxPropertySettings Settings { get; }

    public static implicit operator T(RxProperty<T> property) 
        => property.Value;

    public RxProperty(T initialValue)
        : this(initalValue: initialValue)
    {

    }

    public RxProperty(IScheduler? eventScheduler = null,
            RxPropertySettings settings = RxPropertySettings.Default,
            IEqualityComparer<T>? equalityComparer = null)
        : this(default!, eventScheduler, settings, equalityComparer)
    {
    }

    public RxProperty(T initalValue,
            IScheduler? eventScheduler = null,
            RxPropertySettings settings = RxPropertySettings.Default,
            IEqualityComparer<T>? equalityComparer = null)
    {
        EventScheduler = eventScheduler ?? Schedulers.UiScheduler;
        _value = initalValue;
        Settings = settings;
        _comparer = equalityComparer ?? EqualityComparer<T>.Default;
        _sourceDisposable = Disposable.Empty;
    }

    public RxProperty(IObservable<T> source, T initialValue,
            IScheduler? eventScheduler = null,
            RxPropertySettings settings = RxPropertySettings.Default,
            IEqualityComparer<T>? equalityComparer = null)
        : this(initialValue, eventScheduler, settings, equalityComparer)
    {
        _sourceDisposable = source.Subscribe(x => Value = x);
    }

    public IDisposable Subscribe(IObserver<T> observer)
    {
        _observers.Add(observer);

        if (RaiseLatestValueOnSubscribe)
        {
            observer.OnNext(_value);
        }

        return Disposable.Create(() => DisposeObserver(observer));
    }

    public override string? ToString()
    {
        return Value?.ToString();
    }

    public void ForceNotify()
    {
        RaiseValueChanged(ref _value);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        if (disposing)
        {
            foreach (var observer in _observers)
            {
                observer.OnCompleted();
            }

            _sourceDisposable.Dispose();
        }

        IsDisposed = true;
    }

    private void DisposeObserver(IObserver<T> observer)
    {
        _observers.Remove(observer);
    }

    private void SetValue(T value)
    {
        _value = value;

        if (!IsDisposed)
        {
            RaiseValueChanged(ref value);
        }

    }

    private void RaiseValueChanged(ref T value)
    {
        foreach (var observer in _observers)
        {
            observer.OnNext(value);
        }

        EventScheduler.Schedule(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value))));
    }
}
