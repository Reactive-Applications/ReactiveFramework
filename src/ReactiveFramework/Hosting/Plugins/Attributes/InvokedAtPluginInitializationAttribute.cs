namespace ReactiveFramework.Hosting.Plugins.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class InvokedAtPluginInitializationAttribute : Attribute
{
    public InvokedAtPluginInitializationAttribute(int priority = 0)
    {
        Priority = priority;
    }

    public int Priority { get; }
}
