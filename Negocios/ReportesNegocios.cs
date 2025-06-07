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
    public class ReportesNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public ReportesNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public List<ReporteMolde> ListarReportes()
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "SELECT")
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_Reportes", parametros);
            var lista = new List<ReporteMolde>();

            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new ReporteMolde
                {
                    Cedula = row["cedula"].ToString(),
                    Nombre = row["Nombre"].ToString(),
                    IdEva = Convert.ToInt32(row["idEvaluacion"]),
                    Fecha = Convert.ToDateTime(row["fechaCreacion"]),
                    Observaciones = row["Observaciones"].ToString(),
                    Objetivo = row["Objetivo"].ToString(),
                    Competencia = row["Competencia"].ToString(),
                    Nota = Convert.ToDouble(row["ValorObtenido"])
                });
            }

            return lista;
        }

        public List<ReporteMolde> ListarReportesID(string cedula)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "SELECTUNO"),
            new SqlParameter("@ID", cedula)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_Reportes", parametros);
            var lista = new List<ReporteMolde>();

            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new ReporteMolde
                {
                    Cedula = row["cedula"].ToString(),
                    Nombre = row["Nombre"].ToString(),
                    IdEva = Convert.ToInt32(row["idEvaluacion"]),
                    Fecha = Convert.ToDateTime(row["fechaCreacion"]),
                    Observaciones = row["Observaciones"].ToString(),
                    Objetivo = row["Objetivo"].ToString(),
                    Competencia = row["Competencia"].ToString(),
                    Nota = Convert.ToDouble(row["ValorObtenido"])
                });
            }

            return lista;
        }
    }
}//fin space
