using RxFramework.Hosting.Plugins.Attributes;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;

namespace RxFramework.Hosting.Plugins;

public class PluginDescription
{
    private IEnumerable<MethodInfo>? _registrationMethods;
    private IEnumerable<MethodInfo>? _initializationMethods;

    public string? PluginName { get; private set; }
    
    public Version? PluginVersion { get; private set; }
    
    public Type PluginType { get; }

    public Plugin? Instance { get; private set; }

    [MemberNotNullWhen(true,nameof(Instance), nameof(Instance), nameof(_registrationMethods), nameof(_initializationMethods))]
    public bool IsRegistered { get; private set; }

    public bool IsInitialized { get; private set; }

    public PluginDescription(Type type)
    {
        PluginType = type;
    }

    public override string? ToString()
    {
        return IsRegistered ? $"{PluginName} version: {PluginVersion}" : base.ToString();
    }

    public void InitializePlugin(IServiceProvider appServices)
    {
        if (!IsRegistered)
        {
            throw new InvalidOperationException($"Plugin {this} must be registered first");
        }

        if (IsInitialized)
        {
            throw new InvalidOperationException($"plugin {this} is already initialized");
        }

        foreach (var registrationMethod in _initializationMethods)
        {
            var parameterTypes = registrationMethod
                .GetParameters()
                .Select(p => p.ParameterType);

            var parameters = appServices
                .GetServices(parameterTypes)
                .ToArray();

            registrationMethod.Invoke(Instance, parameters);
        }

        IsInitialized = true;
    }

    public void RegisterPlugin(IServiceProvider registrationServices)
    {
        if (IsRegistered)
        {
            throw new InvalidOperationException($"Plugin {this} is already registered");
        }

        InitializeDescriptor();

        foreach (var registrationMethod in _registrationMethods)
        {
            var parameterTypes = registrationMethod
                .GetParameters()
                .Select(p => p.ParameterType);

            var parameters = registrationServices
                .GetServices(parameterTypes)
                .ToArray();

            registrationMethod.Invoke(Instance,parameters);
        }

        IsRegistered = true;
    }

    [MemberNotNull(nameof(PluginName), nameof(Instance), nameof(_registrationMethods), nameof(_initializationMethods), nameof(Instance))]
    private void InitializeDescriptor()
    {
        Instance = Activator.CreateInstance(PluginType) as Plugin ??
            throw new NotSupportedException($"Can't create an instance of the {PluginType} plugin. Does the plugin provide an parameterless constructor?"); ;

        PluginVersion = Instance.GetVersion();
        PluginName = Instance.GetName();

        _registrationMethods = PluginType.GetMethods()
            .Where(m => Attribute.IsDefined(m, typeof(InvokedAtPluginRegistrationAttribute)))
            .Select(m => (Attribute: m.GetCustomAttribute<InvokedAtPluginRegistrationAttribute>(true)!, MethodInfo: m))
            .Union(PluginType.GetInterfaces()
                .SelectMany(i => i.GetMethods())
                .Where(m => Attribute.IsDefined(m, typeof(InvokedAtPluginRegistrationAttribute)))
                .Select(m => (Attribute: m.GetCustomAttribute<InvokedAtPluginRegistrationAttribute>(true)!, MethodInfo: m)))
            .OrderBy(v => v.Attribute.Priority)
            .Select(v => v.MethodInfo);

        _initializationMethods = PluginType.GetMethods()
            .Where(m => Attribute.IsDefined(m, typeof(InvokedAtPluginInitializationAttribute)))
            .Select(m => (Attribute: m.GetCustomAttribute<InvokedAtPluginInitializationAttribute>(true)!, MethodInfo: m))
            .Union(PluginType.GetInterfaces()
                .SelectMany(i => i.GetMethods())
                .Where(m => Attribute.IsDefined(m, typeof(InvokedAtPluginInitializationAttribute)))
                .Select(m => (Attribute: m.GetCustomAttribute<InvokedAtPluginInitializationAttribute>(true)!, MethodInfo: m)))
            .OrderBy(v => v.Attribute.Priority)
            .Select(v => v.MethodInfo);
    }
}