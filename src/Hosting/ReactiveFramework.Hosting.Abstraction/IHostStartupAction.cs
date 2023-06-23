namespace ReactiveFramework.Hosting.Abstraction;
public interface IHostStartupAction
{

    int Priority { get; }
    HostStartupActionExecution ExecutionTime { get; }
    Task Execute(CancellationToken cancellation);
}

public static class HostStartupPriorities
{
    public const int Default = 0;
}
