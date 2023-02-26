using Microsoft.Extensions.Hosting;

namespace ReactiveFramework.WPF.Hosting;
public interface IWpfHost : IHost
{
    Task InitializeAsync();
}
