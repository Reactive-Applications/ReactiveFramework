using System.Reactive.Concurrency;

namespace ReactiveFramework;
public static class Schedulers
{

#pragma warning disable IDE0032 // Automatisch generierte Eigenschaft verwenden
    private static IScheduler _uiScheduler = DefaultScheduler.Instance;
#pragma warning restore IDE0032 // Automatisch generierte Eigenschaft verwenden

    public static IScheduler UiScheduler => _uiScheduler;

    public static IScheduler TaskPoolScheduler => System.Reactive.Concurrency.TaskPoolScheduler.Default;

}
