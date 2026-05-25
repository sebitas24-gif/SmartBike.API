using SmartBike_Mobile.Services;
using SmartBike_Mobile.ViewModels;

namespace SmartBike_Mobile.Views;

public partial class HistorialPage : ContentPage
{
    private readonly HistorialViewModel _viewModel;
    private readonly ISesionService _sesionService;
    public HistorialPage()
	{
		InitializeComponent();
        _sesionService = Shell.Current.Handler.MauiContext.Services.GetService<ISesionService>();
        _viewModel = new HistorialViewModel(new ActividadService());
        BindingContext = _viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Al aparecer, obtenemos la cédula del servicio y cargamos
        var cedula = _sesionService.CedulaUsuario;

        if (!string.IsNullOrEmpty(cedula))
        {
            await _viewModel.CargarHistorial(cedula); 
        }
    }
}