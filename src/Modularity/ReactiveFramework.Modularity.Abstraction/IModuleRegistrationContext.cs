using ReactiveFramework.Hosting.Abstraction;

namespace ReactiveFramework.Modularity.Abstraction;

public interface IModuleRegistrationContext
{
    T Get<T>();
}

public interface IModuleRegistrationContextBuilder
{
    IModuleRegistrationContext Build(IRxHostBuilder hostBuilder);
}