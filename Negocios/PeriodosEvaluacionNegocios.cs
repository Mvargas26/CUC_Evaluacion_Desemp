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
    public class PeriodosEvaluacionNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public PeriodosEvaluacionNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public List<PeriodosModel> ListarPeriodos()
        {
            try
            {
                var parametros = new SqlParameter[]
                {
            new SqlParameter("@Accion", "SELECT"),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
                };

                DataTable dt = _accesoBD.EjecutarSPconDT("sp_PeriodosCRUD", parametros);

                string mensajeError = parametros.Last().Value?.ToString();
                if (!string.IsNullOrWhiteSpace(mensajeError))
                {
                    throw new Exception("Error SP: " + mensajeError);
                }

                return dt.AsEnumerable().Select(row => new PeriodosModel
                {
                    idPeriodo = Convert.ToInt32(row["idPeriodo"]),
                    Anio = Convert.ToInt32(row["anio"]),
                    FechaInicio = Convert.ToDateTime(row["fechaInicio"]),
                    FechaFin = Convert.ToDateTime(row["fechaFin"]),
                    Estado = Convert.ToBoolean(row["estado"]),
                    Nombre = row["nombre"].ToString()
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar los períodos: " + ex.Message);
            }
        }

        public List<PeriodosModel> ListarPeriodosAnioActual()
        {
            try
            {
                // Obtenemos el año actual
                int anioActual = DateTime.Now.Year;

                var parametros = new SqlParameter[]
                {
            new SqlParameter("@Accion", "SELECT"),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
                };

                DataTable dt = _accesoBD.EjecutarSPconDT("sp_PeriodosCRUD", parametros);

                string mensajeError = parametros.Last().Value?.ToString();
                if (!string.IsNullOrWhiteSpace(mensajeError))
                {
                    throw new Exception("Error SP: " + mensajeError);
                }

                // Filtrar por año actual
                return dt.AsEnumerable()
                        .Where(row => Convert.ToInt32(row["anio"]) == anioActual)
                        .Select(row => new PeriodosModel
                        {
                            idPeriodo = Convert.ToInt32(row["idPeriodo"]),
                            Anio = Convert.ToInt32(row["anio"]),
                            FechaInicio = Convert.ToDateTime(row["fechaInicio"]),
                            FechaFin = Convert.ToDateTime(row["fechaFin"]),
                            Estado = Convert.ToBoolean(row["estado"]),
                            Nombre = row["nombre"].ToString()
                        }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar los períodos del año actual: " + ex.Message);
            }
        }

        public void CrearPeriodo(PeriodosModel periodo)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
            new SqlParameter("@Accion", "CREATE"),
            new SqlParameter("@fechaInicio", periodo.FechaInicio),
            new SqlParameter("@fechaFin", periodo.FechaFin),
            new SqlParameter("@anio", periodo.Anio),
            new SqlParameter("@estado", periodo.Estado),
            new SqlParameter("@nombre", periodo.Nombre),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
                };

                _accesoBD.EjecutarSPconDT("sp_PeriodosCRUD", parametros);

                string mensajeError = parametros.Last().Value?.ToString();
                if (!string.IsNullOrWhiteSpace(mensajeError))
                {
                    throw new Exception("Error SP: " + mensajeError);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el período: " + ex.Message);
            }
        }

        public void ModificarPeriodo(PeriodosModel periodo)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
            new SqlParameter("@Accion", "UPDATE"),
            new SqlParameter("@idPeriodo", periodo.idPeriodo),
            new SqlParameter("@fechaInicio", periodo.FechaInicio),
            new SqlParameter("@fechaFin", periodo.FechaFin),
            new SqlParameter("@anio", periodo.Anio),
            new SqlParameter("@estado", periodo.Estado),
            new SqlParameter("@nombre", periodo.Nombre),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
                };

                _accesoBD.EjecutarSPconDT("sp_PeriodosCRUD", parametros);

                string mensajeError = parametros.Last().Value?.ToString();
                if (!string.IsNullOrWhiteSpace(mensajeError))
                {
                    throw new Exception("Error SP: " + mensajeError);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar el período: " + ex.Message);
            }
        }

        public void EliminarPeriodo(int idPeriodo)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
            new SqlParameter("@Accion", "DELETE"),
            new SqlParameter("@idPeriodo", idPeriodo),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
                };

                _accesoBD.EjecutarSPconDT("sp_PeriodosCRUD", parametros);

                string mensajeError = parametros.Last().Value?.ToString();
                if (!string.IsNullOrWhiteSpace(mensajeError))
                {
                    throw new Exception("Error SP: " + mensajeError);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el período: " + ex.Message);
            }
        }
        public PeriodosModel ObtenerPeriodoID(int idPeriodo)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
            new SqlParameter("@Accion", "SELECTID"),
            new SqlParameter("@idPeriodo", idPeriodo),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
                };

                DataTable dt = _accesoBD.EjecutarSPconDT("sp_PeriodosCRUD", parametros);

                string mensajeError = parametros.Last().Value?.ToString();
                if (!string.IsNullOrWhiteSpace(mensajeError))
                {
                    throw new Exception("Error SP: " + mensajeError);
                }

                if (dt.Rows.Count == 0)
                    return null;

                DataRow row = dt.Rows[0];
                return new PeriodosModel
                {
                    idPeriodo = Convert.ToInt32(row["idPeriodo"]),
                    Anio = Convert.ToInt32(row["anio"]),
                    FechaInicio = Convert.ToDateTime(row["fechaInicio"]),
                    FechaFin = Convert.ToDateTime(row["fechaFin"]),
                    Estado = Convert.ToBoolean(row["estado"]),
                    Nombre = row["nombre"].ToString()
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el período: " + ex.Message);
            }
        }

    }//fin class
}//fin spce
