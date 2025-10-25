using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Entidades
{
    public class EvaluacionModel
    {
        public int IdEvaluacion { get; set; }
        
        public string IdFuncionario { get; set; }

        public string Observaciones { get; set; } 

        public DateTime FechaCreacion { get; set; }

        public int EstadoEvaluacion { get; set; }
        public int IdConglomerado { get; set; }
        public int? IdPeriodo { get; set; }
        public decimal? NotaFinal { get; set; }

    }//Fin Clase
}//Fin Space 
