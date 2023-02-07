using ReactiveFramework.Commands;
using System.Reactive;

namespace ReactiveFramework.Dialog;
public interface IDialogViewModel : IViewModel
{
    IObservable<bool> CanCloseDialog { get; }

    IRxCommand CloseCommand { get; }
}

public interface IDialogViewModel<TResult> : IDialogViewModel
{
    new IRxCommand<TResult> CloseCommand { get; }
}
