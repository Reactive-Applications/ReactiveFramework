namespace ReactiveFramework.Hosting.Abstraction;
public interface IStartupService
{
    Task OnAppInitiallization(IServiceProvider initializationServices, CancellationToken cancellationToken = default);

    Task OnAppStart(IServiceProvider runtimeServices, CancellationToken cancellationToken = default);
}
