using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using AvaloniaServerTest.ViewModels;
using AvaloniaServerTest.Views;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaServerTest;

public partial class App : Application
{
    private WebApplication? _app;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        InitializeAspnet();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
            desktop.Exit += DesktopOnExit;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private async void DesktopOnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        if (_app is not null)
            await _app.StopAsync();
    }

    private void InitializeAspnet()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddControllers();
        // builder.WebHost.ConfigureKestrel((context, serverOptions) =>
        // {
        //     var kestrelSection = context.Configuration.GetSection("Kestrel");
        //
        //     serverOptions.Configure(kestrelSection)
        //         .Endpoint("MyServer", listenOptions =>
        //         {
        //             // ...
        //         });
        // });
        _app = builder.Build();

        _app.MapControllers();

        // app.MapGet("/weatherforecast", (HttpContext httpContext) =>
        //     {
        //         var forecast = Enumerable.Range(1, 5).Select(index =>
        //                 new WeatherForecast
        //                 {
        //                     Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        //                     TemperatureC = Random.Shared.Next(-20, 55),
        //                     Summary = summaries[Random.Shared.Next(summaries.Length)]
        //                 })
        //             .ToArray();
        //         return forecast;
        //     })
        //     .WithName("GetWeatherForecast")
        //     .WithOpenApi();

        _ = _app.RunAsync();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}