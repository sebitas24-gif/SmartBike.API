using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBike_Mobile.Services
{
    public interface ISesionService
    {
        string CedulaUsuario { get; set; }
    }

    public class SesionService : ISesionService
    {
        public string CedulaUsuario { get; set; }
    }
}
