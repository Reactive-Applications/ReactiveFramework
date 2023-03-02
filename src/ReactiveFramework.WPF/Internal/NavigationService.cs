using ReactiveFramework.WPF.Behaviors;
using ReactiveFramework.WPF.ViewComposition;
using System.Reactive.Linq;
using System.Windows;

namespace ReactiveFramework.WPF.Internal;
internal class NavigationService : INavigationService
{
    private readonly IViewProvider _viewProvider;
    private readonly IViewCollection _viewCollection;
    private readonly IViewAdapterCollection _viewAdapters;
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<object, NavigationBackStack> _backStacks = new();
    private readonly Dictionary<object, List<FrameworkElement>> _viewCache = new();
    private readonly Dictionary<object, IViewAdapter> _viewAdapterForContainers = new();

    private uint _backStackCapacity = 1;

    public NavigationService(IViewProvider viewProvider, IViewCollection viewCollection, IViewAdapterCollection viewAdapters, IServiceProvider serviceProvider)
    {
        _viewProvider = viewProvider;
        _viewCollection = viewCollection;
        _viewAdapters = viewAdapters;
        _serviceProvider = serviceProvider;
    }

    public uint BackStackCapacity
    {
        get => _backStackCapacity;
        set
        {
            _backStackCapacity = value;
            foreach (var backStack in _backStacks.Values)
            {
                backStack.Capacity = _backStackCapacity;
            }
        }
    }

    public IObservable<bool> CanNavigateBack(object containerKey)
    {
        if (!_backStacks.TryGetValue(containerKey, out var backStack))
        {
            backStack = new(_backStackCapacity);
            _backStacks[containerKey] = backStack;
        }

        return backStack.CanNavigateBack();
    }

    public IObservable<bool> CanNavigateForward(object containerKey)
    {
        if (!_backStacks.TryGetValue(containerKey, out var backStack))
        {
            backStack = new(_backStackCapacity);
            _backStacks[containerKey] = backStack;
        }

        return backStack.CanNavigateForward();
    }

    public void NavigateBack(object containerKey)
    {
        if (!CanNavigateBack(containerKey).FirstAsync().Wait())
        {
            throw new InvalidOperationException("can't navigate back");
        }

        ViewContainer.ExecuteContainerAction(containerKey, container =>
        {
            var entry = _backStacks[containerKey].NavigateBack();
            var adapter = _viewAdapterForContainers[containerKey];

            var oldView = adapter.GetActiveView(container)!;

            if (oldView.DataContext is INavigableViewModel oldVm)
            {
                oldVm.OnNavigatedFrom();
            }

            adapter.Select(container, entry.View);

            if (entry.View.DataContext is INavigableViewModel newVm)
            {
                newVm.OnNavigatedTo();
            }
        });
    }

    public void NavigateForward(object containerKey)
    {
        if (!CanNavigateForward(containerKey).FirstAsync().Wait())
        {
            throw new InvalidOperationException("can't navigate forward");
        }

        ViewContainer.ExecuteContainerAction(containerKey, container =>
        {
            var entry = _backStacks[containerKey].NavigateForward();
            var adapter = _viewAdapterForContainers[containerKey];

            var oldView = adapter.GetActiveView(container)!;

            if (oldView.DataContext is INavigableViewModel oldVm)
            {
                oldVm.OnNavigatedFrom();
            }

            adapter.Select(container, entry.View);

            if (entry.View.DataContext is INavigableViewModel newVm)
            {
                newVm.OnNavigatedTo();
            }
        });
    }

    public void NavigateTo<TViewModel>(object containerKey) where TViewModel : INavigableViewModel
    {
        if (!_viewCollection.GetDescriptorsByKey(ViewDescriptorKeys.ContainsViewContainerKey)
            .Any(v => v.Properties.TryGetValue(ViewDescriptorKeys.ContainsViewContainerKey, out var navKeys) && navKeys is HashSet<object> hasSet && hasSet.Contains(containerKey)))
        {
            throw new InvalidOperationException($"No navigation frame with the key: {containerKey} found.");
        }

        ViewContainer.ExecuteContainerAction(containerKey, container =>
        {
            var newView = ExecuteNavigation(containerKey, container, typeof(TViewModel));

            if (newView.DataContext is INavigableViewModel paramVm)
            {
                paramVm.OnNavigatedTo();
            }
        });
    }

    public void NavigateTo<TViewModel, TParameter>(object containerKey, TParameter parameter) where TViewModel : INavigableViewModel<TParameter>
    {

        if (!_viewCollection.GetDescriptorsByKey(ViewDescriptorKeys.ContainsViewContainerKey)
            .Any(v => v.Properties.TryGetValue(ViewDescriptorKeys.ContainsViewContainerKey, out var navKeys) && navKeys is HashSet<object> hasSet && hasSet.Contains(containerKey)))
        {
            throw new InvalidOperationException($"No navigation frame with the key: {containerKey} found.");
        }

        ViewContainer.ExecuteContainerAction(containerKey, container =>
        {
            var newView = ExecuteNavigation(containerKey, container, typeof(TViewModel));

            if (newView.DataContext is INavigableViewModel<TParameter> paramVm)
            {
                paramVm.OnNavigatedTo(parameter);
            }
        });
    }

    public void NavigateTo<TViewModel>(object containerKey, TViewModel viewModel) where TViewModel : INavigableViewModel
    {
        if (!_viewCollection.GetDescriptorsByKey(ViewDescriptorKeys.ContainsViewContainerKey)
            .Any(v => v.Properties.TryGetValue(ViewDescriptorKeys.ContainsViewContainerKey, out var navKeys) && navKeys is HashSet<object> hasSet && hasSet.Contains(containerKey)))
        {
            throw new InvalidOperationException($"No navigation frame with the key: {containerKey} found.");
        }

        ViewContainer.ExecuteContainerAction(containerKey, container =>
        {
            var newView = ExecuteNavigation(containerKey, container, viewModel.GetType(), viewModel);

            viewModel.OnNavigatedTo();
        });
    }

    private FrameworkElement ExecuteNavigation(object containerKey, FrameworkElement container, Type viewModelType, IViewModel? viewModel = null)
    {
        if (!_viewAdapterForContainers.TryGetValue(containerKey, out var adapter))
        {
            adapter = _viewAdapters.GetAdapterFor(container.GetType());
            _viewAdapterForContainers[containerKey] = adapter;
        }

        if (!_viewCache.TryGetValue(containerKey, out var cachedViews))
        {
            cachedViews = new();
            _viewCache[containerKey] = cachedViews;
        }

        if (!_backStacks.TryGetValue(containerKey, out var backStack))
        {
            backStack = new(_backStackCapacity);
            _backStacks[containerKey] = backStack;
        }

        var oldView = adapter.GetActiveView(container);

        if (oldView != null && oldView.DataContext is INavigableViewModel vm)
        {
            vm.OnNavigatedFrom();
        }

        var newView = adapter.GetElements(container).FirstOrDefault(e => e.DataContext.GetType() == viewModelType);

        newView ??= cachedViews.FirstOrDefault(e => e.DataContext.GetType() == viewModelType);

        if (newView == null)
        {
            newView ??= _viewProvider.GetView(viewModelType);
            cachedViews.Add(newView);
        }

        if (newView.DataContext == null || viewModel != null)
        {
            viewModel ??= (IViewModel)_serviceProvider.GetUnregisteredService(viewModelType);
            newView.DataContext = viewModel;
        }

        backStack.Insert(newView);
        adapter.Select(container, newView);

        return newView;
    }
}
