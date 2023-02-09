using Microsoft.Extensions.Hosting;
using ReactiveFramework.WPF.Hosting;

var builder = HostedWPFApp.CreateDefaultBuilder(args);

builder.Plugins.Add<WpfPlugin.WpfPlugin>();

// To add some xaml resources:
//builder.AddResources("path to resource");

var host = builder.Build();

await host.RunAsync();