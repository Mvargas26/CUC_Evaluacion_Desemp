using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class PuestosModel
    {
        public int idPuesto { get; set; }
        public string Puesto { get; set; }
        public int IdDependencia { get; set; }
        public string Descripcion { get; set; }
    }
}
