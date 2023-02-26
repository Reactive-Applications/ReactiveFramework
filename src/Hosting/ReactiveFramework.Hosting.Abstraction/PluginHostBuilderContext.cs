using Microsoft.Extensions.Configuration;

namespace ReactiveFramework.Hosting.Abstraction;
public class PluginHostBuilderContext
{
    public IPluginHostEnvironment Environment { get; set; } = default!;

    public IConfiguration Configuration { get; set; } = default!;
}
