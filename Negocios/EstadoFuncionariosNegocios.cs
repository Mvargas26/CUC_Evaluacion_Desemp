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
    public class EstadoFuncionariosNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public EstadoFuncionariosNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public List<EstadoFuncionarioModel> ListarEstadosFuncionario()
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Accion", "SELECT"),
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_CrudEstadoFuncionarios", parametros);

            var lista = new List<EstadoFuncionarioModel>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new EstadoFuncionarioModel
                {
                    IdEstadoFuncionario = Convert.ToInt32(row["idEstadofunc"]),
                    Estado = row["EstadoFuncionario"].ToString()
                });
            }

            return lista;
        }

    }//fin class
}
