using RxFramework.Sandbox.WPF.ViewModels;
using RxFramework.WPF.Attributes;
using System.Windows;

namespace RxFramework.Sandbox.WPF.Views;
/// <summary>
/// Interaktionslogik für Shell.xaml
/// </summary>
[Shell]
[ViewFor<ShellViewModel>]
public partial class Shell : Window
{
    public Shell()
    {
        InitializeComponent();
    }
}
