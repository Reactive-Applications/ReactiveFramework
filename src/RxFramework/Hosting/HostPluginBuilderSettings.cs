using Microsoft.Extensions.Configuration;
using RxFramework.Hosting.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxFramework.Hosting;
public class PluginAppBuilderOptions
{
    public bool DisableDefaults { get; set; }

    /// <summary>
    /// The command line arguments. This is unused if <see cref="DisableDefaults"/> is <see langword="true"/>.
    /// </summary>
    public string[]? Args { get; set; }

    public ConfigurationManager? Configuration { get; set; }

    /// <summary>
    /// The environment name.
    /// </summary>
    public string? EnvironmentName { get; set; }

    /// <summary>
    /// The application name.
    /// </summary>
    public string? ApplicationName { get; set; }

    /// <summary>
    /// The content root path.
    /// </summary>
    public string? ContentRootPath { get; set; }

    public IPluginCollection? PluginCollection { get; set; }

    public IPluginManager? PluginManager { get; set; }

    public IPluginInitializerCollection? PluginInitializers { get; set; }
}
