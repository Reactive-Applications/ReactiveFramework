using Microsoft.Extensions.Hosting;
using ReactiveFramework.WPF.Hosting;
using WpfPlugin;
using WpfPluginApp;
using WpfPluginApp.ViewModels;

var builder = Host.CreateApplicationBuilder(args);
var hostBuilder = (IHostBuilder)builder;



builder.Build();


var app = builder.Build();


await app.RunAsync();