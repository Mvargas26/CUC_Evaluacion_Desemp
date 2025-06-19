using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class FuncionarioModel
    {
        public string Cedula { get; set; }
        public string Nombre { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellido1} {Apellido2}";
        public string Apellido1 { get; set; }
        public string Apellido2 { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public int IdDepartamento { get; set; }
        public string Departamento { get; set; }
        public int IdRol { get; set; }
        public string Rol { get; set; }
        public int IdPuesto { get; set; }
        public string Puesto { get; set; }
        public int IdConglomerado { get; set; }
        public string NombreConglomerado { get; set; }
        public int IdEstadoFuncionario { get; set; }
        public string Estado { get; set; }
        public string CodigoSeguridad { get; set; }
                public string Telefono { get; set; }



    }//public class
}//fin space
