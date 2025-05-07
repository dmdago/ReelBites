using Microsoft.Extensions.Logging;
using ReelBites.Data;
using ReelBites.Services;
using ReelBites.ViewModels;
using ReelBites.Views;

namespace ReelBites;
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
                fonts.AddFont("FontAwesome.ttf", "FontAwesome");
            });

        // Register services
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<IPreferencesService, PreferencesService>();

        // Register APIs
        builder.Services.AddSingleton<IDramaApi, DramaApi>();
        builder.Services.AddSingleton<IUserApi>(provider =>
            new UserApi(provider.GetRequiredService<HttpClient>(),
                       provider.GetRequiredService<IPreferencesService>()));
        builder.Services.AddSingleton<IAuthApi>(provider =>
            new AuthApi(provider.GetRequiredService<HttpClient>()));

        // Register services
        builder.Services.AddSingleton<IDramaService, DramaService>();
        builder.Services.AddSingleton<IUserService, UserService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<INotificationService, NotificationService>();


        // Register viewmodels
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<DramaDetailsViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<CreateDramaViewModel>();
        builder.Services.AddTransient<ExploreViewModel>();
        builder.Services.AddTransient<NotificationsViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<EditProfileViewModel>();
        //builder.Services.AddTransient<FollowersViewModel>();
        //builder.Services.AddTransient<FollowingViewModel>();
        //builder.Services.AddTransient<SettingsViewModel>();

        // Register pages
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<DramaDetailsPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<CreateDramaPage>();
        builder.Services.AddTransient<ExplorePage>();
        builder.Services.AddTransient<NotificationsPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<EditProfilePage>();
        //builder.Services.AddTransient<FollowersPage>();
        //builder.Services.AddTransient<FollowingPage>();
        //builder.Services.AddTransient<SettingsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
