using ReactiveFramework;
using ReactiveFramework.ReactiveProperty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPlugin.ViewModels;
public class ViewBViewModel : NavigableViewModel<string>
{

    public RxProperty<string> Message { get; } = new("Default Message");

    public override void OnNavigatedTo(string parameter)
    {
        Message.Value = parameter;
    }
}
