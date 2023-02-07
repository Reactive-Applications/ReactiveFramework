using ReactiveFramework.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveFramework.WPF.Hosting;
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
