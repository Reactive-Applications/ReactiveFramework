using System.Diagnostics.CodeAnalysis;

namespace ReactiveFramework.WPF.ViewComposition;

public interface IReadOnlyViewCollection : IReadOnlyCollection<ViewDescriptor>
{
    ViewDescriptor GetDescriptorForViewModel(Type viewModelType);
    bool TryGetDescriptorForViewModel(Type viewModelType, [MaybeNullWhen(false)] out ViewDescriptor descriptor);
    IEnumerable<ViewDescriptor> GetDescriptorsByKey(object key);
}