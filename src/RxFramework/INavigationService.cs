namespace RxFramework;
public interface INavigationService
{
    uint BackStackCapacity { get; set; }

    void NavigateTo<TViewModel>(object containerKey)
        where TViewModel : INavigableViewModel;

    void NavigateTo<TViewModel>(object containerKey, TViewModel viewModel)
        where TViewModel : INavigableViewModel;

    void NavigateTo<TViewModel, TParameter>(object containerKey, TParameter parameter)
        where TViewModel : INavigableViewModel<TParameter>;

    IObservable<bool> CanNavigateBack(object containerKey);
    IObservable<bool> CanNavigateForward(object containerKey);

    void NavigateBack(object containerKey);

    void NavigateForward(object containerKey);
}