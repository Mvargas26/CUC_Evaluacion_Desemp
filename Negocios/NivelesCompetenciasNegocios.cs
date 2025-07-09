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
    public class NivelesComportamientosNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public NivelesComportamientosNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public List<NivelComportamientoModel> ListarNivelesComportamientos()
        {
            var parametros = new SqlParameter[]
            {
        new SqlParameter("@Operacion", "SELECT"),
        new SqlParameter("@idNivel", DBNull.Value),
        new SqlParameter("@nombre", DBNull.Value),
        new SqlParameter("@valor", DBNull.Value),
        new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_NivelesComportamientosCRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }

            var lista = new List<NivelComportamientoModel>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new NivelComportamientoModel
                {
                    idNivel = Convert.ToInt32(row["idNivel"]),
                    nombre = row["nombre"].ToString(),
                    valor = Convert.ToInt32(row["valor"])
                });
            }

            return lista;
        }

        public NivelComportamientoModel ConsultarNivelComportamientoID(int idNivel)
        {
            var parametros = new SqlParameter[]
            {
        new SqlParameter("@Operacion", "SELECTID"),
        new SqlParameter("@idNivel", idNivel),
        new SqlParameter("@nombre", DBNull.Value),
        new SqlParameter("@valor", DBNull.Value),
        new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_NivelesCompetenciasCRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new NivelComportamientoModel
            {
                idNivel = Convert.ToInt32(row["idNivel"]),
                nombre = row["nombre"].ToString(),
                valor = Convert.ToInt32(row["valor"])
            };
        }

        public void InsertarNivelComportamiento(NivelComportamientoModel modelo)
        {
            var parametros = new SqlParameter[]
            {
        new SqlParameter("@Operacion", "INSERT"),
        new SqlParameter("@idNivel", DBNull.Value),
        new SqlParameter("@nombre", modelo.nombre),
        new SqlParameter("@valor", modelo.valor),
        new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_NivelesCompetenciasCRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }

        public void ActualizarNivelComportamiento(NivelComportamientoModel modelo)
        {
            var parametros = new SqlParameter[]
            {
        new SqlParameter("@Operacion", "UPDATE"),
        new SqlParameter("@idNivel", modelo.idNivel),
        new SqlParameter("@nombre", modelo.nombre),
        new SqlParameter("@valor", modelo.valor),
        new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_NivelesCompetenciasCRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }

        public void EliminarNivelComportamiento(int idNivel)
        {
            var parametros = new SqlParameter[]
            {
        new SqlParameter("@Operacion", "DELETE"),
        new SqlParameter("@idNivel", idNivel),
        new SqlParameter("@nombre", DBNull.Value),
        new SqlParameter("@valor", DBNull.Value),
        new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_NivelesCompetenciasCRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }

    }//fin class
}
