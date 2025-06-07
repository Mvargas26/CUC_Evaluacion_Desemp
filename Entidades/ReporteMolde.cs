using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class ReporteMolde
    {
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public int IdEva {  get; set; }
        public DateTime Fecha { get; set; }
        public string Observaciones { get; set; }
        public string Objetivo { get; set; }
        public string Competencia { get; set; } 
        public double Nota { get; set; }
        public string Titulo { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public List<string> Contenido { get; set; }
    }
}
