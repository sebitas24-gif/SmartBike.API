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
    public class HistorialItem
    {
        public string Icono { get; set; } = string.Empty;
        public string Transporte { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty;
        public string Co2 { get; set; } = string.Empty;
    }

    public class DashboardViewModel : INotifyPropertyChanged
    {
        private string _nombreUsuario = "Usuario";
        private string _tipoUsuario = "Estudiante";
        private string _co2Hoy = "0.0";
        private string _mensajeImpacto = "¡Sin registros hoy! Usa bicicleta 🚲";
        private double _barraProgreso = 0;
        private string _porcentajeMeta = "0% de tu meta diaria";
        private string _co2Ahorrado = "0.0";
        private string _diasBicicleta = "0";
        private string _arbolesEquivalentes = "0";
        private string _rachaActual = "0";
        private int _transporteSeleccionado = 0;
        private string _recomendacion = "Usar bicicleta en trayectos cortos reduce hasta 4.6 kg de CO₂ diarios. ¡Pequeños cambios hacen grandes diferencias!";
        private string _incentivo = "Completa 5 días en bici y desbloquea el badge 🌱 Eco Rider";

        public ICommand SeleccionarTransporteCommand { get; }
        public ICommand RegistrarTransporteCommand { get; }
        public ICommand VerHistorialCommand { get; }
        public ICommand CerrarSesionCommand { get; }

        public ObservableCollection<HistorialItem> HistorialReciente { get; } = new();

        public DashboardViewModel()
        {
            SeleccionarTransporteCommand = new Command<string>(SeleccionarTransporte);
            RegistrarTransporteCommand = new Command(async () => await RegistrarTransporteAsync());
            VerHistorialCommand = new Command(async () => await Shell.Current.GoToAsync("HistorialPage"));
            CerrarSesionCommand = new Command(async () => await CerrarSesionAsync());
            CargarDatosMock();
        }

        public string NombreUsuario
        {
            get => _nombreUsuario;
            set { _nombreUsuario = value; OnPropertyChanged(); }
        }
        public string TipoUsuario
        {
            get => _tipoUsuario;
            set { _tipoUsuario = value; OnPropertyChanged(); }
        }
        public string Co2Hoy
        {
            get => _co2Hoy;
            set { _co2Hoy = value; OnPropertyChanged(); }
        }
        public string MensajeImpacto
        {
            get => _mensajeImpacto;
            set { _mensajeImpacto = value; OnPropertyChanged(); }
        }
        public double BarraProgreso
        {
            get => _barraProgreso;
            set { _barraProgreso = value; OnPropertyChanged(); }
        }
        public string PorcentajeMeta
        {
            get => _porcentajeMeta;
            set { _porcentajeMeta = value; OnPropertyChanged(); }
        }
        public string Co2Ahorrado
        {
            get => _co2Ahorrado;
            set { _co2Ahorrado = value; OnPropertyChanged(); }
        }
        public string DiasBicicleta
        {
            get => _diasBicicleta;
            set { _diasBicicleta = value; OnPropertyChanged(); }
        }
        public string ArbolesEquivalentes
        {
            get => _arbolesEquivalentes;
            set { _arbolesEquivalentes = value; OnPropertyChanged(); }
        }
        public string RachaActual
        {
            get => _rachaActual;
            set { _rachaActual = value; OnPropertyChanged(); }
        }
        public string Recomendacion
        {
            get => _recomendacion;
            set { _recomendacion = value; OnPropertyChanged(); }
        }
        public string Incentivo
        {
            get => _incentivo;
            set { _incentivo = value; OnPropertyChanged(); }
        }

        public string ColorBicicleta => _transporteSeleccionado == 1 ? "#2E7D32" : "#F1F8E9";
        public string ColorBus => _transporteSeleccionado == 2 ? "#1565C0" : "#E3F2FD";
        public string ColorAuto => _transporteSeleccionado == 3 ? "#BF360C" : "#FBE9E7";
        public string ColorPie => _transporteSeleccionado == 4 ? "#6A1B9A" : "#F3E5F5";
        public string TextoBicicleta => _transporteSeleccionado == 1 ? "White" : "#424242";
        public string TextoBus => _transporteSeleccionado == 2 ? "White" : "#424242";
        public string TextoAuto => _transporteSeleccionado == 3 ? "White" : "#424242";
        public string TextoPie => _transporteSeleccionado == 4 ? "White" : "#424242";

        private void SeleccionarTransporte(string id)
        {
            _transporteSeleccionado = int.Parse(id);
            OnPropertyChanged(nameof(ColorBicicleta)); OnPropertyChanged(nameof(ColorBus));
            OnPropertyChanged(nameof(ColorAuto)); OnPropertyChanged(nameof(ColorPie));
            OnPropertyChanged(nameof(TextoBicicleta)); OnPropertyChanged(nameof(TextoBus));
            OnPropertyChanged(nameof(TextoAuto)); OnPropertyChanged(nameof(TextoPie));
        }

        private async Task RegistrarTransporteAsync()
        {
            if (_transporteSeleccionado == 0)
            {
                await Shell.Current.DisplayAlert("Aviso", "Por favor selecciona un medio de transporte.", "OK");
                return;
            }

            var co2Map = new Dictionary<int, decimal> { { 1, 0.0m }, { 2, 1.5m }, { 3, 4.6m }, { 4, 0.0m } };
            var nombres = new Dictionary<int, string> { { 1, "Bicicleta" }, { 2, "Bus" }, { 3, "Auto" }, { 4, "A Pie" } };
            var iconos = new Dictionary<int, string> { { 1, "🚲" }, { 2, "🚌" }, { 3, "🚗" }, { 4, "🚶" } };

            decimal co2 = co2Map[_transporteSeleccionado];
            Co2Hoy = co2.ToString("0.0");
            MensajeImpacto = co2 == 0
                ? "¡Excelente! Cero emisiones hoy 🌿"
                : $"Equivale a {(co2 / 21.7m * 365m):0.0} días de absorción de un árbol";

            HistorialReciente.Insert(0, new HistorialItem
            {
                Icono = iconos[_transporteSeleccionado],
                Transporte = nombres[_transporteSeleccionado],
                Fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                Co2 = co2.ToString("0.0")
            });

            await Shell.Current.DisplayAlert("✅ Registrado",
                $"Transporte: {nombres[_transporteSeleccionado]}\nCO₂: {co2:0.0} kg", "OK");

            _transporteSeleccionado = 0;
            OnPropertyChanged(nameof(ColorBicicleta)); OnPropertyChanged(nameof(ColorBus));
            OnPropertyChanged(nameof(ColorAuto)); OnPropertyChanged(nameof(ColorPie));
            OnPropertyChanged(nameof(TextoBicicleta)); OnPropertyChanged(nameof(TextoBus));
            OnPropertyChanged(nameof(TextoAuto)); OnPropertyChanged(nameof(TextoPie));
        }

        private void CargarDatosMock()
        {
            Co2Ahorrado = "12.3";
            DiasBicicleta = "8";
            ArbolesEquivalentes = "2";
            RachaActual = "3";
            BarraProgreso = 120;
            PorcentajeMeta = "44% de tu meta semanal";

            HistorialReciente.Add(new HistorialItem { Icono = "🚲", Transporte = "Bicicleta", Fecha = "23/05/2026", Co2 = "0.0" });
            HistorialReciente.Add(new HistorialItem { Icono = "🚌", Transporte = "Bus", Fecha = "22/05/2026", Co2 = "1.5" });
            HistorialReciente.Add(new HistorialItem { Icono = "🚗", Transporte = "Auto", Fecha = "21/05/2026", Co2 = "4.6" });
        }

        private async Task CerrarSesionAsync()
        {
            bool confirmar = await Shell.Current.DisplayAlert(
                "Cerrar Sesión", "¿Estás seguro que deseas salir?", "Sí, salir", "Cancelar");

            if (confirmar)
                await Shell.Current.GoToAsync("//LoginPage");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
