using RxFramework.Hosting.Plugins.Attributes;
using System.Collections;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;

namespace RxFramework.Hosting.Plugins.Internal;
internal sealed class PluginCollection : IPluginCollection, IDisposable
{
    private readonly List<PluginDescription> _plugins = new();
    private readonly Dictionary<Type, IObservable<Unit>> _triggerDictionary = new();
    private readonly BehaviorSubject<Unit> _defaultTriggerObservable = new(Unit.Default);

    public void Add<T>() where T : IPlugin
    {
        var type = typeof(T);
        _plugins.Add(GetPluginDescription(type));
    }

    public void Add(Type pluginType)
    {
        if (!pluginType.GetInterfaces().Contains(typeof(IPlugin)))
        {
            throw new ArgumentException("pluginType must implement IPlugin");
        }

        _plugins.Add(GetPluginDescription(pluginType));
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
            var dirname = Path.GetFileName(Path.GetDirectoryName(innerDirectory));
            var file = Directory.GetFiles(innerDirectory, $"*{dirname}*.dll").First();

            if (file == null)
            {
                throw new FileNotFoundException($"no dll for '{dirname}' found");
            }

            var ass = Assembly.LoadFrom(file);
            var plugin = ass.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IPlugin))).First();

            if (plugin == null)
            {
                throw new Exception($"{file}' did not contain any Plugin");
            }

            Add(plugin);
        }
    }

    public IEnumerator<PluginDescription> GetEnumerator()
    {
        return _plugins.GetEnumerator();
    }


    public void RegisterInitializationTrigger(Type pluginType, IObservable<Unit> trigger)
    {
        if (!pluginType.IsAssignableTo(typeof(IPlugin)))
        {
            throw new NotSupportedException($"pluginType must implement the {typeof(IPlugin).FullName} interface");
        }
        _triggerDictionary.Add(pluginType, trigger);
    }

    public IObservable<Unit> GetTriggerFor(Type pluginType)
    {
        if (!pluginType.IsAssignableTo(typeof(IPlugin)))
        {
            throw new NotSupportedException($"pluginType must implement the {typeof(IPlugin).FullName} interface");
        }

        var basetype = pluginType;
        while (basetype != null)
        {
            if (_triggerDictionary.TryGetValue(basetype, out var adapter))
            {
                return adapter;
            }
            basetype = basetype.BaseType;
        }
        return _defaultTriggerObservable;
    }

    public void Dispose()
    {
        _defaultTriggerObservable.Dispose();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _plugins.GetEnumerator();
    }

    private PluginDescription GetPluginDescription(Type type)
    {
        var name = type.Name;

        var nameAttribute = type.GetCustomAttribute<PluginNameAttribute>();
        if (nameAttribute != null)
        {
            name = nameAttribute.Name;
        }

        var version = type.Assembly.GetName().Version;
        var versionAtribute = type.GetCustomAttribute<PluginVersionAttribute>();
        if (versionAtribute != null)
        {
            version = versionAtribute.Version;
        }

        return new PluginDescription(name, type, version!);
    }
}
