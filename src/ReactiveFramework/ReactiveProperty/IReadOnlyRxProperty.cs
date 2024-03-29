﻿using System.ComponentModel;

namespace ReactiveFramework.ReactiveProperty;

public interface IReadOnlyRxProperty : INotifyPropertyChanged
{
    object? Value { get; }
}

public interface IReadOnlyRxProperty<T> : IReadOnlyRxProperty, IObservable<T>, IDisposable
{
    new T Value { get; }

    bool IsDisposed { get; }

    RxPropertySettings Settings { get; }
}
