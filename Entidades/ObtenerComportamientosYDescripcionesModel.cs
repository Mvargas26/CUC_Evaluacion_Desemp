using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class ObtenerComportamientosYDescripcionesModel
    {
        public int idCompetencia { get; set; }
        public string Competencia { get; set; }
        public string DescriCompetencia { get; set; }
        public int idComport { get; set; }
        public string Comportamiento { get; set; }
        public string Nivel { get; set; }
        public string Descripcion { get; set; }
    }//fin class
}
