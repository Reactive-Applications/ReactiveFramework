using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveFramework.Hosting;
using ReactiveFramework.Hosting.Abstraction.Plugins;
using WpfPlugin;

var builder = PluginApplication.CreateDefaultBuilder(args);

var app = builder.Build();

app.Plugins.Add<Testplugin>();

await app.Initialize();

await app.RunAsync();