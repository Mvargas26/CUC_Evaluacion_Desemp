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
    public class PuestosNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public PuestosNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public List<PuestoModel> ObtenerPuestos()
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "SELECT"),
            new SqlParameter("@idPuesto", DBNull.Value)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_CrudPuesto", parametros);
            var lista = new List<PuestoModel>();

            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new PuestoModel
                {
                    idPuesto = Convert.ToInt32(row["idPuesto"]),
                    Puesto = row["Puesto"].ToString()
                });
            }

            return lista;
        }

        public PuestoModel ObtenerPuestoId(int id)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "SELECT"),
            new SqlParameter("@idPuesto", id)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_CrudPuesto", parametros);

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new PuestoModel
            {
                idPuesto = Convert.ToInt32(row["idPuesto"]),
                Puesto = row["Puesto"].ToString()
            };
        }

        public void AgregarPuesto(PuestoModel puesto)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "INSERT"),
            new SqlParameter("@Puesto", puesto.Puesto)
            };

            _accesoBD.EjecutarSPconDT("sp_CrudPuesto", parametros);
        }

        public void ActualizarPuesto(PuestoModel puesto)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "UPDATE"),
            new SqlParameter("@idPuesto", puesto.idPuesto),
            new SqlParameter("@Puesto", puesto.Puesto)
            };

            _accesoBD.EjecutarSPconDT("sp_CrudPuesto", parametros);
        }

        public void EliminarPuesto(int id)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "DELETE"),
            new SqlParameter("@idPuesto", id)
            };

            _accesoBD.EjecutarSPconDT("sp_CrudPuesto", parametros);
        }
    }
}//fin space
