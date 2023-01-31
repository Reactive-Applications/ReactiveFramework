namespace RxFramework.Dialog;
public interface IDialogService
{
    T ShowDialog<T>()
        where T : IDialogViewModel;

    void ShowDialog<T>(T viewModel)
        where T : IDialogViewModel;

    TResult? ShowDialog<TViewModel, TResult>()
        where TViewModel : IDialogViewModel<TResult>;

    TResult? ShowDialog<TViewModel, TResult>(TViewModel viewModel)
        where TViewModel : IDialogViewModel<TResult>;

    Task<TResult?> ShowDialogAsync<TViewModel, TResult>()
        where TViewModel : IDialogViewModel<TResult>;

    Task<TResult?> ShowDialogAsync<TViewModel, TResult>(TViewModel viewModel)
        where TViewModel : IDialogViewModel<TResult>;
}
