namespace RxFramework.WPF.ExtensionMethodes;
public static class EnumerableExtensions
{
    public static IEnumerable<T> AsSingleEnumerable<T>(this T element)
    {
        yield return element;
    }
}
