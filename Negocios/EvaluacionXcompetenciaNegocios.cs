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
    public class EvaluacionXcompetenciaNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public EvaluacionXcompetenciaNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public void CrearEvaluacionXCompetencia(EvaluacionXcompetenciaModel nueva)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "C"),
            new SqlParameter("@idEvaluacion", nueva.IdEvaluacion),
            new SqlParameter("@idCompetencia", nueva.IdCompetencia),
            new SqlParameter("@valorObtenido", nueva.ValorObtenido),
            new SqlParameter("@idComportamiento", nueva.IdCompotamiento),
            new SqlParameter("@idNivel", nueva.IdNivel)
            };

            _accesoBD.EjecutarSPconDT("sp_EvaluacionPorCompetencia_CRUD", parametros);
        }

        public List<EvaluacionXcompetenciaModel> ListarEvaluacionXCompetencia(int? idEvaxComp = null)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "R"),
            new SqlParameter("@idEvaxComp", (object)idEvaxComp ?? DBNull.Value)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_EvaluacionPorCompetencia_CRUD", parametros);

            return dt.AsEnumerable().Select(row => new EvaluacionXcompetenciaModel
            {
                IdEvaxComp = Convert.ToInt32(row["idEvaxComp"]),
                IdEvaluacion = Convert.ToInt32(row["idEvaluacion"]),
                IdCompetencia = Convert.ToInt32(row["idCompetencia"]),
                ValorObtenido = row["valorObtenido"] != DBNull.Value ? Convert.ToDecimal(row["valorObtenido"]) : 0,
                Peso = row["peso"] != DBNull.Value ? Convert.ToDecimal(row["peso"]) : 0,
                Meta = row["meta"] != DBNull.Value ? row["meta"].ToString() : string.Empty
            }).ToList();
        }

        public void ActualizarEvaluacionXCompetencia(EvaluacionXcompetenciaModel actualizada)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "U"),
            new SqlParameter("@idEvaxComp", actualizada.IdEvaxComp),
            new SqlParameter("@idEvaluacion", actualizada.IdEvaluacion),
            new SqlParameter("@idCompetencia", actualizada.IdCompetencia),
            new SqlParameter("@valorObtenido", actualizada.ValorObtenido),
            new SqlParameter("@peso", actualizada.Peso),
            new SqlParameter("@meta", actualizada.Meta ?? string.Empty)
            };

            _accesoBD.EjecutarSPconDT("sp_EvaluacionPorCompetencia_CRUD", parametros);
        }

        public void EliminarEvaluacionXCompetencia(int idEvaxComp)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "D"),
            new SqlParameter("@idEvaxComp", idEvaxComp)
            };

            _accesoBD.EjecutarSPconDT("sp_EvaluacionPorCompetencia_CRUD", parametros);
        }

        public EvaluacionXcompetenciaModel ConsultarEvaXCompPorID(int idEvaxComp)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "R"),
            new SqlParameter("@idEvaxComp", idEvaxComp)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_EvaluacionPorCompetencia_CRUD", parametros);

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new EvaluacionXcompetenciaModel
            {
                IdEvaxComp = Convert.ToInt32(row["idEvaxComp"]),
                IdEvaluacion = Convert.ToInt32(row["idEvaluacion"]),
                IdCompetencia = Convert.ToInt32(row["idCompetencia"]),
                ValorObtenido = row["valorObtenido"] != DBNull.Value ? Convert.ToDecimal(row["valorObtenido"]) : 0,
                Peso = row["peso"] != DBNull.Value ? Convert.ToDecimal(row["peso"]) : 0,
                Meta = row["meta"] != DBNull.Value ? row["meta"].ToString() : string.Empty
            };
        }
    }//fin class
}//fin space
