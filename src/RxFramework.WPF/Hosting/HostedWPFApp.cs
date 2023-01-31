using RxFramework.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxFramework.WPF.Hosting;
public static class HostedWPFApp
{
    public static IHostedWPFAppBuilder CreateDefaultBuilder()
    {
        return new HostedWPFAppBuilder();
    }

    public static IHostedWPFAppBuilder CreateDefaultBuilder(string[] args)
    {
        return new HostedWPFAppBuilder(args);
    }

    public static IHostedWPFAppBuilder CreateDefaultBuilder(WPFAppBuilderOptions options)
    {
        return new HostedWPFAppBuilder(options);
    }
}
