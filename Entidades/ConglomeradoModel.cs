using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class ConglomeradoModel
    {
        public int IdConglomerado { get; set; }
     
        public string NombreConglomerado { get; set; }
     
        public string Descripcion { get; set; }

    }//fin class
}//fin space
