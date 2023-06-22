using ReactiveFramework.Modularity.Abstraction;
using System.Collections;

namespace ReactiveFramework.Modularity.Internal;
internal sealed class ModuleCatalog : IModuleCatalog, IReadonlyModuleCatalog
{
    private readonly HashSet<IModule> _modules = new();

    public bool IsReadOnly => false;
    public int Count => _modules.Count;

    public T Add<T>() where T : IModule, new()
    {
        if (IsReadOnly)
        {
            throw new InvalidOperationException("Plugin collection can't be Modified");
        }

        var type = typeof(T);
        var module = new T();
        _modules.Add(module);
        return module;
    }

    public IModule Add(Type moduleType)
    {
        if (IsReadOnly)
        {
            throw new InvalidOperationException("Plugin collection can't be Modified");
        }

        if (!moduleType.IsAssignableTo(typeof(IModule)))
        {
            throw new ArgumentException("pluginType must implement IPlugin");
        }

        var module = Activator.CreateInstance(moduleType) as IModule ?? throw new InvalidOperationException($"can't create module of type {moduleType.FullName}");
        _modules.Add(module);
        return module;
    }

    public void Add(IModule module)
    {
        if (IsReadOnly)
        {
            throw new InvalidOperationException("Plugin collection can't be Modified");
        }

        _modules.Add(module);
    }

    public void Clear()
    {
        _modules.Clear();
    }

    public bool Contains(IModule item)
    {
        return _modules.Contains(item);
    }

    public void CopyTo(IModule[] array, int arrayIndex)
    {
        _modules.CopyTo(array, arrayIndex);
    }

    public IEnumerator<IModule> GetEnumerator()
    {
        return _modules.GetEnumerator();
    }

    public bool Remove(IModule item)
    {
        return _modules.Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _modules.GetEnumerator();
    }
}
