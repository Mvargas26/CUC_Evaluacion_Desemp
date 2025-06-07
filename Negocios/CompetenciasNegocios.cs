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
    public class CompetenciasNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public CompetenciasNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public CompetenciasModel ConsultarCompetenciaID(int idCompetencia)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                new SqlParameter("@Operacion", "R"),
                new SqlParameter("@idCompetencia", idCompetencia)
                };

                DataTable dt = _accesoBD.EjecutarSPconDT("sp_Competencias_CRUD", parametros);

                if (dt.Rows.Count == 0)
                    return null;

                DataRow row = dt.Rows[0];

                return new CompetenciasModel
                {
                    IdCompetencia = Convert.ToInt32(row["idCompetencia"]),
                    Competencia = row["Competencia"].ToString(),
                    Calificacion = Convert.ToDecimal(row["Calificacion"]),
                    IdTipoCompetencia = row["idTipoCompetencia"] != DBNull.Value ? Convert.ToInt32(row["idTipoCompetencia"]) : (int?)null
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar la competencia por ID: " + ex.Message);
            }
        }

        public List<CompetenciasModel> ListarCompetencias()
        {
            List<CompetenciasModel> lista = new List<CompetenciasModel>();

            try
            {
                var parametros = new SqlParameter[]
                {
                new SqlParameter("@Operacion", "R"),
                new SqlParameter("@idCompetencia", DBNull.Value)
                };

                DataTable dt = _accesoBD.EjecutarSPconDT("sp_Competencias_CRUD", parametros);

                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(new CompetenciasModel
                    {
                        IdCompetencia = Convert.ToInt32(row["idCompetencia"]),
                        Competencia = row["Competencia"].ToString(),
                        Calificacion = Convert.ToDecimal(row["Calificacion"]),
                        Tipo = row["Tipo"].ToString(),
                        IdTipoCompetencia = row["idTipoCompetencia"] != DBNull.Value ? Convert.ToInt32(row["idTipoCompetencia"]) : (int?)null
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar las competencias: " + ex.Message);
            }

            return lista;
        }

        public void CrearCompetencia(CompetenciasModel competencia)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                new SqlParameter("@Operacion", "C"),
                new SqlParameter("@Competencia", competencia.Competencia),
                new SqlParameter("@Calificacion", competencia.Calificacion),
                new SqlParameter("@idTipoCompetencia", competencia.IdTipoCompetencia)
                };

                _accesoBD.EjecutarSPconDT("sp_Competencias_CRUD", parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear la competencia: " + ex.Message);
            }
        }

        public void ModificarCompetencia(CompetenciasModel competencia)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                new SqlParameter("@Operacion", "U"),
                new SqlParameter("@idCompetencia", competencia.IdCompetencia),
                new SqlParameter("@Competencia", competencia.Competencia),
                new SqlParameter("@Calificacion", competencia.Calificacion),
                new SqlParameter("@idTipoCompetencia", competencia.IdTipoCompetencia)
                };

                _accesoBD.EjecutarSPconDT("sp_Competencias_CRUD", parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar la competencia: " + ex.Message);
            }
        }

        public void EliminarCompetencia(int id)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                new SqlParameter("@Operacion", "D"),
                new SqlParameter("@idCompetencia", id)
                };

                _accesoBD.EjecutarSPconDT("sp_Competencias_CRUD", parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar la competencia: " + ex.Message);
            }
        }
    }//fin class
}//fin space
