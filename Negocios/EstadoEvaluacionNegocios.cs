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
    public class EstadoEvaluacionNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public EstadoEvaluacionNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public List<EstadoEvaluacionModel> ListarEstados()
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "SELECT")
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_CrudEstadoEvaluacion", parametros);

            var lista = new List<EstadoEvaluacionModel>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new EstadoEvaluacionModel
                {
                    IdEstado = Convert.ToInt32(row["idEstado"]),
                    EstadoEvaluacion = row["EstadoEvaluacion"].ToString()
                });
            }

            return lista;
        }

        public void CrearEstado(EstadoEvaluacionModel nuevo)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "INSERT"),
            new SqlParameter("@EstadoEvaluacion", nuevo.EstadoEvaluacion)
            };

            _accesoBD.EjecutarSPconDT("sp_CrudEstadoEvaluacion", parametros);
        }

        public void ModificarEstado(EstadoEvaluacionModel estado)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "UPDATE"),
            new SqlParameter("@IdEstado", estado.IdEstado),
            new SqlParameter("@EstadoEvaluacion", estado.EstadoEvaluacion)
            };

            _accesoBD.EjecutarSPconDT("sp_CrudEstadoEvaluacion", parametros);
        }

        public void EliminarEstado(int idEstado)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "DELETE"),
            new SqlParameter("@IdEstado", idEstado)
            };

            _accesoBD.EjecutarSPconDT("sp_CrudEstadoEvaluacion", parametros);
        }

        public EstadoEvaluacionModel ConsultarEstadoPorID(int idEstado)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@IdEstado", idEstado)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_ConsultarEstadoEvaluacionPorID", parametros);

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new EstadoEvaluacionModel
            {
                IdEstado = Convert.ToInt32(row["idEstado"]),
                EstadoEvaluacion = row["EstadoEvaluacion"].ToString()
            };
        }
    }//fin class
}//fin space
