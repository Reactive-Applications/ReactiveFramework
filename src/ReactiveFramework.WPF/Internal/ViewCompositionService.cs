﻿using ReactiveFramework.WPF.Behaviors;
using ReactiveFramework.WPF.ViewComposition;

namespace ReactiveFramework.WPF.Internal;
internal class ViewCompositionService : IViewCompositionService
{
    private readonly IViewCollection _viewCollection;
    private readonly IViewAdapterCollection _viewAdapters;
    private readonly IViewProvider _viewProvider;

    public ViewCompositionService(IViewCollection viewCollection, IViewAdapterCollection viewAdapters, IViewProvider viewProvider)
    {
        _viewCollection = viewCollection;
        _viewAdapters = viewAdapters;
        _viewProvider = viewProvider;
    }

    public void InsertView<TViewModel>(object containerKey) where TViewModel : IViewModel
    {
        if (!_viewCollection.GetDescriptorsByKey(ViewDescriptorKeys.ContainsViewContainerKey)
            .Any(v => v.Properties.TryGetValue(ViewDescriptorKeys.ContainsViewContainerKey, out var navKeys) && navKeys is HashSet<object> hasSet && hasSet.Contains(containerKey)))
        {
            throw new InvalidOperationException($"No navigation frame with the key: {containerKey} found.");
        }

        ViewContainer.ExecuteContainerAction(containerKey, container =>
        {
            var adapter = _viewAdapters.GetAdapterFor(container.GetType());
            var view = _viewProvider.GetViewWithViewModel(typeof(TViewModel));
            adapter.Insert(container, view);
        });
    }

    public void RemoveView<TViewModel>(object containerKey) where TViewModel : IViewModel
    {
        if (!_viewCollection.GetDescriptorsByKey(ViewDescriptorKeys.ContainsViewContainerKey).Any(v => v.Properties[ViewDescriptorKeys.ContainsViewContainerKey].Equals(containerKey)))
        {
            throw new InvalidOperationException($"No navigation frame with the key: {containerKey} found.");
        }

        if (!ViewContainer.ViewContainers.TryGetValue(containerKey, out var container))
        {
            if (!ViewContainer.InitialInsertions.TryGetValue(containerKey, out var initialInsertions))
            {
                initialInsertions = new();
                ViewContainer.InitialInsertions[containerKey] = initialInsertions;
            }

            initialInsertions.Add(c =>
            {
                var adapter = _viewAdapters.GetAdapterFor(c.GetType());
                var view = adapter.GetElements(c).FirstOrDefault(e => e.DataContext.GetType() == typeof(TViewModel));
                if (view != null)
                {
                    adapter.Remove(c, view);
                }
            });

            return;
        }

        var adapter = _viewAdapters.GetAdapterFor(container.GetType());
        var view = adapter.GetElements(container).FirstOrDefault(e => e.DataContext.GetType() == typeof(TViewModel));
        if (view != null)
        {
            adapter.Remove(container, view);
        }
    }
}
