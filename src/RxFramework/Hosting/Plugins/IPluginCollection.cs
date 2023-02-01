using System.Reactive;

namespace RxFramework.Hosting.Plugins;
public interface IPluginCollection : IEnumerable<PluginDescription>
{
    void Add<T>() where T : IPlugin, new();
    void Add(Type pluginType);
    void Add(PluginDescription pluginDescription);
    void AddFromDirectory(string directory);
}