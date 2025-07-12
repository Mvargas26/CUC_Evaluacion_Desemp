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
    public class ObtenerComportamientosYDescripcionesNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public ObtenerComportamientosYDescripcionesNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public List<ObtenerComportamientosYDescripcionesModel> ListarComportamientosYDescripcionesNegocios(int idparametro,string operacion)
        {
            var parametros = new SqlParameter[]
            {
        new SqlParameter("@idParametro", idparametro),
        new SqlParameter("@operacion", operacion),
        new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_ObtenerComportamientosYDescripciones", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError) && mensajeError != "OK")
            {
                throw new Exception("Error SP: " + mensajeError);
            }

            var lista = new List<ObtenerComportamientosYDescripcionesModel>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new ObtenerComportamientosYDescripcionesModel
                {
                    idCompetencia = Convert.ToInt32(row["idCompetencia"]),
                    Competencia = row["Competencia"].ToString(),
                    DescriCompetencia = row["DescriCompetencia"].ToString(),
                    idTipoCompetencia = Convert.ToInt32(row["idTipoCompetencia"]),
                    Tipo = row["Tipo"].ToString(),
                    idComport = Convert.ToInt32(row["idComport"]),
                    Comportamiento = row["Comportamiento"].ToString(),
                    Nivel = row["Nivel"].ToString(),
                    Descripcion = row["Descripcion"].ToString()
                });
            }

            return lista;
        }


    }//fin class
}
