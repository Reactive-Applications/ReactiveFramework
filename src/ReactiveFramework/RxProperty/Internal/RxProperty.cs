using ReactiveFramework.Extensions;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Security.Cryptography.X509Certificates;

namespace ReactiveFramework.RxProperty.Internal;
public class RxProperty<T> : IRxProperty<T>
{
    private T _value;
    private List<IObserver<T>> _observers = new();
    private IEqualityComparer<T> _comparer;
    private IDisposable _sourceDisposable;

    public event PropertyChangedEventHandler? PropertyChanged;

    public T Value
    {
        get => _value;
        set => SetValue(value);
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
    public RxPropertyOptions Options { get; }

    public static implicit operator T(RxProperty<T> property)
        => property.Value;

    public RxProperty(T initialValue)
        : this(initalValue: initialValue)
    {

    }

    public RxProperty(T initalValue,
            IScheduler? eventScheduler = null,
            RxPropertyOptions? options = null,
            IEqualityComparer<T>? equalityComparer = null)
    {
        EventScheduler = eventScheduler ?? Schedulers.MainScheduler;
        Options = options ?? RxPropertyOptions.Default;
        
        _value = initalValue;
        _comparer = equalityComparer ?? EqualityComparer<T>.Default;
        _sourceDisposable = Disposable.Empty;
    }

    public RxProperty(IObservable<T> source, T initialValue,
            IScheduler? eventScheduler = null,
            RxPropertyOptions? options = null,
            IEqualityComparer<T>? equalityComparer = null)
        : this(initialValue, eventScheduler, options, equalityComparer)
    {
        _sourceDisposable = source.Subscribe(x => Value = x);
    }

    public IDisposable Subscribe(IObserver<T> observer)
    {
        _observers.Add(observer);

        if (Options.RaiseLatestValueOnSubscribe)
        {
            observer.OnNext(_value);
        }

        return Disposable.Create(() => Unsubscribe(observer));
    }

    public override string? ToString()
    {
        return Value?.ToString();
    }

    public void NotifyChanged()
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

    private void Unsubscribe(IObserver<T> observer)
    {
        _observers.Remove(observer);
    }

    protected virtual void SetValue(T value)
    {
        if (Options.DistinctUntilChanged && _comparer.Equals(_value, value) || IsDisposed)
        {
            return;
        }
        _value = value;

        RaiseValueChanged(ref _value);

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

internal class RxProperty<T, TTransformed> : RxProperty<T>, IRxProperty<T, TTransformed>
{
    private TTransformed _tValue;

    private readonly Func<T, TTransformed> _transformFunc;
    private readonly Func<TTransformed, T> _transformBackFunc;

    public RxProperty(
        T initialValue,
        Func<T, TTransformed> transformFunc,
        Func<TTransformed, T> transformBackFunc,
        IScheduler? eventScheduler = null,
        RxPropertyOptions? options = null,
        IEqualityComparer<T>? equalityComparer = null) 
        : base(initialValue, eventScheduler, options, equalityComparer)
    {
        _transformFunc = transformFunc;
        _transformBackFunc = transformBackFunc;
        _tValue = _transformFunc(initialValue);
    }

    public RxProperty(
        TTransformed initialValue,
        Func<T, TTransformed> transformFunc,
        Func<TTransformed, T> transformBackFunc,
        IScheduler? eventScheduler = null,
        RxPropertyOptions? options = null,
        IEqualityComparer<T>? equalityComparer = null)
        : base(transformBackFunc(initialValue), eventScheduler, options, equalityComparer)
    {
        _transformFunc = transformFunc;
        _transformBackFunc = transformBackFunc;
        _tValue = initialValue;
    }



    public TTransformed TransformedValue 
    {
        get => _tValue;
        set => SetValue(_transformBackFunc(value));
    }

    public IDisposable Subscribe(IObserver<TTransformed> observer)
    {
        return this.Subscribe<T>(
            v => 
            {
                _tValue = _transformFunc(v);
                observer.OnNext(_tValue);
            },
            observer.OnError,
            observer.OnCompleted);
    }
}
