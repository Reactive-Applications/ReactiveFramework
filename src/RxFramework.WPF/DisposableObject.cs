using System;

namespace RxFramework.WPF;
public class DisposableObject : IDisposable
{
    private EventHandler? _disposingEventHandlers;

    public bool IsDisposing { get; private set; }

    public bool IsDisposed { get; private set; }

    public event EventHandler Disposing
    {
        add
        {
            ThrowIfDisposed();
            _disposingEventHandlers = (EventHandler)Delegate.Combine(_disposingEventHandlers, value);
        }
        remove => _disposingEventHandlers = (EventHandler?)Delegate.Remove(_disposingEventHandlers, value);
    }

    ~DisposableObject()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void ThrowIfDisposed()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed
            || IsDisposing)
        {
            return;
        }

        IsDisposing = true;

        try
        {
            if (disposing)
            {
                _disposingEventHandlers?.Invoke(this, EventArgs.Empty);
                _disposingEventHandlers = null;
                DisposeManagedResources();
            }

            DisposeNativeResources();
        }
        finally
        {
            IsDisposed = true;
            IsDisposing = false;
        }
    }

    protected virtual void DisposeManagedResources()
    {
    }

    protected virtual void DisposeNativeResources()
    {
    }
}

