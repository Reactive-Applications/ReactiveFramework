namespace ReactiveFramework.Hosting.Abstraction.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class InvokedAtAppInitializationAttribute : Attribute
{

    public InvokedAtAppInitializationAttribute(int priority = 0, bool canRunAsync = true)
    {
        Priority = priority;
        CanRunAsync = canRunAsync;
    }

    public int Priority { get; }

    public bool CanRunAsync { get; }
}
