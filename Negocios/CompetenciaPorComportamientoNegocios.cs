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
    public class CompetenciaPorComportamientoNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public CompetenciaPorComportamientoNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public List<CompetenciaPorComportamiento> ListarCompetenciaPorComportamiento(int idCompetencia)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@idCompetencia", idCompetencia),
                new SqlParameter("@idComportamiento", DBNull.Value),
                new SqlParameter("@Observaciones", DBNull.Value),
                new SqlParameter("@NivelObtenido", DBNull.Value),
                new SqlParameter("@Accion", "CONSULTAR"),
                 new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }

            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_CompetenciaPorComportamiento_CRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError) && mensajeError != "OK")
            {
                throw new Exception("Error SP: " + mensajeError);
            }

            var lista = new List<CompetenciaPorComportamiento>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new CompetenciaPorComportamiento
                {
                    idCompetencia = Convert.ToInt32(row["idCompetencia"]),
                    idComportamiento = Convert.ToInt32(row["idComportamiento"]),
                    Observaciones = row["Observaciones"] != DBNull.Value ? row["Observaciones"].ToString() : null,
                    NivelObtenido = row["NivelObtenido"] != DBNull.Value ? Convert.ToInt32(row["NivelObtenido"]) : 0
                });
            }

            return lista;
        }

        public void InsertarCompetenciaPorComportamiento(CompetenciaPorComportamiento modelo)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@idCompetencia", modelo.idCompetencia),
                new SqlParameter("@idComportamiento", modelo.idComportamiento),
                new SqlParameter("@Observaciones", (object)modelo.Observaciones ?? DBNull.Value),
                new SqlParameter("@NivelObtenido", modelo.NivelObtenido == 0 ? DBNull.Value : (object)modelo.NivelObtenido),
                new SqlParameter("@Accion", "INSERTAR"),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 500){Direction = ParameterDirection.Output}
            };

            _accesoBD.EjecutarSPconDT("sp_CompetenciaPorComportamiento_CRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError) && mensajeError != "OK")
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }

        public void ActualizarCompetenciaPorComportamiento(CompetenciaPorComportamiento modelo)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@idCompetencia", modelo.idCompetencia),
                new SqlParameter("@idComportamiento", modelo.idComportamiento),
                new SqlParameter("@Observaciones", (object)modelo.Observaciones ?? DBNull.Value),
                new SqlParameter("@NivelObtenido", modelo.NivelObtenido == 0 ? DBNull.Value : (object)modelo.NivelObtenido),
                new SqlParameter("@Accion", "ACTUALIZAR"),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) {Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_CompetenciaPorComportamiento_CRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError) && mensajeError != "OK")
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }

        public void EliminarCompetenciaPorComportamiento(int idCompetencia, int idComportamiento)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@idCompetencia", idCompetencia),
                new SqlParameter("@idComportamiento", idComportamiento),
                new SqlParameter("@Observaciones", DBNull.Value),
                new SqlParameter("@NivelObtenido", DBNull.Value),
                new SqlParameter("@Accion", "ELIMINAR"),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) {Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_CompetenciaPorComportamiento_CRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError) && mensajeError != "OK")
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }

    }//fin class
}
