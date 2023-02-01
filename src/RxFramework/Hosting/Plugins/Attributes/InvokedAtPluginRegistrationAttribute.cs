namespace RxFramework.Hosting.Plugins.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class InvokedAtPluginRegistrationAttribute : Attribute
{

    public InvokedAtPluginRegistrationAttribute(int priority = 0)
    {
        Priority = priority;
    }

    public int Priority { get; }
}
