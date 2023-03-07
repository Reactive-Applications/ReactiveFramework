namespace ReactiveFramework.RxProperty;

public class RxPropertyOptions
{
    public static RxPropertyOptions Default { get; } = new()
    {
        DistinctUntilChanged = true,
        RaiseLatestValueOnSubscribe = true,
    };

    public bool DistinctUntilChanged { get; set; }

    public bool RaiseLatestValueOnSubscribe { get; set; }
}