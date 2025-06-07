using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class CompetenciasModel
    {
        public int IdCompetencia { get; set; }
        public string Competencia { get; set; }
        public decimal Calificacion { get; set; }
        public string Tipo { get; set; }
        public int? IdTipoCompetencia { get; set; }
    }
}
