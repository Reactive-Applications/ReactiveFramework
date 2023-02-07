using ReactiveFramework;

public abstract class ViewModelBase : IViewModel
{

    public virtual void OnViewLoaded()
    {
    }

    public virtual void OnViewStateChanged(ViewState oldState, ViewState newState) { }

}