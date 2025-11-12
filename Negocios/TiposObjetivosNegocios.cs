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
    public class TiposObjetivosNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public TiposObjetivosNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public TiposObjetivosModel ConsultarTipoObjetivoPorID(int id)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Operacion", "R"),
                new SqlParameter("@idTipoObjetivo", id),
                new SqlParameter("@Tipo", DBNull.Value),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_TipoObjetivos_CRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new TiposObjetivosModel
            {
                IdTipoObjetivo = Convert.ToInt32(row["idTipoObjetivo"]),
                Tipo = row["Tipo"].ToString()
            };
        }


        public List<TiposObjetivosModel> ListarTiposObjetivos()
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Operacion", "R"),
                new SqlParameter("@idTipoObjetivo", DBNull.Value),
                new SqlParameter("@Tipo", DBNull.Value),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_TipoObjetivos_CRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }

            var lista = new List<TiposObjetivosModel>();

            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new TiposObjetivosModel
                {
                    IdTipoObjetivo = Convert.ToInt32(row["idTipoObjetivo"]),
                    Tipo = row["Tipo"].ToString()
                });
            }

            return lista;
        }


        public void CrearTipoObjetivo(TiposObjetivosModel tipoObjetivoNuevo)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Operacion", "C"),
                new SqlParameter("@idTipoObjetivo", DBNull.Value),
                new SqlParameter("@Tipo", tipoObjetivoNuevo.Tipo),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_TipoObjetivos_CRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }

        public void ModificarTipoObjetivo(TiposObjetivosModel tipoObjetivo)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Operacion", "U"),
                new SqlParameter("@idTipoObjetivo", tipoObjetivo.IdTipoObjetivo),
                new SqlParameter("@Tipo", tipoObjetivo.Tipo),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_TipoObjetivos_CRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }

        public void EliminarTipoObjetivo(int idTipoObjetivo)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Operacion", "D"),
                new SqlParameter("@idTipoObjetivo", idTipoObjetivo),
                new SqlParameter("@Tipo", DBNull.Value),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_TipoObjetivos_CRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }

    }
}//fin class
  