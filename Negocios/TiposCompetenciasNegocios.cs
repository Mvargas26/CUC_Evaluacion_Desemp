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
    public class TiposCompetenciasNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public TiposCompetenciasNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public TiposCompetenciasModel ConsultarTipoCompetenciaID(int idTipoCompetencia)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "R"),
            new SqlParameter("@idTipoCompetencia", idTipoCompetencia)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_TipoCompetencias_CRUD", parametros);

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new TiposCompetenciasModel
            {
                IdTipoCompetencia = Convert.ToInt32(row["idTipoCompetencia"]),
                Tipo = row["Tipo"].ToString()
            };
        }

        public List<TiposCompetenciasModel> ListarTiposCompetencias()
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "R")
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_TipoCompetencias_CRUD", parametros);
            var lista = new List<TiposCompetenciasModel>();

            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new TiposCompetenciasModel
                {
                    IdTipoCompetencia = Convert.ToInt32(row["idTipoCompetencia"]),
                    Tipo = row["Tipo"].ToString()
                });
            }

            return lista;
        }

        public void CrearTipoCompetencia(TiposCompetenciasModel tipoCompetencia)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "C"),
            new SqlParameter("@Tipo", tipoCompetencia.Tipo)
            };

            _accesoBD.EjecutarSPconDT("sp_TipoCompetencias_CRUD", parametros);
        }

        public void ModificarTipoCompetencia(TiposCompetenciasModel tipoCompetencia)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "U"),
            new SqlParameter("@idTipoCompetencia", tipoCompetencia.IdTipoCompetencia),
            new SqlParameter("@Tipo", tipoCompetencia.Tipo)
            };

            _accesoBD.EjecutarSPconDT("sp_TipoCompetencias_CRUD", parametros);
        }

        public void EliminarTipoCompetencia(int idTipoCompetencia)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "D"),
            new SqlParameter("@idTipoCompetencia", idTipoCompetencia)
            };

            _accesoBD.EjecutarSPconDT("sp_TipoCompetencias_CRUD", parametros);
        }
    }//fin class
}//fin space
