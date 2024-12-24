using Firebase.Database;
using Microsoft.Extensions.Logging;

namespace AlumnosAPP
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

            var firebaseClient = new FirebaseClient("https://eva4-8e56b-default-rtdb.firebaseio.com/");

            builder.Services.AddSingleton(firebaseClient);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
