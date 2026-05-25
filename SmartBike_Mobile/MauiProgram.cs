using Microsoft.Extensions.Logging;
using SmartBike_Mobile.Services; // Asegúrate de importar tus servicios
using SmartBike_Mobile.ViewModels;
using SmartBike_Mobile.Views; // Asegúrate de importar tus vistas
namespace SmartBike_Mobile;

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
        builder.Services.AddSingleton<IUsuarioService, UsuarioService>();

        // 2. Registro del ViewModel (Transient: se crea cada vez que se navega a la página)
        builder.Services.AddTransient<LoginViewModel>();

        // 3. Registro de Páginas (Si usas Shell, también registra tus páginas)
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddSingleton<ISesionService, SesionService>();
#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
