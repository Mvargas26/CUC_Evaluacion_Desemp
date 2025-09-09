using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class EvaluacionXcompetenciaModel
    {
        public int IdEvaxComp { get; set; }
        public int IdEvaluacion { get; set; }
        public int IdCompetencia { get; set; }
        public decimal ValorObtenido { get; set; }
        public decimal Peso { get; set; }
        public string Meta { get; set; }
        public string NombreCompetencia { get; set; }
        public string TipoCompetencia { get; set; }
        public int IdComportamiento { get; set; }
        public int IdNivel { get; set; }

    }//fn class
}//fin space
