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
    public class RolesNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public RolesNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public RolesModel ConsultarRolID(int id)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@accion", "Consultar"),
            new SqlParameter("@idRol", id)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_RolesCRUD", parametros);

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new RolesModel
            {
                idRol = Convert.ToInt32(row["idRol"]),
                Rol = row["Rol"].ToString()
            };
        }

        public List<RolesModel> ListarRoles()
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@accion", "Listar")
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_RolesCRUD", parametros);
            var lista = new List<RolesModel>();

            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new RolesModel
                {
                    idRol = Convert.ToInt32(row["idRol"]),
                    Rol = row["Rol"].ToString()
                });
            }

            return lista;
        }

        public int CrearRol(RolesModel rol)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@accion", "Crear"),
            new SqlParameter("@Rol", rol.Rol)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_RolesCRUD", parametros);

            if (dt.Rows.Count > 0 && dt.Rows[0]["idRol"] != DBNull.Value)
            {
                return Convert.ToInt32(dt.Rows[0]["idRol"]);
            }

            throw new Exception("No se pudo obtener el ID del nuevo rol.");
        }

        public bool ActualizarRol(RolesModel rol)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@accion", "Modificar"),
            new SqlParameter("@idRol", rol.idRol),
            new SqlParameter("@Rol", rol.Rol)
            };

            _accesoBD.EjecutarSPconDT("sp_RolesCRUD", parametros);
            return true;
        }

        public bool EliminarRol(int idRol)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@accion", "Eliminar"),
            new SqlParameter("@idRol", idRol)
            };

            _accesoBD.EjecutarSPconDT("sp_RolesCRUD", parametros);
            return true;
        }
    }
}//fin space
