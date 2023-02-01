using RxFramework.Sandbox.TestPlugin;

namespace RxFramework.Sandbox.WPF.ViewModels;
public class ShellViewModel : ViewModelBase
{
    public ShellViewModel(IService service)
    {
        service.WriteToConsole("ShellVm created");
    }
}
