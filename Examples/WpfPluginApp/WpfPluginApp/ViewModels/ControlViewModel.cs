using ReactiveFramework;
using ReactiveFramework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfPlugin.ViewModels;

namespace WpfPluginApp.ViewModels;
public class ControlViewModel : ViewModelBase
{
    public RxCommand NavigateToViewACommand { get; }

    public ControlViewModel(INavigationService navigationService)
    {
        NavigateToViewACommand = RxCommand.Create(() => navigationService.NavigateTo<ViewAViewModel>("PageViews"));
    }
}
