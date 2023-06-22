using ReactiveFramework.Hosting.Abstraction;
using ReactiveFramework.Modularity.Abstraction;
using ReactiveFramework.Modularity.Internal;

namespace ReactiveFramework.Modularity;
public class ModuleRegistrationContextBuilder : IModuleRegistrationContextBuilder
{
    private readonly Dictionary<Type, object> _contextObjects = new();


    public IModuleRegistrationContext Build(IRxHostBuilder builder)
    {
        ProvidContextObjects(builder);
        return new ModuleRegistrationContext(_contextObjects);
    }

    protected virtual void ProvidContextObjects(IRxHostBuilder builder)
    {
        AddContextObject(builder.Services);
        AddContextObject(builder.Configuration);
        AddContextObject(builder.Environment);
        AddContextObject(builder.Logging);
    }

    protected void AddContextObject<T>(T ctxObject)
        where T : notnull
    {
        _contextObjects.Add(typeof(T), ctxObject);
    }
}
