using Datos.Services;
using Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

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
                new SqlParameter("@idArea", DBNull.Value),
                new SqlParameter("@NombreArea", DBNull.Value),
                new SqlParameter("@Descripcion", DBNull.Value),
                new SqlParameter("@Estado", DBNull.Value),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_AreasCRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }

            var lista = new List<AreasModel>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new AreasModel
                {
                    idArea = Convert.ToInt32(row["idArea"]),
                    NombreArea = row["NombreArea"].ToString(),
                    Descripcion = row["Descripcion"].ToString(),
                    Estado = Convert.ToBoolean(row["Estado"])
                });
            }

            return lista;
        }

        public AreasModel ConsultarAreaID(int idArea)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Operacion", "R"),
                new SqlParameter("@idArea", idArea),
                new SqlParameter("@NombreArea", DBNull.Value),
                new SqlParameter("@Descripcion", DBNull.Value),
                new SqlParameter("@Estado", DBNull.Value),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_AreasCRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new AreasModel
            {
                idArea = Convert.ToInt32(row["idArea"]),
                NombreArea = row["NombreArea"].ToString(),
                Descripcion = row["Descripcion"].ToString(),
                Estado = Convert.ToBoolean(row["Estado"])
            };
        }

        public void CrearArea(AreasModel area)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Operacion", "C"),
                new SqlParameter("@idArea", DBNull.Value),
                new SqlParameter("@NombreArea", area.NombreArea),
                new SqlParameter("@Descripcion", area.Descripcion),
                new SqlParameter("@Estado", area.Estado),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_AreasCRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }

        public void ModificarArea(AreasModel area)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Operacion", "U"),
                new SqlParameter("@idArea", area.idArea),
                new SqlParameter("@NombreArea", area.NombreArea),
                new SqlParameter("@Descripcion", area.Descripcion),
                new SqlParameter("@Estado", area.Estado),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_AreasCRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }

        public void EliminarArea(int id)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Operacion", "D"),
                new SqlParameter("@idArea", id),
                new SqlParameter("@NombreArea", DBNull.Value),
                new SqlParameter("@Descripcion", DBNull.Value),
                new SqlParameter("@Estado", DBNull.Value),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_AreasCRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }
    }
}
