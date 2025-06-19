using Datos.Services;
using Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocios
{
    public class FuncionarioPorAreaNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public FuncionarioPorAreaNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public void CrearFuncionarioPorArea(FuncionarioPorAreaModel newFuncionarioPorAreaModel)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Operacion", "INSERT"),
                new SqlParameter("@cedulaFuncionario", newFuncionarioPorAreaModel.cedulaFuncionario),
                new SqlParameter("@idArea", newFuncionarioPorAreaModel.idArea),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_FuncionarioPorAreaCRUD", parametros);

            var mensajeError = parametros[3].Value.ToString();
            if (!string.IsNullOrEmpty(mensajeError))
            {
                throw new Exception(mensajeError);
            }
        }

    }//fin class
}
