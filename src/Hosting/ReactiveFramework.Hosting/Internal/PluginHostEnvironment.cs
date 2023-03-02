using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using ReactiveFramework.Hosting.Abstraction;

namespace ReactiveFramework.Hosting.Internal;
public class PluginHostEnvironment : IPluginHostEnvironment
{
    public PluginHostEnvironment()
    {
        
    }

    public string EnvironmentName { get; set; }
    public string ApplicationName { get; set; }
    public string ContentRootPath { get; set; }
    public IFileProvider ContentRootFileProvider { get; set; }
}
