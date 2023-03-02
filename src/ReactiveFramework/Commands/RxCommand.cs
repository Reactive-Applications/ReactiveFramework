using ReactiveFramework;
using System.ComponentModel.Design;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Windows.Input;

namespace ReactiveFramework.Commands;


public class RxCommand<TParam, TResult> : IRxCommand<TParam, TResult>
{
    private bool _canExecuteValue;
    private readonly IObservable<bool> _canExecute;
    private readonly IDisposable _canExecuteSubscribtion;

    private readonly Func<TParam, IObservable<TResult>> _execute;

    private readonly Subject<ExecutionInfo> _executionInfo;

    private readonly IObservable<bool> _isExecuting;
    private readonly IScheduler _outputScheduler;
    private readonly IObservable<TResult> _results;
    private readonly ISubject<ExecutionInfo, ExecutionInfo> _synchronizedExecutionInfo;

    public event EventHandler? CanExecuteChanged;

    public IObservable<bool> CanExecute => _canExecute;

    public IObservable<bool> IsExecuting => _isExecuting;
    public bool IsDisposed { get; private set; }

    internal RxCommand(Func<TParam, IObservable<TResult>> execute,
        IObservable<bool> canExecute,
        IScheduler? outputScheduler = null)
    {
        ArgumentNullException.ThrowIfNull(execute);
        ArgumentNullException.ThrowIfNull(canExecute);

        _execute = execute;
        _outputScheduler = outputScheduler ?? Schedulers.TaskPoolScheduler;
        _executionInfo = new Subject<ExecutionInfo>();
        _synchronizedExecutionInfo = Subject.Synchronize(_executionInfo, _outputScheduler);
        _isExecuting = _synchronizedExecutionInfo.Scan(0, (acc, next) =>
        {
            return next.Demarcation switch
            {
                ExecutionDemarcation.Begin => acc + 1,
                ExecutionDemarcation.End => acc - 1,
                _ => acc
            };
        }).Select(count => count > 0)
        .StartWith(false)
        .DistinctUntilChanged()
        .Replay(1)
        .RefCount();

        _canExecute = canExecute.CombineLatest(IsExecuting, (canEx, isEx) => canEx && !isEx)
            .DistinctUntilChanged()
            .Replay(1)
            .RefCount();

        _results = _synchronizedExecutionInfo
            .Where(x => x.Demarcation == ExecutionDemarcation.Result)
            .Select(x => x.Result);

        _canExecuteSubscribtion = _canExecute.ObserveOn(Schedulers.MainScheduler).Subscribe(b =>
        {
            _canExecuteValue = b;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        });

    }

    public IObservable<TResult> Execute(TParam param)
    {
        return Observable.Defer(() =>
       {
           _synchronizedExecutionInfo.OnNext(ExecutionInfo.CreateBegin());
           return Observable.Empty<TResult>();
       }).Concat(_execute(param))
       .Do(result => _synchronizedExecutionInfo.OnNext(ExecutionInfo.CreateResult(result)))
       .Finally(() => _synchronizedExecutionInfo.OnNext(ExecutionInfo.CreateEnd()))
       .PublishLast()
       .RefCount();
    }

    public IObservable<TResult> Execute()
    {
        return Execute(default!);
    }

    public IDisposable Subscribe(IObserver<TResult> observer)
    {
        return _results.Subscribe(observer);
    }


    bool ICommand.CanExecute(object? parameter)
       => _canExecuteValue;

    void ICommand.Execute(object? parameter)
    {
        parameter ??= default(TParam);

        if (parameter is not null && parameter is not TParam tparam)
        {
            throw new InvalidOperationException($"Command expected parameter of type {typeof(TParam).FullName}, but parameter was {parameter.GetType().FullName}");
        }

        var res = parameter is null ? Execute() : Execute((TParam)parameter);

        res.Subscribe();
    }

    #region IDisposable

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
            _canExecuteSubscribtion.Dispose();
        }

        IsDisposed = true;
    }

    #endregion

    private enum ExecutionDemarcation
    {
        Begin,

        Result,

        End
    }

    private readonly struct ExecutionInfo
    {
        private ExecutionInfo(ExecutionDemarcation demarcation, TResult result)
        {
            Demarcation = demarcation;
            Result = result;
        }

        public ExecutionDemarcation Demarcation { get; }

        public TResult Result { get; }

        public static ExecutionInfo CreateBegin() => new(ExecutionDemarcation.Begin, default!);

        public static ExecutionInfo CreateResult(TResult result) =>
            new(ExecutionDemarcation.Result, result);

        public static ExecutionInfo CreateEnd() => new(ExecutionDemarcation.End, default!);
    }
}

public class RxCommand<TResult> : RxCommand<Unit, TResult>, IRxCommand<TResult>
{
    internal RxCommand(Func<Unit, IObservable<TResult>> execute, IObservable<bool> canExecute, IScheduler? outputScheduler = null) : base(execute, canExecute, outputScheduler)
    {
    }
}

public class RxCommand : RxCommand<Unit, Unit>, IRxCommand
{
    internal RxCommand(Func<Unit, IObservable<Unit>> execute, IObservable<bool> canExecute, IScheduler? outputScheduler = null)
        : base(execute, canExecute, outputScheduler)
    {
    }

    public static RxCommand Create(Action execute,
        IObservable<bool>? canExecute = null,
        IScheduler? outputSchedulder = null)
    {
        ArgumentNullException.ThrowIfNull(execute);

        return new RxCommand(_ => Observable.Create<Unit>(o =>
        {
            execute();
            o.OnNext(Unit.Default);
            o.OnCompleted();
            return Disposable.Empty;
        }),
        canExecute ?? Observable.Return(true),
        outputSchedulder);
    }

    public static RxCommand CreateFromObservable(Func<IObservable<Unit>> execute,
        IObservable<bool>? canExecute = null,
        IScheduler? outputScheduler = null)
    {
        return new RxCommand(_ => execute(),
            canExecute ?? Observable.Return(true),
            outputScheduler);
    }

    public static RxCommand CreateFromTask(Func<Task> executeAsync,
        IObservable<bool>? canExecute = null,
        IScheduler? outputScheduler = null)
    {
        ArgumentNullException.ThrowIfNull(executeAsync);

        return CreateFromObservable(() => Observable.FromAsync(executeAsync), canExecute, outputScheduler);
    }

    public static RxCommand CreateFromTask(Func<CancellationToken, Task> executeAsync,
        IObservable<bool>? canExecute = null,
        IScheduler? outputScheduler = null)
    {
        ArgumentNullException.ThrowIfNull(executeAsync);

        return CreateFromObservable(() => Observable.FromAsync(executeAsync), canExecute, outputScheduler);
    }

    public static RxCommand<TResult> Create<TResult>(Func<TResult> execute,
        IObservable<bool>? canExecute = null,
        IScheduler? outputSchedulder = null)
    {
        ArgumentNullException.ThrowIfNull(execute);

        return new RxCommand<TResult>(_ => Observable.Create<TResult>(o =>
        {
            o.OnNext(execute());
            o.OnCompleted();
            return Disposable.Empty;
        }),
        canExecute ?? Observable.Return(true),
        outputSchedulder);
    }

    public static RxCommand<TResult> CreateFromObservable<TResult>(Func<IObservable<TResult>> execute,
        IObservable<bool>? canExecute = null,
        IScheduler? outputScheduler = null)
    {
        ArgumentNullException.ThrowIfNull(execute);

        return new RxCommand<TResult>(_ => execute(),
            canExecute ?? Observable.Return(true),
            outputScheduler);
    }

    public static RxCommand<TResult> CreateFromTask<TResult>(Func<Task<TResult>> executeAsync,
        IObservable<bool>? canExecute = null,
        IScheduler? outputScheduler = null)
    {
        return CreateFromObservable(() => executeAsync().ToObservable(), canExecute, outputScheduler);
    }

    public static RxCommand<TResult> CreateFromTask<TResult>(Func<CancellationToken, Task<TResult>> execute,
        IObservable<bool>? canExecute = null,
        IScheduler? outputScheduler = null)
    {
        ArgumentNullException.ThrowIfNull(execute);
        return CreateFromObservable(() => Observable.FromAsync(execute), canExecute, outputScheduler);
    }


    public static RxCommand<TParam, TResult> Create<TParam, TResult>(Func<TParam, TResult> execute,
        IObservable<bool>? canExecute = null,
        IScheduler? outputScheduler = null)
    {
        ArgumentNullException.ThrowIfNull(execute);

        return new RxCommand<TParam, TResult>(p => Observable.Create<TResult>(o =>
        {
            o.OnNext(execute(p));
            o.OnCompleted();
            return Disposable.Empty;
        }),
        canExecute ?? Observable.Return(true),
        outputScheduler);
    }

    public static RxCommand<TParam, TResult> CreateFromObservable<TParam, TResult>(Func<TParam, IObservable<TResult>> execute,
        IObservable<bool>? canExecute = null,
        IScheduler? outputScheduler = null)
    {

        ArgumentNullException.ThrowIfNull(execute);

        return new RxCommand<TParam, TResult>(execute,
            canExecute ?? Observable.Return(true), outputScheduler);
    }

    public static RxCommand<TParam, TResult> CreateFromTask<TParam, TResult>(Func<TParam, Task<TResult>> executeAsync,
        IObservable<bool>? canExecute = null,
        IScheduler? outputScheduler = null)
    {
        ArgumentNullException.ThrowIfNull(executeAsync);

        return CreateFromObservable<TParam, TResult>(p => executeAsync(p).ToObservable(),
            canExecute ?? Observable.Return(true),
            outputScheduler);
    }

    public static RxCommand<TParam, TResult> CreateFromTask<TParam, TResult>(Func<TParam, CancellationToken, Task<TResult>> executeAsync,
        IObservable<bool>? canExecute = null,
        IScheduler? outputScheduler = null)
    {
        ArgumentNullException.ThrowIfNull(executeAsync);

        return CreateFromObservable<TParam, TResult>(
            p => Observable.FromAsync(ct => executeAsync(p, ct)),
            canExecute ?? Observable.Return(true),
            outputScheduler);
    }
}
