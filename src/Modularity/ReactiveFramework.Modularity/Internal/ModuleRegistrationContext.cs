using ReactiveFramework.Modularity.Abstraction;

namespace ReactiveFramework.Modularity.Internal;
internal class ModuleRegistrationContext : IModuleRegistrationContext
{
    private readonly Dictionary<Type, object> _contextObjects;

    public ModuleRegistrationContext(Dictionary<Type, object> contextObjects)
    {
        _contextObjects = contextObjects;
    }

    public T Get<T>()
        => (T)_contextObjects[typeof(T)];
}
