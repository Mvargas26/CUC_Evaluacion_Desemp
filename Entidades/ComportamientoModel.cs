using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class ComportamientoModel
    {
        public int idComport { get; set; }
        public string Nombre { get; set; }

        public List<NivelComportamientoModel> Niveles { get; set; } = new List<NivelComportamientoModel>();
    }

}
