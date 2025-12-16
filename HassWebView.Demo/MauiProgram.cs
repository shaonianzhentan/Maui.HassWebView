using HassWebView.Core;
using Microsoft.Extensions.Logging;

namespace HassWebView.Demo
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .UseHassWebView()
                .UseImmersiveMode()
                .UseRemoteControl();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // Register pages for dependency injection
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MediaPage>();

            return builder.Build();
        }
    }
}
