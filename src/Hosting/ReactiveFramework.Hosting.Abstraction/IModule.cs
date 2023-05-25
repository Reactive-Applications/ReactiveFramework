namespace ReactiveFramework.Modularity.Abstraction;

public interface IModule
{
    Task RegisterModuleAsync(IModuleRegistrationContext context);

    Task StartModuleAsync(ModulStartContext context);

    Version GetVersion();

    string GetName();
}