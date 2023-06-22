using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace ReactiveFramework.Hosting.Internal;
internal partial class RxHostBuilder
{
    private sealed class ServiceProviderFactoryAdapter<TContainerBuilder> : IServiceProviderFactory<object> where TContainerBuilder : notnull
    {
        private IServiceProviderFactory<TContainerBuilder>? _serviceProviderFactory;

        private readonly Func<HostBuilderContext>? _contextResolver;
        private Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>>? _factoryResolver;

        public ServiceProviderFactoryAdapter(IServiceProviderFactory<TContainerBuilder> serviceProviderFactory)
        {
            _serviceProviderFactory = serviceProviderFactory;
        }

        public ServiceProviderFactoryAdapter(Func<HostBuilderContext> contextResolver, Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factoryResolver)
        {
            _contextResolver = contextResolver;
            _factoryResolver = factoryResolver;
        }

        public object CreateBuilder(IServiceCollection services)
        {
            if (_serviceProviderFactory == null)
            {
                Debug.Assert(_factoryResolver != null && _contextResolver != null);
                _serviceProviderFactory = _factoryResolver(_contextResolver());

                if (_serviceProviderFactory == null)
                {
                    throw new InvalidOperationException("service provider factory returns null");
                }
            }
            return _serviceProviderFactory.CreateBuilder(services);
        }
        public IServiceProvider CreateServiceProvider(object containerBuilder)
        {
            if (_serviceProviderFactory == null)
            {
                throw new InvalidOperationException("call CreateBuilder first");
            }

            return _serviceProviderFactory.CreateServiceProvider((TContainerBuilder)containerBuilder);
        }
    }
}
