using ReactiveFramework;
using ReactiveFramework.Commands;
using ReactiveFramework.ReactiveProperty;

namespace WpfPlugin.ViewModels;
public class ViewAViewModel : NavigableViewModel
{
    public RxProperty<string> Message { get; } = new();

    public RxCommand NavigateToViewBCommand { get; }

    public ViewAViewModel(INavigationService navigationService)
    {
        NavigateToViewBCommand = RxCommand.Create(() => navigationService.NavigateTo<ViewBViewModel, string>("PageViews", Message));
    }

    public override void OnNavigatedFrom()
    {
        // Do Some stuff after navigating away from this view.
    }

    public override void OnNavigatedTo()
    {
        // Do Some stuff after navigating to this view.
        Message.Value = string.Empty;
    }
}
