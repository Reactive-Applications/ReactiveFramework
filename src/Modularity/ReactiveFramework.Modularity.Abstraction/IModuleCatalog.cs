namespace ReactiveFramework.Modularity.Abstraction;

public interface IModuleCatalog : ICollection<IModule>
{
    T Add<T>() where T : IModule, new();
    IModule Add(Type moduleType);
}

public interface IReadonlyModuleCatalog : IReadOnlyCollection<IModule> { }