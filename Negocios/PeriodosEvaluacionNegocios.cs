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

        public List<PeriodoEvaluacionModel> ListarPeriodos()
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "SELECT")
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_CrudPeriodo", parametros);

            var lista = new List<PeriodoEvaluacionModel>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new PeriodoEvaluacionModel
                {
                    Anio = Convert.ToInt32(row["anio"]),
                    FechaMaxima = Convert.ToDateTime(row["fechaMaxima"])
                });
            }

            return lista;
        }

        public void CrearPeriodo(PeriodoEvaluacionModel nuevo)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "INSERT"),
            new SqlParameter("@ANNO", nuevo.Anio),
            new SqlParameter("@FechaM", nuevo.FechaMaxima)
            };

            _accesoBD.EjecutarSPconDT("sp_CrudPeriodo", parametros);
        }

        public void ModificarPeriodo(PeriodoEvaluacionModel periodo)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "UPDATE"),
            new SqlParameter("@ANNO", periodo.Anio),
            new SqlParameter("@FechaM", periodo.FechaMaxima)
            };

            _accesoBD.EjecutarSPconDT("sp_CrudPeriodo", parametros);
        }

        public void EliminarPeriodo(int anio)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "DELETE"),
            new SqlParameter("@ANNO", anio)
            };

            _accesoBD.EjecutarSPconDT("sp_CrudPeriodo", parametros);
        }

        public PeriodoEvaluacionModel ConsultarPeriodoPorAnio(int anio)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "SELECT"),
            new SqlParameter("@ANNO", anio)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_CrudPeriodo", parametros);

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new PeriodoEvaluacionModel
            {
                Anio = Convert.ToInt32(row["anio"]),
                FechaMaxima = Convert.ToDateTime(row["fechaMaxima"])
            };
        }
    }
}//fin class
