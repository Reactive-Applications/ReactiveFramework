namespace ReactiveFramework.Hosting.Abstraction.Plugins;

public interface IPluginCollection : IEnumerable<PluginDescription>
{
    bool IsReadOnly { get; }

    void Add<T>() where T : IPlugin, new();
    void Add(Type pluginType);
    void Add(PluginDescription pluginDescription);
    void AddFromDirectory(string directory);

    void MakeReadOnly();
}