using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class FuncionarioViewModel
    {
        public FuncionarioModel Funcionario { get; set; }
        public List<PuestosModel> Puestos { get; set; }
        public List<ConglomeradoModel> Conglomerados { get; set; }
        public List<DependenciasModel> Dependencias { get; set; }
        public List<RolesModel> Roles { get; set; }
        public List<EstadoFuncionarioModel> EstadosFuncionario { get; set; }
        public List <AreasModel>  Areas { get; set; }
        public List <FuncionarioModel> Jefes { get; set; }
        public List <CarrerasModel> Carreras { get; set; }

        public List<int> IdConglomeradosSeleccionados { get; set; }
        public List<int> IdAreasSeleccionadas { get; set; }
    }
}
