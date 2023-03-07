using ReactiveFramework;
using ReactiveFramework.Commands;
using ReactiveFramework.RxProperty;
using System.Reactive.Linq;

namespace WpfPlugin.ViewModels;
public class ViewAViewModel : NavigableViewModel
{
    public IRxProperty<string> Message { get; }

    public RxCommand NavigateToViewBCommand { get; }

    public ViewAViewModel(INavigationService navigationService)
    {
        Message = RxProperty.Create(string.Empty);

        // Executes navigation to View B. 
        // Can only be executed if Message is not empty.
        NavigateToViewBCommand = RxCommand.Create(
            () => navigationService.NavigateTo<ViewBViewModel, string>("PageViews", Message.Value),
            Message.Select(s => !string.IsNullOrWhiteSpace(s)));
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
