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
    public class EvaluacionXObjetivosNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public EvaluacionXObjetivosNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public List<EvaluacionXObjetivoModel> ListarEvaluacionesXObjetivo()
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "R")
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_EvaluacionPorObjetivo_CRUD", parametros);

            var lista = new List<EvaluacionXObjetivoModel>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new EvaluacionXObjetivoModel
                {
                    IdEvaxObj = Convert.ToInt32(row["idEvaxObj"]),
                    IdEvaluacion = Convert.ToInt32(row["idEvaluacion"]),
                    IdObjetivo = Convert.ToInt32(row["idObjetivo"]),
                    ValorObtenido = row["ValorObtenido"] != DBNull.Value ? Convert.ToDecimal(row["ValorObtenido"]) : 0,
                    Peso = row["peso"] != DBNull.Value ? Convert.ToDecimal(row["peso"]) : 0,
                    Meta = row["meta"] != DBNull.Value ? Convert.ToString(row["meta"]) : string.Empty
                });
            }

            return lista;
        }

        public void CrearEvaluacionXObjetivo(EvaluacionXObjetivoModel nueva)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "C"),
            new SqlParameter("@idEvaluacion", nueva.IdEvaluacion),
            new SqlParameter("@idObjetivo", nueva.IdObjetivo),
            new SqlParameter("@ValorObtenido", nueva.ValorObtenido),
            new SqlParameter("@peso", nueva.Peso),
            new SqlParameter("@meta", nueva.Meta ?? string.Empty)
            };

            _accesoBD.EjecutarSPconDT("sp_EvaluacionPorObjetivo_CRUD", parametros);
        }

        public void ModificarEvaluacionXObjetivo(EvaluacionXObjetivoModel evaluacion)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "U"),
            new SqlParameter("@idEvaxObj", evaluacion.IdEvaxObj),
            new SqlParameter("@idEvaluacion", evaluacion.IdEvaluacion),
            new SqlParameter("@idObjetivo", evaluacion.IdObjetivo),
            new SqlParameter("@ValorObtenido", evaluacion.ValorObtenido),
            new SqlParameter("@peso", evaluacion.Peso),
            new SqlParameter("@meta", evaluacion.Meta ?? string.Empty)
            };

            _accesoBD.EjecutarSPconDT("sp_EvaluacionPorObjetivo_CRUD", parametros);
        }

        public void EliminarEvaluacionXObjetivo(int idEvaxObj)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "D"),
            new SqlParameter("@idEvaxObj", idEvaxObj)
            };

            _accesoBD.EjecutarSPconDT("sp_EvaluacionPorObjetivo_CRUD", parametros);
        }

        public EvaluacionXObjetivoModel ConsultarEvaluacionXObjetivoPorID(int idEvaxObj)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "R"),
            new SqlParameter("@idEvaxObj", idEvaxObj)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_EvaluacionPorObjetivo_CRUD", parametros);

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new EvaluacionXObjetivoModel
            {
                IdEvaxObj = Convert.ToInt32(row["idEvaxObj"]),
                IdEvaluacion = Convert.ToInt32(row["idEvaluacion"]),
                IdObjetivo = Convert.ToInt32(row["idObjetivo"]),
                ValorObtenido = row["ValorObtenido"] != DBNull.Value ? Convert.ToDecimal(row["ValorObtenido"]) : 0,
                Peso = row["peso"] != DBNull.Value ? Convert.ToDecimal(row["peso"]) : 0,
                Meta = row["meta"] != DBNull.Value ? Convert.ToString(row["meta"]) : string.Empty
            };
        }
    }

}//fin space
