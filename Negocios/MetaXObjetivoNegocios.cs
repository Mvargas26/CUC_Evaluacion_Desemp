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
    public class MetaXObjetivoNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public MetaXObjetivoNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public MetaXObjetivoModel ConsultarMetaXObjetivoID(int id)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "R"),
            new SqlParameter("@idMetaXObj", id)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("metaXObjetivo_CRUD", parametros);

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new MetaXObjetivoModel
            {
                IdMetaXObj = Convert.ToInt32(row["idMetaXObj"]),
                IdObjetivo = Convert.ToInt32(row["idObjetivo"]),
                IdMeta = Convert.ToInt32(row["idMeta"]),
                ValorObtenido = Convert.ToDecimal(row["valorObtenido"])
            };
        }

        public List<MetaXObjetivoModel> ListarMetaXObjetivos()
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "R"),
            new SqlParameter("@idMetaXObj", DBNull.Value)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("metaXObjetivo_CRUD", parametros);

            var lista = new List<MetaXObjetivoModel>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new MetaXObjetivoModel
                {
                    IdMetaXObj = Convert.ToInt32(row["idMetaXObj"]),
                    IdObjetivo = Convert.ToInt32(row["idObjetivo"]),
                    IdMeta = Convert.ToInt32(row["idMeta"]),
                    ValorObtenido = Convert.ToDecimal(row["valorObtenido"])
                });
            }

            return lista;
        }

        public int CrearMetaXObjetivo(MetaXObjetivoModel modelo)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "C"),
            new SqlParameter("@idObjetivo", modelo.IdObjetivo),
            new SqlParameter("@idMeta", modelo.IdMeta),
            new SqlParameter("@valorObtenido", modelo.ValorObtenido)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("metaXObjetivo_CRUD", parametros);

            if (dt.Rows.Count > 0)
                return Convert.ToInt32(dt.Rows[0]["idMetaXObj"]);

            return 0;
        }

        public bool ActualizarMetaXObjetivo(MetaXObjetivoModel modelo)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "U"),
            new SqlParameter("@idMetaXObj", modelo.IdMetaXObj),
            new SqlParameter("@idObjetivo", modelo.IdObjetivo),
            new SqlParameter("@idMeta", modelo.IdMeta),
            new SqlParameter("@valorObtenido", modelo.ValorObtenido)
            };

            _accesoBD.EjecutarSPconDT("metaXObjetivo_CRUD", parametros);
            return true;
        }

        public bool EliminarMetaXObjetivo(int id)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "D"),
            new SqlParameter("@idMetaXObj", id)
            };

            _accesoBD.EjecutarSPconDT("metaXObjetivo_CRUD", parametros);
            return true;
        }
    }
}//fin space
