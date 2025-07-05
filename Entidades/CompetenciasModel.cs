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
        public string Descripcion { get; set; }
        public int IdTipoCompetencia { get; set; }
        public TiposCompetenciasModel TipoCompetencia { get; set; }
    }
}
