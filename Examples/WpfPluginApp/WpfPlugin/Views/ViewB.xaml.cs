using ReactiveFramework.WPF.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfPlugin.ViewModels;

namespace WpfPlugin.Views;
/// <summary>
/// Interaktionslogik für ViewB.xaml
/// </summary>
[ViewFor<ViewBViewModel>]
public partial class ViewB
{
    public ViewB()
    {
        InitializeComponent();
    }
}
