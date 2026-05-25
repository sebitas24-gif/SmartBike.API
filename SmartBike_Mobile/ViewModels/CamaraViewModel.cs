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

namespace SmartBike_Mobile.ViewModels
{
    public class ConteoCamaraItem
    {
        public int IdConteCama { get; set; }
        public string NombreCamara { get; set; } = string.Empty;
        public string IpCamara { get; set; } = string.Empty;
        public bool Estado { get; set; }
        public int IdTipoTransporte { get; set; }
        public DateTime Fecha { get; set; }

        public string FechaTexto => Fecha.ToString("dd/MM/yyyy HH:mm");
        public string EstadoTexto => Estado ? "Activa" : "Inactiva";
        public string ColorEstado => Estado ? "#2E7D32" : "#BF360C";
        public string Icono => IdTipoTransporte switch
        {
            1 => "🚲",
            2 => "🚌",
            3 => "🚗",
            _ => "📷"
        };
        public string ColorFondo => IdTipoTransporte switch
        {
            1 => "#E8F5E9",
            2 => "#E3F2FD",
            3 => "#FBE9E7",
            _ => "#F5F5F5"
        };
    }

    public class CamaraViewModel : INotifyPropertyChanged
    {
        private const string BaseUrl = "http://192.168.0.103:5023/api/";
        private readonly HttpClient _http;

        private bool _cargando = false;
        private bool _hayError = false;
        private string _mensajeError = string.Empty;
        private string _estadoConexion = "SIN DATOS";
        private string _colorEstado = "#F57F17";
        private string _ultimaActualizacion = "Sin actualizar";
        private int _conteoBicicletas = 0;
        private int _conteoVehiculos = 0;

        public ObservableCollection<ConteoCamaraItem> UltimasDetecciones { get; } = new();
        public ICommand ActualizarCommand { get; }

        public CamaraViewModel()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (m, c, ch, e) => true
            };
            _http = new HttpClient(handler);
            ActualizarCommand = new Command(async () => await CargarDatosAsync());
            _ = CargarDatosAsync();
        }

        public bool Cargando { get => _cargando; set { _cargando = value; OnPropertyChanged(); } }
        public bool HayError { get => _hayError; set { _hayError = value; OnPropertyChanged(); } }
        public string MensajeError { get => _mensajeError; set { _mensajeError = value; OnPropertyChanged(); } }
        public string EstadoConexion { get => _estadoConexion; set { _estadoConexion = value; OnPropertyChanged(); } }
        public string ColorEstado { get => _colorEstado; set { _colorEstado = value; OnPropertyChanged(); } }
        public string UltimaActualizacion { get => _ultimaActualizacion; set { _ultimaActualizacion = value; OnPropertyChanged(); } }
        public int ConteoBicicletas { get => _conteoBicicletas; set { _conteoBicicletas = value; OnPropertyChanged(); } }
        public int ConteoVehiculos { get => _conteoVehiculos; set { _conteoVehiculos = value; OnPropertyChanged(); } }

        private async Task CargarDatosAsync()
        {
            Cargando = true;
            HayError = false;
            EstadoConexion = "CONECTANDO";
            ColorEstado = "#F57F17";
            UltimasDetecciones.Clear();

            try
            {
                var lista = await _http.GetFromJsonAsync<List<ConteoCamaraItem>>($"{BaseUrl}camaras/conteos");

                if (lista != null && lista.Count > 0)
                {
                    foreach (var item in lista.OrderByDescending(c => c.Fecha))
                        UltimasDetecciones.Add(item);

                    // Contar bicis vs vehículos según tipo de transporte
                    ConteoBicicletas = lista.Count(c => c.IdTipoTransporte == 1);
                    ConteoVehiculos = lista.Count(c => c.IdTipoTransporte != 1);

                    EstadoConexion = "CONECTADO";
                    ColorEstado = "#2E7D32";
                    UltimaActualizacion = $"Actualizado: {DateTime.Now:HH:mm:ss}";
                }
                else
                {
                    EstadoConexion = "SIN DATOS";
                    ColorEstado = "#F57F17";
                    UltimaActualizacion = "Sin detecciones registradas";
                }
            }
            catch (Exception ex)
            {
                HayError = true;
                MensajeError = $"Error al conectar: {ex.Message}";
                EstadoConexion = "DESCONECTADO";
                ColorEstado = "#BF360C";
            }
            finally
            {
                Cargando = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
