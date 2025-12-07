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

            var detalle = new List<object>();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    detalle.Add(new
                    {
                        Funcionario = row.Table.Columns.Contains("Funcionario") ? row["Funcionario"]?.ToString() : "",
                        NotaFinal = row.Table.Columns.Contains("NotaFinal") && row["NotaFinal"] != DBNull.Value
                            ? Convert.ToDecimal(row["NotaFinal"])
                            : 0m,
                        NombreConglomerado = row.Table.Columns.Contains("NombreConglomerado") ? row["NombreConglomerado"]?.ToString() : "",
                        NivelDesempeno = row.Table.Columns.Contains("NivelDesempeno") ? row["NivelDesempeno"]?.ToString() : "",
                        DescripcionRubro = row.Table.Columns.Contains("DescripcionRubro") ? row["DescripcionRubro"]?.ToString() : "",
                        Observaciones = row.Table.Columns.Contains("Observaciones") ? row["Observaciones"]?.ToString() : ""
                    });
                }
            }

            return new
            {
                detalle = detalle
            };
        }//ReporteGeneralRH


    }//fin class
}//fin space
