using ReactiveFramework;
using ReactiveFramework.RxProperty;

namespace WpfPlugin.ViewModels;
public class ViewBViewModel : NavigableViewModel<string>
{

    public IRxProperty<string> Message { get; } = RxProperty.Create("Default Message");

    public override void OnNavigatedTo(string parameter)
    {
        Message.Value = parameter;
    }
}
