namespace RxFramework.WPF.FluentControls.Behaviors;
public sealed class ChangeScope : DisposableObject
{
    private readonly GlowWindowBehavior _behavior;

    public ChangeScope(GlowWindowBehavior behavior)
    {
        _behavior = behavior;
        _behavior.DeferGlowChangesCount++;
    }

    protected override void DisposeManagedResources()
    {
        _behavior.DeferGlowChangesCount--;
        if (_behavior.DeferGlowChangesCount == 0)
        {
            _behavior.EndDeferGlowChanges();
        }
    }
}
