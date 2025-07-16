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
    public class TiposCompetenciasNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public TiposCompetenciasNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public TiposCompetenciasModel ConsultarTipoCompetenciaID(int idTipoCompetencia)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Operacion", "SELECT"),
                new SqlParameter("@idTipoCompetencia", idTipoCompetencia),
                new SqlParameter("@Tipo", DBNull.Value),
                new SqlParameter("@Ambito", DBNull.Value),
                new SqlParameter("@idConglomeradoRelacionado", DBNull.Value),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_tiposCompetencias_CRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new TiposCompetenciasModel
            {
                IdTipoCompetencia = Convert.ToInt32(row["idTipoCompetencia"]),
                Tipo = row["Tipo"].ToString(),
                Ambito = row["Ambito"] as string,
                IdConglomeradoRelacionado = row["idConglomeradoRelacionado"] != DBNull.Value
                                        ? Convert.ToInt32(row["idConglomeradoRelacionado"])
                                        : (int?)null
            };
        }

        public List<TiposCompetenciasModel> ListarTiposCompetencias()
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Operacion", "SELECT"),
                new SqlParameter("@idTipoCompetencia", DBNull.Value),
                new SqlParameter("@Tipo", DBNull.Value),
                new SqlParameter("@Ambito", DBNull.Value),
                new SqlParameter("@idConglomeradoRelacionado", DBNull.Value),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_tiposCompetencias_CRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }

            var lista = new List<TiposCompetenciasModel>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new TiposCompetenciasModel
                {
                    IdTipoCompetencia = Convert.ToInt32(row["idTipoCompetencia"]),
                    Tipo = row["Tipo"].ToString(),
                    Ambito = row["Ambito"] as string,
                    IdConglomeradoRelacionado = row["idConglomeradoRelacionado"] != DBNull.Value
                                    ? Convert.ToInt32(row["idConglomeradoRelacionado"])
                                    : (int?)null
                });
            }

            return lista;
        }

        public string CrearTipoCompetencia(TiposCompetenciasModel tipoCompetencia)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Operacion", "INSERT"),
                new SqlParameter("@idTipoCompetencia", DBNull.Value),
                new SqlParameter("@Tipo", tipoCompetencia.Tipo),
                new SqlParameter("@Ambito", (object)tipoCompetencia.Ambito ?? DBNull.Value),
                new SqlParameter("@idConglomeradoRelacionado",
                    tipoCompetencia.IdConglomeradoRelacionado.HasValue
                    ? (object)tipoCompetencia.IdConglomeradoRelacionado.Value
                    : DBNull.Value),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_tiposCompetencias_CRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            return string.IsNullOrWhiteSpace(mensajeError)
                ? "Registro creado exitosamente"
                : "Error: " + mensajeError;
        }

        public string ModificarTipoCompetencia(TiposCompetenciasModel tipoCompetencia)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Operacion", "UPDATE"),
                new SqlParameter("@idTipoCompetencia", tipoCompetencia.IdTipoCompetencia),
                new SqlParameter("@Tipo", tipoCompetencia.Tipo),
                new SqlParameter("@Ambito", (object)tipoCompetencia.Ambito ?? DBNull.Value),
                new SqlParameter("@idConglomeradoRelacionado",
                    tipoCompetencia.IdConglomeradoRelacionado.HasValue
                    ? (object)tipoCompetencia.IdConglomeradoRelacionado.Value
                    : DBNull.Value),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_tiposCompetencias_CRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            return string.IsNullOrWhiteSpace(mensajeError)
                ? "Registro actualizado exitosamente"
                : "Error: " + mensajeError;
        }
        public void EliminarTipoCompetencia(int idTipoCompetencia)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Operacion", "DELETE"),
                new SqlParameter("@idTipoCompetencia", idTipoCompetencia),
                new SqlParameter("@Tipo", DBNull.Value),
                new SqlParameter("@Ambito", DBNull.Value),
                new SqlParameter("@idConglomeradoRelacionado", DBNull.Value),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };
            _accesoBD.EjecutarSPconDT("sp_tiposCompetencias_CRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }

        public List<TiposCompetenciasModel> ListarTiposCompetenciasXConglomerado(int idConglo)
        {
            var parametros = new List<SqlParameter>
    {
        new SqlParameter("@Operacion","SELECTXCONGLO"),
        new SqlParameter("@idTipoCompetencia", DBNull.Value),
        new SqlParameter("@Tipo", DBNull.Value),
        new SqlParameter("@Ambito", DBNull.Value),
        new SqlParameter("@idConglomeradoRelacionado", DBNull.Value),
        new SqlParameter("@idConglo", idConglo),
        new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
    };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_tiposCompetencias_CRUD", parametros.ToArray());

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }

            var lista = new List<TiposCompetenciasModel>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new TiposCompetenciasModel
                {
                    IdTipoCompetencia = Convert.ToInt32(row["idTipoCompetencia"]),
                    Tipo = row["Tipo"].ToString(),
                    Ambito = row["Ambito"] as string,
                    IdConglomeradoRelacionado = row["idConglomeradoRelacionado"] != DBNull.Value
                                    ? Convert.ToInt32(row["idConglomeradoRelacionado"])
                                    : (int?)null
                });
            }

            return lista;
        }

    }//fin class
}//fin space
