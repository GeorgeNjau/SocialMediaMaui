﻿using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace SocialMediaMaui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Ubuntu-Bold.ttf", "UbuntuBold");
                fonts.AddFont("Ubuntu-Regular.ttf", "UbuntuRegular");
            })
            .UseMauiCommunityToolkit();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        ConfigureRefit(builder.Services);

        return builder.Build();
    }

    private static void ConfigureRefit(IServiceCollection services)
    {
        var baseApiUrl = "https://localhost:7022";
        //https://tpthhr0v-7022.inc1.devtunnels.ms/swagger1
    }

}