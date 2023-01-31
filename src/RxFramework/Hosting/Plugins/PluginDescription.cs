namespace RxFramework.Hosting.Plugins;

public class PluginDescription
{
    public string PluginName { get; }
    public Version PluginVersion { get; }
    public Type PluginType { get; }

    public PluginDescription(string name, Type type, Version version)
    {
        PluginName = name;
        PluginVersion = version;
        PluginType = type;
    }

    public override string ToString()
    {
        return $"{PluginName}:{PluginVersion}";
    }
}