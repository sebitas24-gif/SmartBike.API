using SmartBike_Mobile.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Diagnostics;
using SmartBike_Mobile.Models;
using System.Collections.ObjectModel;

namespace SmartBike_Mobile.ViewModels
{

    public class HistorialViewModel : INotifyPropertyChanged
    {
        private readonly IActividadService _actividadService;

        // 1. Constructor vacío (Requerido para que XAML lo instancie)
        public HistorialViewModel() : this(new ActividadService()) { }

        // 2. Constructor real para inyección de dependencias
        public HistorialViewModel(IActividadService actividadService)
        {
            _actividadService = actividadService;
        }

        // 3. Propiedad Observada
        private ObservableCollection<HistorialResponse> _historiales = new();
        public ObservableCollection<HistorialResponse> Historiales
        {
            get => _historiales;
            set { _historiales = value; OnPropertyChanged(); }
        }

        public async Task CargarHistorial(string cedula)
        {
            try
            {
                var lista = await _actividadService.ObtenerResumenTotalesAsync(cedula);

                if (lista != null)
                {
                    Historiales.Clear();
                    foreach (var item in lista)
                    {
                        Historiales.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar historial: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    }
