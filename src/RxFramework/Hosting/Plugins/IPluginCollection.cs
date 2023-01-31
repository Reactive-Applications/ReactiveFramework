using System.Reactive;

namespace RxFramework.Hosting.Plugins;
public interface IPluginCollection : IEnumerable<PluginDescription>
{
    void Add<T>() where T : IPlugin;
    void Add(Type pluginType);
    void Add(PluginDescription pluginDescription);
    void AddFromDirectory(string directory);

    void RegisterInitializationTrigger(Type pluginType, IObservable<Unit> trigger);

    IObservable<Unit> GetTriggerFor(Type pluginType);
}