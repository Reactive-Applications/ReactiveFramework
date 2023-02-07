namespace ReactiveFramework.WPF.Attributes;
public class ContainsViewContainerAttribute : Attribute
{

    public ContainsViewContainerAttribute(object key)
    {
        Key = key;
    }

    public object Key { get; }
}
