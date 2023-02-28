using ReactiveFramework;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Runtime.ExceptionServices;

namespace ReactiveFramework.ReactiveProperty;
public class ReadOnlyRxProperty<T> : IReadOnlyRxProperty<T>, IObserver<T>
{

    private IEqualityComparer<T> _comparer;
    private List<IObserver<T>> _observers = new();

    private IScheduler _eventScheduler;
    private IDisposable _sourceDisposable;

    private bool RaiseLatestValueOnSubscribe => (Settings & RxPropertySettings.RaiseLatestValueOnSubscribe) == RxPropertySettings.RaiseLatestValueOnSubscribe;
    private bool DistinctUntilChanged => (Settings & RxPropertySettings.DistinctUntilChanged) == RxPropertySettings.DistinctUntilChanged;

    public T Value { get; private set; }

    public RxPropertySettings Settings { get; }

    object? IReadOnlyRxProperty.Value => Value;

    public bool IsDisposed { get; private set; }

    public event PropertyChangedEventHandler? PropertyChanged;


    public ReadOnlyRxProperty(T initalValue,
        IScheduler? eventScheduler = null,
        RxPropertySettings settings = RxPropertySettings.Default,
        IEqualityComparer<T>? equalityComparer = null)
    {
        Value = initalValue;
        _comparer = equalityComparer ?? EqualityComparer<T>.Default;
        _eventScheduler = eventScheduler ?? Schedulers.MainScheduler;
        _sourceDisposable = Disposable.Empty;
        Settings = settings;

    }

    public ReadOnlyRxProperty(IObservable<T> source,
        IScheduler? eventScheduler = null,
        RxPropertySettings settings = RxPropertySettings.Default,
        IEqualityComparer<T>? equalityComparer = null)
        : this(default(T)!, eventScheduler, settings, equalityComparer)
    {
        _sourceDisposable = source.Subscribe(this);
    }

    public ReadOnlyRxProperty(IObservable<T> source,
        T initalValue,
        IScheduler? eventScheduler = null,
        RxPropertySettings settings = RxPropertySettings.Default,
        IEqualityComparer<T>? equalityComparer = null)
        : this(initalValue, eventScheduler, settings, equalityComparer)
    {
        _sourceDisposable = source.Subscribe(this);
    }

    public void OnCompleted()
    {
        Dispose();
    }

    public void OnError(Exception error)
    {
        ExceptionDispatchInfo.Capture(error).Throw();
    }

    public void OnNext(T value)
    {
        if (IsDisposed)
        {
            return;
        }

        if (DistinctUntilChanged && _comparer.Equals(Value, value))
        {
            return;
        }

        Value = value;

        foreach (var observer in _observers)
        {
            observer.OnNext(value);
        }

        _eventScheduler.Schedule(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(value))));
    }

    public IDisposable Subscribe(IObserver<T> observer)
    {
        if (IsDisposed)
        {
            observer.OnCompleted();
        }

        if (RaiseLatestValueOnSubscribe)
        {
            observer.OnNext(Value);
        }

        return Disposable.Create(() => DisposeObserver(observer));
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

            _observers = null;
            _sourceDisposable = null;
        }

        IsDisposed = true;
    }

    private void DisposeObserver(IObserver<T> observer)
    {
        _observers.Remove(observer);
    }
}
public static class ReadOnlyRxProperty
{

    public static ReadOnlyRxProperty<T> ToReadOnlyReactiveProperty<T>(this IObservable<T> source,
        T initialValue,
        IScheduler? eventScheduler = null,
        RxPropertySettings settings = RxPropertySettings.Default,
        IEqualityComparer<T>? equalityComparer = null)
    {
        return new ReadOnlyRxProperty<T>(source, initialValue, eventScheduler, settings, equalityComparer);
    }

    public static ReadOnlyRxProperty<T> ToReadOnlyReactiveProperty<T>(this IObservable<T> source,
        IScheduler? eventScheduler = null,
        RxPropertySettings settings = RxPropertySettings.Default,
        IEqualityComparer<T>? equalityComparer = null)
    {
        return new ReadOnlyRxProperty<T>(source, default!, eventScheduler, settings, equalityComparer);
    }
}