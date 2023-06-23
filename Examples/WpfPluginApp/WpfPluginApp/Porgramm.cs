using Microsoft.Extensions.Hosting;
using ReactiveFramework.Hosting;
using ReactiveFramework.Modularity.Extensions;
using ReactiveFramework.WPF.Hosting;
using WpfPlugin;
using WpfPluginApp;
using WpfPluginApp.ViewModels;
using WpfPluginApp.Views;

var builder = RxHost.CreateDefaultBuilder(args);

builder.ConfigureWpfApp<App>();
builder.UseSplashWindow<SplashWindowViewModel>();


await builder.RegisterModuleAsync<WPFModule>(default);

var app = builder.Build();
await app.RunAsync();