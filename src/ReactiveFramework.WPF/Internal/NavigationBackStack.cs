using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security.Cryptography.Xml;
using System.Windows;

namespace ReactiveFramework.WPF.Internal;
public class NavigationBackStack
{

    private NavigationEntry? _current;
    private NavigationEntry? _root;
    private NavigationEntry? _tail;

    private readonly BehaviorSubject<bool> _canNavigateBackSubject = new(false);
    private readonly BehaviorSubject<bool> _canNavigateForwardSubject = new(false);

    private uint _count;

    public uint Capacity { get; set; }

    public IObservable<bool> CanNavigateBack()
    {
        return _canNavigateBackSubject;
    }

    public IObservable<bool> CanNavigateForward()
    {
        return _canNavigateForwardSubject;
    }

    public void Insert(FrameworkElement element)
    {
        if (Capacity <= 1)
        {
            return;
        }

        var entry = new NavigationEntry(element);
        _current = entry;

        switch (_count)
        {
            case 0:
                _root = entry;
                _tail = entry;
                break;
            case 1:
                _tail = entry;
                _root.After = _tail;
                _tail.Before = _root;
                break;
            default:
                _tail.After = entry;
                entry.Before = _tail;
                _tail = entry;
                break;
        }

        _count++;

        if (_count > Capacity)
        {

            _root = _root.After;
            _root.Before = null;
            _count--;
        }

        _canNavigateBackSubject.OnNext(_current.Before != null);
    }

    public NavigationBackStack(uint capacity = 10)
    {
        Capacity = capacity;
    }

    public NavigationEntry NavigateBack()
    {
        if (!CanNavigateBack().FirstAsync().Wait())
        {
            throw new InvalidOperationException("cant navigate back");
        }

        _current = _current.Before;

        _canNavigateBackSubject.OnNext(_current.Before != null);
        _canNavigateForwardSubject.OnNext(_current.After != null);

        return _current;
    }

    public NavigationEntry NavigateForward()
    {
        if (!CanNavigateForward().FirstAsync().Wait())
        {
            throw new InvalidOperationException("cant navigate back");
        }

        _current = _current.After;

        _canNavigateForwardSubject.OnNext(_current.After != null);
        _canNavigateBackSubject.OnNext(_current.Before != null);

        return _current;
    }
}
