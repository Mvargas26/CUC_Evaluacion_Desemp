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
    public class ObjetivoNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public ObjetivoNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public ObjetivoModel ConsultarObjetivoID(int idObjetivo)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "R"),
            new SqlParameter("@idObjetivo", idObjetivo),
            new SqlParameter("@Objetivo", DBNull.Value),
            new SqlParameter("@Porcentaje", DBNull.Value),
            new SqlParameter("@idTipoObjetivo", DBNull.Value),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }

            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_Objetivos_CRUD", parametros);
            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }


            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new ObjetivoModel
            {
                IdObjetivo = Convert.ToInt32(row["idObjetivo"]),
                Objetivo = row["Objetivo"].ToString(),
                Porcentaje = Convert.ToDecimal(row["Porcentaje"]),
                IdTipoObjetivo = row["idTipoObjetivo"] != DBNull.Value ?
                                 Convert.ToInt32(row["idTipoObjetivo"]) : (int?)null
            };
        }

        public List<ObjetivoModel> ListarObjetivos()
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "R"),
            new SqlParameter("@idObjetivo", DBNull.Value),
            new SqlParameter("@Objetivo", DBNull.Value),
            new SqlParameter("@Porcentaje", DBNull.Value),
            new SqlParameter("@idTipoObjetivo", DBNull.Value),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_Objetivos_CRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }

            var lista = new List<ObjetivoModel>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new ObjetivoModel
                {
                    IdObjetivo = Convert.ToInt32(row["idObjetivo"]),
                    Objetivo = row["Objetivo"].ToString(),
                    Porcentaje = Convert.ToDecimal(row["Porcentaje"]),
                    Tipo = row["Tipo"].ToString(),
                    IdTipoObjetivo = row["idTipoObjetivo"] != DBNull.Value ?
                                    Convert.ToInt32(row["idTipoObjetivo"]) : (int?)null
                });
            }

            return lista;
        }

        public void CrearObjetivo(ObjetivoModel objetivo)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "C"),
              new SqlParameter("@idObjetivo", DBNull.Value),
            new SqlParameter("@Objetivo", objetivo.Objetivo),
            new SqlParameter("@Porcentaje", objetivo.Porcentaje),
            new SqlParameter("@idTipoObjetivo", objetivo.IdTipoObjetivo ?? (object)DBNull.Value),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }

            };

            _accesoBD.EjecutarSPconDT("sp_Objetivos_CRUD", parametros);
            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }

        public void ModificarObjetivo(ObjetivoModel objetivo)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "U"),
            new SqlParameter("@idObjetivo", objetivo.IdObjetivo),
            new SqlParameter("@Objetivo", objetivo.Objetivo),
            new SqlParameter("@Porcentaje", objetivo.Porcentaje),
            new SqlParameter("@idTipoObjetivo", objetivo.IdTipoObjetivo ?? (object)DBNull.Value),
              new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_Objetivos_CRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }

        public void EliminarObjetivo(int id)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "D"),
            new SqlParameter("@idObjetivo", id),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_Objetivos_CRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }

        public List<TiposObjetivosModel> ListarTiposObjetivo()
        {
            var lista = new List<TiposObjetivosModel>();
            var query = "sp_TiposObjetivo_Listar"; 

            var dt = _accesoBD.EjecutarSPconDT(query); 

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


    }
}
