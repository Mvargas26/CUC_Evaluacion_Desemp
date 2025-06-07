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
    public class EvaluacionesNegocio
    {
        private readonly IAccesoBD _accesoBD;

        public EvaluacionesNegocio(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public List<EvaluacionModel> ListarEvaluaciones()
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "SELECT")
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_CrudEvaluaciones", parametros);

            var lista = new List<EvaluacionModel>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new EvaluacionModel
                {
                    IdEvaluacion = Convert.ToInt32(row["idEvaluacion"]),
                    IdFuncionario = row["idFuncionario"].ToString(),
                    Observaciones = row["Observaciones"]?.ToString(),
                    FechaCreacion = Convert.ToDateTime(row["fechaCreacion"]),
                    EstadoEvaluacion = Convert.ToInt32(row["estadoEvaluacion"])
                });
            }

            return lista;
        }

        public EvaluacionModel CrearEvaluacion(EvaluacionModel nueva)
        {
            string fechaTratada = nueva.FechaCreacion.ToString("yyyy-MM-dd");
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "C"),
            new SqlParameter("@idFuncionario", nueva.IdFuncionario),
            new SqlParameter("@observaciones", (object)nueva.Observaciones ?? DBNull.Value),
            new SqlParameter("@fechaCreacion", fechaTratada),
            new SqlParameter("@estadoEvaluacion", nueva.EstadoEvaluacion),
            new SqlParameter("@idConglomerado", nueva.IdConglomerado)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("[sp_Evaluacion_CRUD]", parametros);

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new EvaluacionModel
            {
                IdEvaluacion = Convert.ToInt32(row["idEvaluacion"]),
            };
        }

        public void ModificarEvaluacion(EvaluacionModel evaluacion)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "U"),
            new SqlParameter("@idEvaluacion", evaluacion.IdEvaluacion),
            new SqlParameter("@idFuncionario", evaluacion.IdFuncionario),
            new SqlParameter("@idConglomerado", (object)evaluacion.IdConglomerado ?? DBNull.Value),
            new SqlParameter("@Observaciones", (object)evaluacion.Observaciones ?? DBNull.Value),
            new SqlParameter("@fechaCreacion", evaluacion.FechaCreacion),
            new SqlParameter("@estadoEvaluacion", evaluacion.EstadoEvaluacion)
            };

            _accesoBD.EjecutarSPconDT("[adm].[sp_Evaluacion_CRUD]", parametros);
        }

        public void EliminarEvaluacion(int idEvaluacion)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "DELETE"),
            new SqlParameter("@IdEvaluacion", idEvaluacion)
            };

            _accesoBD.EjecutarSPconDT("sp_CrudEvaluaciones", parametros);
        }

        public EvaluacionModel ConsultarEvaluacionPorID(int idEvaluacion)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion","R"),
            new SqlParameter("@idEvaluacion", idEvaluacion)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_Evaluacion_CRUD", parametros);

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new EvaluacionModel
            {
                IdEvaluacion = Convert.ToInt32(row["idEvaluacion"]),
                IdFuncionario = row["idFuncionario"] != DBNull.Value ? row["idFuncionario"].ToString() : null,
                IdConglomerado = Convert.ToInt32(row["idConglomerado"]),
                Observaciones = row["Observaciones"] != DBNull.Value ? row["Observaciones"].ToString() : null,
                FechaCreacion = Convert.ToDateTime(row["fechaCreacion"]),
                EstadoEvaluacion = Convert.ToInt32(row["estadoEvaluacion"])
            };
        }

        public EvaluacionModel ConsultarEvaluacionComoFuncionario(string idFuncionario, int idConglomerado)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@idFuncionario", idFuncionario),
            new SqlParameter("@idConglomerado", idConglomerado)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_consultarEvaluacionComoFuncionario", parametros);

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new EvaluacionModel
            {
                IdEvaluacion = Convert.ToInt32(row["idEvaluacion"]),
                IdFuncionario = row["idFuncionario"].ToString(),
                Observaciones = row["Observaciones"]?.ToString(),
                FechaCreacion = Convert.ToDateTime(row["fechaCreacion"]),
                EstadoEvaluacion = Convert.ToInt32(row["estadoEvaluacion"]),
            };
        }

        public EvaluacionModel ConsultarEvaluacionPorAprobar(string idFuncionario, int idConglomerado)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@idFuncionario", idFuncionario),
            new SqlParameter("@idConglomerado", idConglomerado)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_ConsultarEvaluacionPorAprobar", parametros);

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new EvaluacionModel
            {
                IdEvaluacion = Convert.ToInt32(row["idEvaluacion"]),
                IdFuncionario = row["idFuncionario"].ToString(),
                Observaciones = row["Observaciones"]?.ToString(),
                FechaCreacion = Convert.ToDateTime(row["fechaCreacion"]),
                EstadoEvaluacion = Convert.ToInt32(row["estadoEvaluacion"]),
            };
        }

        public (List<ObjetivoModel>, List<CompetenciasModel>) ListarObjYCompetenciasXConglomerado(int idConglomerado)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@idConglomerado", idConglomerado)
            };

            DataSet ds = _accesoBD.EjecutarSPconDS("sp_ListaObjetivosYCompetencias", parametros);

            var listaObjetivos = new List<ObjetivoModel>();
            var listaCompetencias = new List<CompetenciasModel>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    listaObjetivos.Add(new ObjetivoModel
                    {
                        IdObjetivo = Convert.ToInt32(row["idObjetivo"]),
                        Objetivo = row["Objetivo"].ToString(),
                        Porcentaje = Convert.ToDecimal(row["PorcentajeObjetivo"]),
                        IdTipoObjetivo = row.Table.Columns.Contains("idTipoObjetivo") && row["idTipoObjetivo"] != DBNull.Value ?
                                          Convert.ToInt32(row["idTipoObjetivo"]) : (int?)null
                    });
                }
            }

            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    listaCompetencias.Add(new CompetenciasModel
                    {
                        IdCompetencia = Convert.ToInt32(row["idCompetencia"]),
                        Competencia = row["Competencia"].ToString(),
                        Calificacion = Convert.ToDecimal(row["Calificacion"]),
                        IdTipoCompetencia = row.Table.Columns.Contains("idTipoCompetencia") && row["idTipoCompetencia"] != DBNull.Value ?
                                             Convert.ToInt32(row["idTipoCompetencia"]) : (int?)null
                    });
                }
            }

            return (listaObjetivos, listaCompetencias);
        }

        public (List<EvaluacionXObjetivoModel>, List<EvaluacionXcompetenciaModel>) Listar_objetivosYCompetenciasXEvaluacion(int idEvaluacion)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@idEvaluacion", idEvaluacion)
            };

            DataSet ds = _accesoBD.EjecutarSPconDS("sp_objetivosYCompetenciasXEvaluacion", parametros);

            var listaObjetivos = new List<EvaluacionXObjetivoModel>();
            var listaCompetencias = new List<EvaluacionXcompetenciaModel>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    listaCompetencias.Add(new EvaluacionXcompetenciaModel
                    {
                        IdEvaxComp = Convert.ToInt32(row["IdEvaxComp"]),
                        IdEvaluacion = Convert.ToInt32(row["idEvaluacion"]),
                        IdCompetencia = Convert.ToInt32(row["idCompetencia"]),
                        ValorObtenido = Convert.ToDecimal(row["valorObtenido"]),
                        Peso = Convert.ToDecimal(row["peso"]),
                        Meta = row["meta"].ToString(),
                        NombreCompetencia = row["NombreCompetencia"].ToString(),
                        TipoCompetencia = row["TipoCompetencia"].ToString()
                    });
                }
            }

            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    listaObjetivos.Add(new EvaluacionXObjetivoModel
                    {
                        IdEvaxObj = Convert.ToInt32(row["IdEvaxObj"]),
                        IdEvaluacion = Convert.ToInt32(row["idEvaluacion"]),
                        IdObjetivo = Convert.ToInt32(row["idObjetivo"]),
                        ValorObtenido = Convert.ToDecimal(row["valorObtenido"]),
                        Peso = Convert.ToDecimal(row["peso"]),
                        Meta = row["meta"].ToString(),
                        NombreObjetivo = row["NombreObjetivo"].ToString(),
                        TipoObjetivo = row["TipoObjetivo"].ToString()
                    });
                }
            }

            return (listaObjetivos, listaCompetencias);
        }
    
    }//fin class
}//fn space
