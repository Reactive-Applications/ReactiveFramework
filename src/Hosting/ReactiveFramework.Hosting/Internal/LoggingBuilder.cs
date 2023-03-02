using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ReactiveFramework.Hosting.Internal;
internal class LoggingBuilder : ILoggingBuilder
{
    public IServiceCollection Services { get; }

    public LoggingBuilder(IServiceCollection services)
    {
        Services = services;
    }
}
