namespace RxFramework.Hosting.Plugins;

public interface IPluginInitializerCollection : IEnumerable<IPluginInitializer>
{
    void Add(IPluginInitializer loader);
    void Add<T>() where T : IPluginInitializer;
    void Add(Type pluginLoaderType);
    void AddFromCollection(IPluginInitializerCollection collection);
    void CreateInitializers(IServiceProvider serviceProvider);
    IEnumerable<IPluginInitializer> GetLoadersFor<T>()
        where T : IPlugin;

    IEnumerable<IPluginInitializer> GetLoadersFor(Type pluginType);
    IEnumerable<IPluginInitializer> GetInitializersFor(IPlugin plugin);
}