using RxFramework.Hosting;
using RxFramework.Sandbox.TestPlugin;
using RxFramework.WPF.Hosting;

var builder = HostedWPFApp.CreateDefaultBuilder(args);

builder.Plugins.Add<SandboxPlugin>();

var host = builder.Build();

await host.StartAsync();