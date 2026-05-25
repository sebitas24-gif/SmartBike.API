using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBike_Mobile.Models
{
    public static class SesionUsuario
    {
        public static string Cedula { get; set; } = string.Empty;
        public static string NombreCompleto { get; set; } = string.Empty;
        public static string CorreoInstitucional { get; set; } = string.Empty;
        public static string TipoUsuario { get; set; } = string.Empty;

        public static void Cerrar()
        {
            Cedula = string.Empty;
            NombreCompleto = string.Empty;
            CorreoInstitucional = string.Empty;
            TipoUsuario = string.Empty;
        }
    }
}
