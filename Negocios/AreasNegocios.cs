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
    public class AreasNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public AreasNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }


        public List<AreasModel> ListarArea()
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "R"),
            new SqlParameter("@idArea", DBNull.Value)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_AreasCRUD", parametros);

            var lista = new List<AreasModel>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new AreasModel
                {
                    idArea = Convert.ToInt32(row["idArea"]),
                    NombreArea = row["NombreArea"].ToString(),
                    Descripcion = row["Descripcion"].ToString(),

                });
            }

            return lista;

        }

        public AreasModel ConsultarAreaID(int idArea)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "R"),
            new SqlParameter("@idArea", idArea)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_AreasCRUD", parametros);

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new AreasModel
            {
                idArea = Convert.ToInt32(row["idArea"]),
                NombreArea = row["NombreArea"].ToString(),
                Descripcion = row["Descripcion"].ToString(),
            };
        }

        public void CrearArea(AreasModel area)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "C"),
            new SqlParameter("@NombreArea", area.NombreArea),
            new SqlParameter("@Descripcion", area.Descripcion)


            };

            _accesoBD.EjecutarSPconDT("sp_AreasCRUD", parametros);
        }

        public void ModificarArea(AreasModel area)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "U"),
            new SqlParameter("@idArea", area.idArea),
            new SqlParameter("@NombreArea", area.NombreArea),
            new SqlParameter("@Descripcion", area.Descripcion)

            };

            _accesoBD.EjecutarSPconDT("sp_AreasCRUD", parametros);
        }

        public void EliminarArea(int id)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "D"),
            new SqlParameter("@idArea", id)
            };

            _accesoBD.EjecutarSPconDT("sp_AreasCRUD", parametros);
        }


    }//fin class
}//fin space
