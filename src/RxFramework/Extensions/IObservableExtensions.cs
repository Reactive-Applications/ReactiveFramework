using RxFramework.Commands;
using RxFramework.ReactiveProperty;
using System.Reactive;
using System.Reactive.Concurrency;

namespace RxFramework.Extensions;
public static class IObservableExtensions
{
    public static RxCommand ToRxCommand(this IObservable<Unit> self, 
        IObservable<bool>? canExecute = null,
        IScheduler? outputScheduler = null)
    {
        return RxCommand.CreateFromObservable(() => self, canExecute, outputScheduler); 
    }

    public static RxCommand<TResult> ToRxCommand<TResult>(this IObservable<TResult> self,
        IObservable<bool>? canExecute = null,
        IScheduler? outputScheduler = null)
    {
        return RxCommand.CreateFromObservable(() => self, canExecute, outputScheduler);
    }

    public static RxProperty<T> ToRxProperty<T>(this IObservable<T> self, 
        IScheduler? scheduler = null,
        RxPropertySettings settings = RxPropertySettings.Default,
        IEqualityComparer<T>? equalityComparer = null
        )
    {
        return new RxProperty<T>(self, default!, scheduler, settings,equalityComparer);
    }

    public static RxProperty<T> ToRxProperty<T>(this IObservable<T> self,
        T initalValue,
        IScheduler? scheduler = null,
        RxPropertySettings settings = RxPropertySettings.Default,
        IEqualityComparer<T>? equalityComparer = null)
    {
        return new RxProperty<T>(self, initalValue, scheduler, settings,equalityComparer);
    }
}
