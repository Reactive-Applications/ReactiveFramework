using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using ReactiveFramework.Hosting.Abstraction;

namespace ReactiveFramework.Hosting.Internal;
internal class PluginHostEnvironment : IPluginHostEnvironment
{
    public PluginHostEnvironment(IHostEnvironment hostEnvironment)
    {
        EnvironmentName = hostEnvironment.EnvironmentName;
        ApplicationName = hostEnvironment.ApplicationName;
        ContentRootPath = hostEnvironment.ContentRootPath;
        ContentRootFileProvider = hostEnvironment.ContentRootFileProvider;
    }

    public string EnvironmentName { get; set; }
    public string ApplicationName { get; set; }
    public string ContentRootPath { get; set; }
    public IFileProvider ContentRootFileProvider { get; set; }
}
