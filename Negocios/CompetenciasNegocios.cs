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
        public List<CompetenciasModel> ListarCompetencias()
        {
            try
            {
                var parametros = new SqlParameter[]
                {
            new SqlParameter("@Accion", "READ_FULL"),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
                };

                DataTable dt = _accesoBD.EjecutarSPconDT("sp_CompetenciasCRUD", parametros);

                string mensajeError = parametros.Last().Value?.ToString();
                if (!string.IsNullOrWhiteSpace(mensajeError))
                {
                    throw new Exception("Error SP: " + mensajeError);
                }

                return dt.AsEnumerable().Select(row => new CompetenciasModel
                {
                    IdCompetencia = Convert.ToInt32(row["idCompetencia"]),
                    Competencia = row["Competencia"].ToString(),
                    Descripcion = row["Descripcion"].ToString(),
                    TipoCompetencia = new TiposCompetenciasModel
                    {
                        IdTipoCompetencia = Convert.ToInt32(row["idTipoCompetencia"]),
                        Tipo = row["Tipo"].ToString(),
                        Ambito = row["Ambito"].ToString()
                    }
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar las competencias: " + ex.Message);
            }
        }
        public CompetenciasModel ConsultarCompetenciaPorId(int idCompetencia)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
            new SqlParameter("@Accion", "READ"),
            new SqlParameter("@idCompetencia", idCompetencia),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
                };

                DataTable dt = _accesoBD.EjecutarSPconDT("sp_CompetenciasCRUD", parametros);

                string mensajeError = parametros.Last().Value?.ToString();
                if (!string.IsNullOrWhiteSpace(mensajeError))
                {
                    throw new Exception("Error SP: " + mensajeError);
                }

                if (dt.Rows.Count == 0)
                    return null;

                DataRow row = dt.Rows[0];

                return new CompetenciasModel
                {
                    IdCompetencia = Convert.ToInt32(row["idCompetencia"]),
                    Competencia = row["Competencia"].ToString(),
                    Descripcion = row["Descripcion"].ToString(),
                    TipoCompetencia = new TiposCompetenciasModel
                    {
                        IdTipoCompetencia = Convert.ToInt32(row["idTipoCompetencia"])
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar la competencia: " + ex.Message);
            }
        }


        public bool CrearCompetencia(CompetenciasModel competencia)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
            new SqlParameter("@Accion", "CREATE"),
            new SqlParameter("@Competencia", competencia.Competencia),
            new SqlParameter("@Descripcion", competencia.Descripcion),
            new SqlParameter("@idTipoCompetencia", competencia.IdTipoCompetencia),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
                };

                _accesoBD.EjecutarSPconDT("sp_CompetenciasCRUD", parametros);

                string mensajeError = parametros.Last().Value?.ToString();
                if (!string.IsNullOrWhiteSpace(mensajeError) && !mensajeError.Contains("exitosamente"))
                {
                    throw new Exception("Error SP: " + mensajeError);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear la competencia: " + ex.Message);
            }
        }

        public bool ModificarCompetencia(CompetenciasModel competencia)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
            new SqlParameter("@Accion", "UPDATE"),
            new SqlParameter("@idCompetencia", competencia.IdCompetencia),
            new SqlParameter("@Competencia", competencia.Competencia),
            new SqlParameter("@Descripcion", competencia.Descripcion),
            new SqlParameter("@idTipoCompetencia", competencia.IdTipoCompetencia),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
                };

                _accesoBD.EjecutarSPconDT("sp_CompetenciasCRUD", parametros);

                string mensajeError = parametros.Last().Value?.ToString();
                if (!string.IsNullOrWhiteSpace(mensajeError) && !mensajeError.Contains("exitosamente"))
                {
                    throw new Exception("Error SP: " + mensajeError);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar la competencia: " + ex.Message);
            }
        }

        public bool EliminarCompetencia(int idCompetencia)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
            new SqlParameter("@Accion", "DELETE"),
            new SqlParameter("@idCompetencia", idCompetencia),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
                };

                _accesoBD.EjecutarSPconDT("sp_CompetenciasCRUD", parametros);

                string mensajeError = parametros.Last().Value?.ToString();
                if (!string.IsNullOrWhiteSpace(mensajeError) && !mensajeError.Contains("exitosamente"))
                {
                    throw new Exception("Error SP: " + mensajeError);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar la competencia: " + ex.Message);
            }
        }
    }//fin class
}//fin space
