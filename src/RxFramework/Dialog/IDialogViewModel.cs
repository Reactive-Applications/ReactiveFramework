using RxFramework.Commands;
using System.Reactive;

namespace RxFramework.Dialog;
public interface IDialogViewModel : IViewModel
{
    IObservable<bool> CanCloseDialog { get; }

    IRxCommand CloseCommand { get; }
}

public interface IDialogViewModel<TResult> : IDialogViewModel
{
    new IRxCommand<TResult> CloseCommand { get; }
} 
