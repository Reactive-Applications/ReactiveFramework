using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using ReactiveFramework.WPF.Attributes;

namespace ReactiveFramework.WPF.ViewComposition;

public class ViewDescriptor
{
    public ViewDescriptor(Type viewType, Type viewModelType)
    {
        ArgumentNullException.ThrowIfNull(viewType);
        ArgumentNullException.ThrowIfNull(viewModelType);

        ViewType = viewType;
        ViewModelType = viewModelType;
    }

    internal HashSet<object> InternalLookupKeys { get; } = new();

    public Type ViewType { get; }
    public Type ViewModelType { get; }
    public Dictionary<object, object> Properties { get; } = new Dictionary<object, object>();
    public IReadOnlyCollection<object> LookupKeys => InternalLookupKeys;

    public static ViewDescriptor Create(Type viewType)
    {
        ArgumentNullException.ThrowIfNull(viewType);

        if (viewType.IsAssignableFrom(typeof(FrameworkElement)))
        {
            throw new NotSupportedException($"view type must be an {typeof(FrameworkElement).FullName}");
        }

        var attribute = viewType.GetCustomAttribute(typeof(ViewForAttribute<>))
            ?? throw new NotSupportedException($"view must be annotated with an {typeof(ViewForAttribute<>).FullName}");

        var vmType = attribute?.GetType().GetGenericArguments()[0];
        var descriptor = new ViewDescriptor(viewType, vmType!);

        return descriptor;
    }
}