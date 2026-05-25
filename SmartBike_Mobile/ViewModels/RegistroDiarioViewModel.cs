using SmartBike_Mobile.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmartBike_Mobile.ViewModels
{
    public class RegistroDiarioViewModel : INotifyPropertyChanged
    {
        private readonly ActividadService _actividadService;

        // Propiedades vinculadas (Binding) al XAML
        public string CedulaUsuario { get; set; } = "1002003004"; // Debería venir del Login
        public string TipoTransporte { get; set; } = string.Empty;

        public ICommand RegistrarViajeCommand { get; }

        public RegistroDiarioViewModel()
        {
            _actividadService = new ActividadService();
            RegistrarViajeCommand = new Command(async () => await EjecutarRegistroViaje());
        }

        private async Task EjecutarRegistroViaje()
        {
            var nuevoViaje = new Models.RegistroDiarioCreate
            {
                IdUsuario = CedulaUsuario,        // Debe coincidir con la propiedad de la clase
                IdTipoTransporte = int.Parse(TipoTransporte), // Asegúrate de convertir el string a int
                Co2Generado = 0,                  // O el valor que corresponda
                IdCamara = 0
            };


            var resultado = await _actividadService.RegistrarViajeAsync(nuevoViaje);

            if (resultado != null)
            {
                await Shell.Current.DisplayAlert("Éxito", "Viaje registrado correctamente.", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo registrar el viaje.", "OK");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string p = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }
}
