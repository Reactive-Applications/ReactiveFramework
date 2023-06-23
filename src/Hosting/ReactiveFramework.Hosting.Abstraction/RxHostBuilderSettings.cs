using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ReactiveFramework.Hosting.Abstraction;
public record RxHostBuilderSettings(
    string[]? Args,
    ConfigurationManager? Configuration,
    string? EnvironmentName,
    string? ApplicationName,
    string? ContentRootPath,
    bool DisableDefaults);