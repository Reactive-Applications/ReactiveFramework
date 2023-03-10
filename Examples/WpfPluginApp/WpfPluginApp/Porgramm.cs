using Microsoft.Extensions.Hosting;
using ReactiveFramework.Hosting;
using ReactiveFramework.WPF.Hosting;
using WpfPlugin;
using WpfPluginApp;
using WpfPluginApp.ViewModels;

var builder = PluginApplication.CreateDefaultBuilder(args);

builder
    .UseWPF<App>()
    .UseSplashWindow<SplashWindowViewModel>();

var app = builder.Build();

app.Plugins.Add<WPFPlugin>();

await app.Initialize();

await app.RunAsync();