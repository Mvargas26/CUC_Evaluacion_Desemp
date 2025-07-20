using Datos;
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
    public class ComportamientosNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public ComportamientosNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public List<ComportamientoModel> ListarComportamientos()
        {
            var parametros = new SqlParameter[]
            {
        new SqlParameter("@Operacion", "SELECT"),
        new SqlParameter("@idComport", DBNull.Value),
        new SqlParameter("@Nombre", DBNull.Value),
        new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_ComportamientosCRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }

            var lista = new List<ComportamientoModel>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new ComportamientoModel
                {
                    idComport = Convert.ToInt32(row["idComport"]),
                    Nombre = row["Nombre"].ToString()
                });
            }

            return lista;
        }

        public ComportamientoModel ConsultarComportamientoID(int idComport)
        {
            var parametros = new SqlParameter[]
            {
        new SqlParameter("@Operacion", "SELECTID"),
        new SqlParameter("@idComport", idComport),
        new SqlParameter("@Nombre", DBNull.Value),
        new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_ComportamientosCRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new ComportamientoModel
            {
                idComport = Convert.ToInt32(row["idComport"]),
                Nombre = row["Nombre"].ToString()
            };
        }

        public void InsertarComportamiento(ComportamientoModel modelo)
        {
            var parametros = new SqlParameter[]
            {
        new SqlParameter("@Operacion", "INSERT"),
        new SqlParameter("@idComport", DBNull.Value),
        new SqlParameter("@Nombre", modelo.Nombre),
        new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_ComportamientosCRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }

        public void ActualizarComportamiento(ComportamientoModel modelo)
        {
            var parametros = new SqlParameter[]
            {
        new SqlParameter("@Operacion", "UPDATE"),
        new SqlParameter("@idComport", modelo.idComport),
        new SqlParameter("@Nombre", modelo.Nombre),
        new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_ComportamientosCRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }

        public void EliminarComportamiento(int idComport)
        {
            var parametros = new SqlParameter[]
            {
        new SqlParameter("@Operacion", "DELETE"),
        new SqlParameter("@idComport", idComport),
        new SqlParameter("@Nombre", DBNull.Value),
        new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_ComportamientosCRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }

    }
}
