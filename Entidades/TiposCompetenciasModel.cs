using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
  public class TiposCompetenciasModel
    {
       public int IdTipoCompetencia { get; set; }
        public string Tipo { get; set; }
        public string Ambito { get; set; }
        public int? IdConglomeradoRelacionado { get; set; }
    }
}
