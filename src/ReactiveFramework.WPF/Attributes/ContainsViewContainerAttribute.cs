namespace ReactiveFramework.WPF.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class ContainsViewContainerAttribute : Attribute
{

    public ContainsViewContainerAttribute(object key)
    {
        Key = key;
    }

    public object Key { get; }
}
