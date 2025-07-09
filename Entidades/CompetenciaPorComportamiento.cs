using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class CompetenciaPorComportamiento
    {
       public int idCompetencia {  get; set; }
           public int idComportamiento { get; set; }
           public string Observaciones {  get; set; }
           public int NivelObtenido { get; set; }
    }//fin class
}
