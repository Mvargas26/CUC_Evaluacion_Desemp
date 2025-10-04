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
    public class EstadoEvaluacionNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public EstadoEvaluacionNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public EstadoEvaluacionModel ConsultarEstadoPorID(int idEstado)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Operacion", "R"),
                new SqlParameter("@IdEstado", idEstado),
                new SqlParameter("@EstadoEvaluacion", DBNull.Value),
                new SqlParameter("@Descripcion", DBNull.Value),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_EstadoEvaluacionCRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new EstadoEvaluacionModel
            {
                IdEstado = Convert.ToInt32(row["IdEstado"]),
                EstadoEvaluacion = row["EstadoEvaluacion"].ToString(),
                Descripcion = row["Descripcion"] == DBNull.Value ? null : row["Descripcion"].ToString()
            };
        }

    }//fin class
}//fin space
