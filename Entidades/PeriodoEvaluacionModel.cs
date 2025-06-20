using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class PeriodosModel
    {
        public int idPeriodo {  get; set; }
        public int Anio { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFIn { get; set; }
        public Boolean Estado { get; set; }
        public string Nombre { get; set; }


    }//Fin clase
} //Fin Space 
