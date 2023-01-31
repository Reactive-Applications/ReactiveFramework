using System.Collections;

namespace RxFramework.Hosting.Plugins.Internal;
internal sealed class PluginInitializerCollection : IPluginInitializerCollection
{
    private readonly Dictionary<Type, IPluginInitializer> _loaders = new Dictionary<Type, IPluginInitializer>();
    private readonly HashSet<Type> _pluginLoaderTypes = new();

    public PluginInitializerCollection()
    {
        Add<DefaultPluginInitializer>();
    }

    public void Add(IPluginInitializer loader)
    {
        _loaders[loader.PluginType] = loader;
    }

    public void AddFromCollection(IPluginInitializerCollection collection)
    {
        foreach (var loader in collection)
        {
            _loaders.TryAdd(loader.PluginType, loader);
        }
    }


    public IEnumerable<IPluginInitializer> GetLoadersFor<T>() where T : IPlugin
    {
        return GetLoadersFor(typeof(T));
    }

    public IEnumerable<IPluginInitializer> GetLoadersFor(Type pluginType)
    {
        if (!pluginType.IsAssignableTo(typeof(IPlugin)))
        {
            throw new NotSupportedException($"Plugin must implement the {typeof(IPlugin)} interface");
        }

        var baseType = pluginType;
        while (baseType != null)
        {
            if (_loaders.ContainsKey(baseType))
            {
                yield return _loaders[baseType];
                baseType = null;
            }
            else
            {
                baseType = baseType.BaseType;
            }

        }

        foreach (var @interface in pluginType.GetInterfaces())
        {
            if (_loaders.ContainsKey(@interface))
            {
                yield return _loaders[@interface];
            }
        }
    }

    public IEnumerable<IPluginInitializer> GetInitializersFor(IPlugin plugin)
    {
        return GetLoadersFor(plugin.GetType());
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _loaders.Values.GetEnumerator();
    }
    public IEnumerator<IPluginInitializer> GetEnumerator()
    {
        return _loaders.Values.GetEnumerator();
    }

    public void Add<T>() where T : IPluginInitializer
    {
        _pluginLoaderTypes.Add(typeof(T));
    }

    public void Add(Type pluginLoaderType)
    {
        if (!pluginLoaderType.IsAssignableTo(typeof(IPluginInitializer)))
        {
            throw new ArgumentException($"Type must implement the {typeof(IPluginInitializer)} interface");
        }

        _pluginLoaderTypes.Add(pluginLoaderType);
    }

    public void CreateInitializers(IServiceProvider serviceProvider)
    {
        foreach (var type in _pluginLoaderTypes)
        {
            var pluginLoader = (IPluginInitializer)serviceProvider.GetUnregisteredService(type);
            Add(pluginLoader);
        }
        _pluginLoaderTypes.Clear();
    }
}
