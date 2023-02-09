using Microsoft.Extensions.Hosting;
using ReactiveFramework.WPF.Hosting;

var builder = HostedWPFApp.CreateDefaultBuilder(args);

builder.Plugins.Add<WpfPlugin.WpfPlugin>();

var host = builder.Build();

await host.RunAsync();