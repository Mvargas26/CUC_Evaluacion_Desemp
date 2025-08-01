using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class CompetenciaViewModel
    {
        public CompetenciasModel Competencias { get; set; } 
        public List<ComportamientoModel> Comportamientos { get; set; }
        public List<NivelComportamientoModel> Niveles { get; set; }

        public List<ComportamientoPorNivel> ComportamientoPorNIveles { get; set; }


    }
}
