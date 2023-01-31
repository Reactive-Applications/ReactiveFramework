namespace RxFramework.Hosting.Plugins.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class PluginVersionAttribute : Attribute
{
    public Version Version { get; }

    public PluginVersionAttribute(Version version)
    {
        Version = version;
    }

    public PluginVersionAttribute(string version)
    {
        Version = new Version(version);
    }
}
