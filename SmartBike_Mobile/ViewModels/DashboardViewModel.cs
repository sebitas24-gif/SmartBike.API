using SmartBike_Mobile.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SmartBike_Mobile.Models;
namespace SmartBike_Mobile.ViewModels
{
    public class HistorialItem
    {
        public string Icono { get; set; } = string.Empty;
        public string Transporte { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty;
        public string Co2 { get; set; } = string.Empty;
    }

    // DTO que coincide EXACTAMENTE con lo que devuelve el API
    public class RegistroDiarioResponseDto
    {
        public int IdReg { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Co2Generado { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public string TransporteDetalle { get; set; } = string.Empty;
        public string CamaraCodigo { get; set; } = string.Empty;
    }

    public class RegistroDiarioCreateDto
    {
        public string IdUsuario { get; set; } = string.Empty;
        public int IdTipoTransporte { get; set; }
        public int IdCamara { get; set; } = 1;
        public decimal Co2Generado { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
    }

    public class DashboardViewModel : INotifyPropertyChanged
    {
        private const string BaseUrl = "http://192.168.0.103:5023/api/";
        private readonly HttpClient _http;

        private string _nombreUsuario = string.Empty;
        private string _tipoUsuario = string.Empty;
        private string _co2Hoy = "0.0";
        private string _mensajeImpacto = "¡Registra tu transporte de hoy! 🚲";
        private string _co2Ahorrado = "—";
        private string _diasBicicleta = "—";
        private string _arbolesEquivalentes = "—";
        private string _rachaActual = "—";
        private int _transporteSeleccionado = 0;

        public ICommand SeleccionarTransporteCommand { get; }
        public ICommand RegistrarTransporteCommand { get; }
        public ICommand VerHistorialCommand { get; }
        public ICommand VerReportesCommand { get; }
        public ICommand VerCamaraCommand { get; }
        public ICommand CerrarSesionCommand { get; }

        public ObservableCollection<HistorialItem> HistorialReciente { get; } = new();

        public DashboardViewModel()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (m, c, ch, e) => true
            };
            _http = new HttpClient(handler);

            NombreUsuario = SesionUsuario.NombreCompleto;
            TipoUsuario = SesionUsuario.TipoUsuario;

            SeleccionarTransporteCommand = new Command<string>(SeleccionarTransporte);
            RegistrarTransporteCommand = new Command(async () => await RegistrarTransporteAsync());
            VerHistorialCommand = new Command(async () => await Shell.Current.GoToAsync("HistorialPage"));
            VerReportesCommand = new Command(async () => await Shell.Current.GoToAsync("ReportesPage"));
            VerCamaraCommand = new Command(async () => await Shell.Current.GoToAsync("CamaraPage"));
            CerrarSesionCommand = new Command(async () => await CerrarSesionAsync());

            _ = CargarHistorialRecienteAsync();
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

        public string ColorBicicleta => _transporteSeleccionado == 1 ? "#2E7D32" : "#F1F8E9";
        public string ColorBus => _transporteSeleccionado == 2 ? "#1565C0" : "#E3F2FD";
        public string ColorAuto => _transporteSeleccionado == 3 ? "#BF360C" : "#FBE9E7";
        public string ColorPie => _transporteSeleccionado == 4 ? "#6A1B9A" : "#F3E5F5";
        public string TextoBicicleta => _transporteSeleccionado == 1 ? "White" : "#424242";
        public string TextoBus => _transporteSeleccionado == 2 ? "White" : "#424242";
        public string TextoAuto => _transporteSeleccionado == 3 ? "White" : "#424242";
        public string TextoPie => _transporteSeleccionado == 4 ? "White" : "#424242";

        private async Task CargarHistorialRecienteAsync()
        {
            try
            {
                var lista = await _http.GetFromJsonAsync<List<RegistroDiarioResponseDto>>(
                    $"{BaseUrl}actividades/historial-usuario/{SesionUsuario.Cedula}");

                if (lista != null && lista.Count > 0)
                {
                    HistorialReciente.Clear();

                    // Calcular estadísticas desde los registros reales
                    decimal totalCo2 = lista.Sum(r => r.Co2Generado);
                    int diasBici = lista.Count(r => r.TransporteDetalle.ToLower().Contains("bici"));
                    decimal co2Ahorrado = lista.Where(r => r.TransporteDetalle.ToLower().Contains("bici") ||
                                                            r.TransporteDetalle.ToLower().Contains("pie"))
                                               .Sum(r => r.Co2Generado == 0 ? 4.6m : 0);

                    Co2Ahorrado = co2Ahorrado.ToString("0.0");
                    DiasBicicleta = diasBici.ToString();
                    ArbolesEquivalentes = (totalCo2 > 0 ? (int)(totalCo2 / 21.7m) : 0).ToString();
                    RachaActual = diasBici.ToString();

                    // Mostrar últimos 3
                    foreach (var r in lista.Take(3))
                    {
                        HistorialReciente.Add(new HistorialItem
                        {
                            Icono = ObtenerIcono(r.TransporteDetalle),
                            Transporte = r.TransporteDetalle,
                            Fecha = r.Fecha.ToString("dd/MM/yyyy"),
                            Co2 = r.Co2Generado.ToString("0.0")
                        });
                    }
                }
            }
            catch
            {
                // Sin conexión — queda en "—"
            }
        }

        private string ObtenerIcono(string transporte)
        {
            var t = transporte.ToLower();
            if (t.Contains("bici")) return "🚲";
            if (t.Contains("bus")) return "🚌";
            if (t.Contains("auto") || t.Contains("car")) return "🚗";
            if (t.Contains("pie")) return "🚶";
            return "🚦";
        }

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

            try
            {
                var dto = new RegistroDiarioCreateDto
                {
                    IdUsuario = SesionUsuario.Cedula,
                    IdTipoTransporte = _transporteSeleccionado,
                    Co2Generado = co2,
                    Fecha = DateTime.Now
                };

                var response = await _http.PostAsJsonAsync($"{BaseUrl}actividades/registrar-viaje", dto);

                if (response.IsSuccessStatusCode)
                {
                    Co2Hoy = co2.ToString("0.0");
                    MensajeImpacto = co2 == 0
                        ? "¡Excelente! Cero emisiones hoy 🌿"
                        : $"Equivale a {(co2 / 21.7m * 365m):0.0} días de absorción de un árbol";

                    await Shell.Current.DisplayAlert("✅ Registrado",
                        $"Transporte: {nombres[_transporteSeleccionado]}\nCO₂: {co2:0.0} kg", "OK");

                    // Recargar historial con datos frescos del API
                    await CargarHistorialRecienteAsync();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    await Shell.Current.DisplayAlert("Error", $"No se pudo registrar: {error}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error de conexión", ex.Message, "OK");
            }

            _transporteSeleccionado = 0;
            OnPropertyChanged(nameof(ColorBicicleta)); OnPropertyChanged(nameof(ColorBus));
            OnPropertyChanged(nameof(ColorAuto)); OnPropertyChanged(nameof(ColorPie));
            OnPropertyChanged(nameof(TextoBicicleta)); OnPropertyChanged(nameof(TextoBus));
            OnPropertyChanged(nameof(TextoAuto)); OnPropertyChanged(nameof(TextoPie));
        }

        private async Task CerrarSesionAsync()
        {
            bool confirmar = await Shell.Current.DisplayAlert(
                "Cerrar Sesión", "¿Estás seguro que deseas salir?", "Sí, salir", "Cancelar");

            if (confirmar)
            {
                SesionUsuario.Cerrar();
                await Shell.Current.GoToAsync("LoginPage");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
