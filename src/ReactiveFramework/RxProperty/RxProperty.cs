using System.Reactive.Concurrency;

namespace ReactiveFramework.RxProperty;
public static class RxProperty
{

    public static IRxProperty<T> Create<T>()
        where T : struct
    {
        return new Internal.RxProperty<T>(default);
    }

    public static IRxProperty<T> Create<T>(
        IScheduler? eventscheduler = null,
        RxPropertyOptions? options = null,
        IEqualityComparer<T>? equalityComparer = null)
        where T : struct
    {
        return new Internal.RxProperty<T>(default, eventscheduler, options, equalityComparer);
    }

    public static IRxProperty<T> Create<T>(T initialValue)
    {
        return new Internal.RxProperty<T>(initialValue);
    }

    public static IRxProperty<T> Create<T>(
        T initialValue,
        IScheduler? eventscheduler = null,
        RxPropertyOptions? options = null,
        IEqualityComparer<T>? equalityComparer = null)
    {
        return new Internal.RxProperty<T>(initialValue, eventscheduler, options, equalityComparer);
    }

    public static IRxProperty<T> ToRxProperty<T>(this IObservable<T> observable)
        where T : struct
    {
        return new Internal.RxProperty<T>(observable, default);
    }

    public static IRxProperty<T> ToRxProperty<T>(
        this IObservable<T> observable,
        IScheduler? eventscheduler = null,
        RxPropertyOptions? options = null,
        IEqualityComparer<T>? equalityComparer = null)
        where T : struct
    {
        return new Internal.RxProperty<T>(observable, default, eventscheduler, options, equalityComparer);
    }

    public static IRxProperty<T> ToRxProperty<T>(this IObservable<T> observable, T initalValue)
    {
        return new Internal.RxProperty<T>(observable, initalValue);
    }

    public static IRxProperty<T> ToRxProperty<T>(
        this IObservable<T> observable,
        T initalValue,
        IScheduler? eventscheduler = null,
        RxPropertyOptions? options = null,
        IEqualityComparer<T>? equalityComparer = null)
    {
        return new Internal.RxProperty<T>(observable, initalValue, eventscheduler, options, equalityComparer);
    }

    public static IReadOnlyRxProperty<T> ToReadOnlyRxProperty<T>(this IObservable<T> observable)
        where T : struct
    {
        return observable.ToRxProperty();
    }

    public static IReadOnlyRxProperty<T> ToReadOnlyRxProperty<T>(
        this IObservable<T> observable,
        IScheduler? eventscheduler = null,
        RxPropertyOptions? options = null,
        IEqualityComparer<T>? equalityComparer = null)
        where T : struct
    {
        return observable.ToReadOnlyRxProperty(eventscheduler, options, equalityComparer);
    }

    public static IReadOnlyRxProperty<T> ToReadOnlyRxProperty<T>(this IObservable<T> observable, T initalValue)
    {
        return observable.ToRxProperty(initalValue);
    }

    public static IRxProperty<T> ToReadOnlyRxProperty<T>(
        this IObservable<T> observable,
        T initalValue,
        IScheduler? eventscheduler = null,
        RxPropertyOptions? options = null,
        IEqualityComparer<T>? equalityComparer = null)
    {
        return observable.ToRxProperty(initalValue, eventscheduler, options, equalityComparer);
    }

    public static IRxProperty<T, TTransformed> Create<T, TTransformed>(
        Func<T, TTransformed> transfromFunc,
        Func<TTransformed, T> transfromBackFunc)
        where T : struct
    {
        return new Internal.RxProperty<T, TTransformed>(default(T), transfromFunc, transfromBackFunc);
    }

    public static IRxProperty<T, TTransformed> Create<T, TTransformed>(
        Func<T, TTransformed> transfromFunc,
        Func<TTransformed, T> transfromBackFunc,
        IScheduler? eventscheduler = null,
        RxPropertyOptions? options = null,
        IEqualityComparer<T>? equalityComparer = null)
        where T : struct
    {
        return new Internal.RxProperty<T, TTransformed>(
            default(T),
            transfromFunc,
            transfromBackFunc,
            eventscheduler,
            options,
            equalityComparer);
    }

    public static IRxProperty<T, TTransformed> CreateDefaultTransformed<T, TTransformed>(
        Func<T, TTransformed> transfromFunc,
        Func<TTransformed, T> transfromBackFunc)
        where TTransformed : struct
    {
        return new Internal.RxProperty<T, TTransformed>(default(TTransformed), transfromFunc, transfromBackFunc);
    }

    public static IRxProperty<T, TTransformed> CreateDefaultTransformed<T, TTransformed>(
        Func<T, TTransformed> transfromFunc,
        Func<TTransformed, T> transfromBackFunc,
        IScheduler? eventscheduler = null,
        RxPropertyOptions? options = null,
        IEqualityComparer<T>? equalityComparer = null)
        where TTransformed : struct
    {
        return new Internal.RxProperty<T, TTransformed>(
            default(TTransformed),
            transfromFunc,
            transfromBackFunc,
            eventscheduler,
            options,
            equalityComparer);
    }

    public static IRxProperty<T, TTransformed> Create<T, TTransformed>(
       T initialValue,
       Func<T, TTransformed> transfromFunc,
       Func<TTransformed, T> transfromBackFunc)
    {
        return new Internal.RxProperty<T, TTransformed>(initialValue, transfromFunc, transfromBackFunc);
    }

    public static IRxProperty<T, TTransformed> Create<T, TTransformed>(
          T initialValue,
          Func<T, TTransformed> transfromFunc,
          Func<TTransformed, T> transfromBackFunc,
          IScheduler? eventscheduler = null,
          RxPropertyOptions? options = null,
          IEqualityComparer<T>? equalityComparer = null)
    {
        return new Internal.RxProperty<T, TTransformed>(
            initialValue,
            transfromFunc,
            transfromBackFunc,
            eventscheduler,
            options,
            equalityComparer);
    }

    public static IRxProperty<T, TTransformed> Create<T, TTransformed>(
       TTransformed initialValue,
       Func<T, TTransformed> transfromFunc,
       Func<TTransformed, T> transfromBackFunc)
    {
        return new Internal.RxProperty<T, TTransformed>(initialValue, transfromFunc, transfromBackFunc);
    }

    public static IRxProperty<T, TTransformed> Create<T, TTransformed>(
          TTransformed initialValue,
          Func<T, TTransformed> transfromFunc,
          Func<TTransformed, T> transfromBackFunc,
          IScheduler? eventscheduler = null,
          RxPropertyOptions? options = null,
          IEqualityComparer<T>? equalityComparer = null)
    {
        return new Internal.RxProperty<T, TTransformed>(
            initialValue,
            transfromFunc,
            transfromBackFunc,
            eventscheduler,
            options,
            equalityComparer);
    }
}
