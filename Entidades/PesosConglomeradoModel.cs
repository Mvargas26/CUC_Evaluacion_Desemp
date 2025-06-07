using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class PesosConglomeradoModel
    {
        public int IdPesoXConglomerado { get; set; }
        public int IdConglomerado { get; set; }
        public int? IdTipoObjetivo { get; set; }
        public int? IdTipoCompetencia { get; set; }
        public decimal Porcentaje { get; set; }

    }//fin class
}//fin space
