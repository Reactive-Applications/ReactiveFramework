namespace RxFramework.ReactiveProperty;

[Flags]
public enum RxPropertySettings
{
    None = 0x00,
    
    DistinctUntilChanged = 0x02,
    
    RaiseLatestValueOnSubscribe = 0x04,

    IgnoreExcpetions = 0x08,

    Default = DistinctUntilChanged | RaiseLatestValueOnSubscribe
}
