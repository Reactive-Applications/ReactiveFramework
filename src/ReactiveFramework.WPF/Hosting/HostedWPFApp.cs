using ReactiveFramework.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveFramework.WPF.Hosting;
public static class HostedWPFApp
{
    public static IWPFAppBuilder CreateDefaultBuilder()
    {
        return new WPFAppBuilder();
    }

    public static IWPFAppBuilder CreateDefaultBuilder(string[] args)
    {
        return new WPFAppBuilder(args);
    }

    public static IWPFAppBuilder CreateDefaultBuilder(WPFAppOptions options)
    {
        return new WPFAppBuilder(options);
    }
}
