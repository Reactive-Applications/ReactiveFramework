using Microsoft.Extensions.Hosting;
using ReactiveFramework.Hosting.Abstraction;
using ReactiveFramework.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveFramework.Hosting;
public class RxHost
{
    public static IRxHostBuilder CreateDefaultBuilder(string[] args)
        => new RxHostBuilder(new RxHostBuilderSettings(
            args,
            null,
            null,
            null,
            null,
            false));

    public static IRxHostBuilder CreateDefaultBuilder(RxHostBuilderSettings settings)
        => new RxHostBuilder(settings);
}
