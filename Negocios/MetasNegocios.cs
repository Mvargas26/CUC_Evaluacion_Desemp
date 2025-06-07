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
    public class MetasNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public MetasNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public MetaModel ConsultarMetaID(int id)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "R"),
            new SqlParameter("@idMeta", id)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_Metas_CRUD", parametros);

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new MetaModel
            {
                IdMeta = Convert.ToInt32(row["idMeta"]),
                Meta = row["Meta"].ToString(),
                Porcentaje = Convert.ToDecimal(row["Porcentaje"])
            };
        }

        public List<MetaModel> ListarMetas()
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "R"),
            new SqlParameter("@idMeta", DBNull.Value)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_Metas_CRUD", parametros);

            var lista = new List<MetaModel>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new MetaModel
                {
                    IdMeta = Convert.ToInt32(row["idMeta"]),
                    Meta = row["Meta"].ToString(),
                    Porcentaje = Convert.ToDecimal(row["Porcentaje"])
                });
            }

            return lista;
        }

        public void CrearMeta(MetaModel metaNueva)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "C"),
            new SqlParameter("@Meta", metaNueva.Meta),
            new SqlParameter("@Porcentaje", metaNueva.Porcentaje)
            };

            _accesoBD.EjecutarSPconDT("sp_Metas_CRUD", parametros);
        }

        public void ModificarMeta(MetaModel meta)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "U"),
            new SqlParameter("@idMeta", meta.IdMeta),
            new SqlParameter("@Meta", meta.Meta),
            new SqlParameter("@Porcentaje", meta.Porcentaje)
            };

            _accesoBD.EjecutarSPconDT("sp_Metas_CRUD", parametros);
        }

        public void EliminarMeta(int idMeta)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "D"),
            new SqlParameter("@idMeta", idMeta)
            };

            _accesoBD.EjecutarSPconDT("sp_Metas_CRUD", parametros);
        }
    }//fin class
}//fin space
