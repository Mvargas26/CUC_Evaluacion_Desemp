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

       
        public PuestosModel ConsultarPuestoID(int idPuesto)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "R"),
            new SqlParameter("@idPuesto", idPuesto)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_Puesto_CRUD", parametros);

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new PuestosModel
            {
                idPuesto = Convert.ToInt32(row["idPuesto"]),
                Puesto = row["Puesto"].ToString(),
            };
        }

        public List<PuestosModel> ListarPuesto()
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "R")
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_PuestosCRUD", parametros);

            var lista = new List<PuestosModel>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new PuestosModel
                {
                    idPuesto = Convert.ToInt32(row["idPuesto"]),
                    Puesto = row["Puesto"].ToString()
                });
            }

            return lista;

        }

        public void CrearPuesto(PuestosModel puesto)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "C"),
            new SqlParameter("@Puesto", puesto.Puesto),

            };

            _accesoBD.EjecutarSPconDT("sp_Puesto_CRUD", parametros);
        }

        public void ModificarPuesto(PuestosModel puesto)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "U"),
            new SqlParameter("@idPuesto", puesto.idPuesto),
            new SqlParameter("@Puesto", puesto.Puesto)

            };

            _accesoBD.EjecutarSPconDT("sp_Puesto_CRUD", parametros);
        }

        public void EliminarPuesto(int id)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "D"),
            new SqlParameter("@idPuesto", id)
            };

            _accesoBD.EjecutarSPconDT("sp_Puesto_CRUD", parametros);
        }

    }
}