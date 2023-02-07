namespace ReactiveFramework;
public interface IViewCompositionService
{
    void InsertView<TViewModel>(object containerKey)
        where TViewModel : IViewModel;

    void RemoveView<TViewModel>(object containerKey)
        where TViewModel : IViewModel;
}
