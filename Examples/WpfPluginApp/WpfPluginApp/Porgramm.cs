using Microsoft.Extensions.Hosting;
using ReactiveFramework.Hosting;

var builder = PluginApplication.CreateBuilder(args);

var app = builder.Build();

await app.StartAsync();