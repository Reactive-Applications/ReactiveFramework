namespace ReactiveFramework;
public interface IViewCompositionService
{
    void InsertView<TViewModel>(object containerKey)
        where TViewModel : IViewModel;

    void InsertView<TViewModel>(TViewModel viewModel, object containerKey)
        where TViewModel : IViewModel;

    void InsertView(Type viewMdoel, object containerKey);

    void RemoveView<TViewModel>(object containerKey)
        where TViewModel : IViewModel;

    void RemoveView<T>(T viewModel, object containerKey)
        where T : IViewModel;

    void RemoveView(Type viewModel, object containerKey);
}
