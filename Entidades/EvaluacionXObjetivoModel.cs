using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class EvaluacionXObjetivoModel
    {
        public int IdEvaxObj { get; set; }
        public int IdEvaluacion { get; set; }
        public virtual EvaluacionModel Evaluacion { get; set; }
        public int IdObjetivo { get; set; }
        public virtual ObjetivoModel Objetivo { get; set; }
        public decimal ValorObtenido { get; set; } 
        public decimal Peso { get; set; }
        public string Meta { get; set; }
        public string NombreObjetivo { get; set; }
        public string TipoObjetivo { get; set; }

    }//Fin Clase 
} // Fin Space 
