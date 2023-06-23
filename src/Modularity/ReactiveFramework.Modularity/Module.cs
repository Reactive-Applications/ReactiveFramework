using Microsoft.Extensions.DependencyInjection;
using ReactiveFramework.Modularity.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveFramework.Modularity;
public abstract class Module : IModule
{
    public Version Version { get; }
    public string Name { get; }

    public virtual Task RegisterModuleAsync(IModuleRegistrationContext context, CancellationToken cancellation)
    {
        RegisterServices(context.GetServiceCollection());
        return Task.CompletedTask;
    }

    public abstract Task StartModuleAsync(IServiceProvider services, CancellationToken cancellation);
    protected abstract void RegisterServices(IServiceCollection services);
}
