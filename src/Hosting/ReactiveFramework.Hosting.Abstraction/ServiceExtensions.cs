namespace ReactiveFramework.Hosting;
public static class ServiceExtensions
{
    public static IEnumerable<object> GetServices(this IServiceProvider serviceProvider, IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            var service = serviceProvider.GetService(type);

            yield return service ?? throw new InvalidOperationException($"{type.FullName} is not registered");
        }
    }
}
