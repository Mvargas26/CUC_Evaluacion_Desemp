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

        public object ReporteGeneralRH(string tipoReporte, string filtro, string periodo)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@TipoReporte", tipoReporte),
            new SqlParameter("@Filtro", string.IsNullOrEmpty(filtro) ? (object)DBNull.Value : filtro),
            new SqlParameter("@IdPeriodo", Convert.ToInt32(periodo)),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            DataSet ds = _accesoBD.EjecutarSPconDS("sp_ReporteGeneralRH", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError) && mensajeError != "OK")
                throw new Exception("Error SP: " + mensajeError);

            var consolidado = new List<object>();
            var detalle = new List<object>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    consolidado.Add(new
                    {
                        Criterio = row.Table.Columns.Contains("Criterio") ? row["Criterio"]?.ToString() : "",
                        CantidadFuncionarios = row.Table.Columns.Contains("CantidadFuncionarios") && row["CantidadFuncionarios"] != DBNull.Value
                            ? Convert.ToInt32(row["CantidadFuncionarios"])
                            : 0,
                        PromedioNotaFinal = row.Table.Columns.Contains("PromedioNotaFinal") && row["PromedioNotaFinal"] != DBNull.Value
                            ? Convert.ToDecimal(row["PromedioNotaFinal"])
                            : 0m
                    });
                }
            }

            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    detalle.Add(new
                    {
                        Funcionario = row.Table.Columns.Contains("Funcionario") ? row["Funcionario"]?.ToString() : "",
                        Observaciones = row.Table.Columns.Contains("Observaciones") ? row["Observaciones"]?.ToString() : "",
                        NotaFinal = row.Table.Columns.Contains("NotaFinal") && row["NotaFinal"] != DBNull.Value
                            ? Convert.ToDecimal(row["NotaFinal"])
                            : 0m
                    });
                }
            }

            return new
            {
                consolidado = consolidado,
                detalle = detalle
            };
        }


    }//fin class
}//fin space
