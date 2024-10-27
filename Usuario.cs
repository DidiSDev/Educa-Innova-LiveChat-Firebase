using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Educa_Innova
{
    // HAGO UNA CLASE USUARIO CON GET Y SET PARA MAPEAR RAPIDO SI ESTÁ O NO EL USUARIO EN LA BBDD
    public class Usuario
    {
        public string id=Guid.NewGuid().ToString("N");
        public string Nombre { get; set; }
        public string Clave { get; set; }
    }
}
