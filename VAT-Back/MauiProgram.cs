using Microsoft.Extensions.Logging;
using VAT_Back.Services; // THIS IS THE KEY LINE

namespace VAT_Back
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
                });

            // SERVICES
            builder.Services.AddSingleton<FirebaseService>();
            builder.Services.AddSingleton<FirebaseStorageService>();

            // PAGES
            builder.Services.AddTransient<RoleSelectionPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<AddReceiptPage>();
            builder.Services.AddTransient<AdminDashboardPage>();
            builder.Services.AddTransient<AdminReviewPage>();
            builder.Services.AddTransient<AboutPage>();
            builder.Services.AddTransient<ReceiptDetailPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}