using ReactiveFramework.Hosting.Abstraction.Plugins;
using ReactiveFramework.Hosting.Plugins;
using System.Collections;
using System.Reactive.Linq;
using System.Reflection;

namespace ReactiveFramework.Hosting.Internal;
internal sealed class PluginCollection : IPluginCollection
{
    private readonly HashSet<PluginDescription> _plugins = new();

    public bool IsReadOnly { get; }

    public void Add<T>() where T : IPlugin, new()
    {
        var type = typeof(T);
        _plugins.Add(new(type));
    }

    public void Add(Type pluginType)
    {
        if (!pluginType.IsAssignableTo(typeof(IPlugin)))
        {
            throw new ArgumentException("pluginType must implement IPlugin");
        }

        _plugins.Add(new(pluginType));
    }

    public void Add(PluginDescription pluginDescription)
    {
        _plugins.Add(pluginDescription);
    }

    public void AddFromDirectory(string directory)
    {
        var innerDirectories = Directory.GetDirectories(directory);

        if (innerDirectories.Length == 0)
        {
            throw new InvalidOperationException("Plugins must be in a seperate folder per plugin");
        }

        foreach (var innerDirectory in innerDirectories)
        {
            var dirName = Path.GetFileName(Path.GetDirectoryName(innerDirectory));
            var file = Directory.GetFiles(innerDirectory, $"*{dirName}*.dll").First()
                ?? throw new FileNotFoundException($"no dll for '{dirName}' found");
            
            var ass = Assembly.LoadFrom(file);
            var plugin = ass.GetTypes().Where(t => t.IsAssignableTo(typeof(IPlugin))).First()
                ?? throw new Exception($"{file}' did not contain any Plugin");
            Add(plugin);
        }
    }

    public IEnumerator<PluginDescription> GetEnumerator()
    {
        return _plugins.GetEnumerator();
    }

    public void MakeReadOnly()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _plugins.GetEnumerator();
    }
}
