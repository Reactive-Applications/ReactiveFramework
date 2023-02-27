using ReactiveFramework.Hosting.Abstraction.Attributes;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ReactiveFramework.Hosting.Abstraction.Plugins;

public class PluginDescription
{
    public IEnumerable<MethodInfo> InitializeMethods { get; private set; }
    public IEnumerable<MethodInfo> StartMethods { get; private set; }

    public string? PluginName { get; private set; }

    public Version? PluginVersion { get; private set; }

    public Type PluginType { get; }

    public IPlugin? Instance { get; private set; }

    [MemberNotNullWhen(true, nameof(Instance))]
    public bool IsRegistered { get; private set; }

    [MemberNotNullWhen(true, nameof(Instance))]
    public bool IsInitialized { get; private set; }

    public PluginDescription(Type type)
    {
        InitializeMethods = Enumerable.Empty<MethodInfo>();
        StartMethods = Enumerable.Empty<MethodInfo>();

        PluginType = type;
    }

    public override string? ToString()
    {
        return IsRegistered ? $"{PluginName} version: {PluginVersion}" : base.ToString();
    }

    [MemberNotNull(nameof(PluginName), nameof(Instance))]
    public void InitializeDescriptor()
    {
        if (IsInitialized)
        {
            return;
        }

        Instance = Activator.CreateInstance(PluginType) as IPlugin;

        PluginVersion = Instance.GetVersion();
        PluginName = Instance.GetName();

        InitializeMethods = PluginType.GetMethods()
            .Where(m => Attribute.IsDefined(m, typeof(InvokedAtAppInitializationAttribute)))
            .Select(m => (Attribute: m.GetCustomAttribute<InvokedAtAppInitializationAttribute>(true)!, MethodInfo: m))
            .Union(PluginType.GetInterfaces()
                .SelectMany(i => i.GetMethods())
                .Where(m => Attribute.IsDefined(m, typeof(InvokedAtAppInitializationAttribute)))
                .Select(m => (Attribute: m.GetCustomAttribute<InvokedAtAppInitializationAttribute>(true)!, MethodInfo: m)))
            .OrderBy(v => v.Attribute.Priority)
            .Select(v => v.MethodInfo);

        StartMethods = PluginType.GetMethods()
            .Where(m => Attribute.IsDefined(m, typeof(InvokedAtAppStartAttribute)))
            .Select(m => (Attribute: m.GetCustomAttribute<InvokedAtAppStartAttribute>(true)!, MethodInfo: m))
            .Union(PluginType.GetInterfaces()
                .SelectMany(i => i.GetMethods())
                .Where(m => Attribute.IsDefined(m, typeof(InvokedAtAppStartAttribute)))
                .Select(m => (Attribute: m.GetCustomAttribute<InvokedAtAppStartAttribute>(true)!, MethodInfo: m)))
            .OrderBy(v => v.Attribute.Priority)
            .Select(v => v.MethodInfo);
    }
}