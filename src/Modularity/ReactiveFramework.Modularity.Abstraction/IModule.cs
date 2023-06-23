namespace ReactiveFramework.Modularity.Abstraction;

public interface IModule
{
    Version Version { get; }
    string Name { get; }

    Task RegisterModuleAsync(IModuleRegistrationContext context, CancellationToken cancellation);
    Task StartModuleAsync(IServiceProvider services, CancellationToken cancellation);
}