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
            var mensajeError = new SqlParameter("@MensajeError", SqlDbType.VarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            var parametros = new SqlParameter[]
            {
        new SqlParameter("@Operacion", "R"),
        new SqlParameter("@idPuesto", idPuesto),
        new SqlParameter("@Puesto", DBNull.Value),
        new SqlParameter("@idDependencia", DBNull.Value),
        new SqlParameter("@Descripcion", DBNull.Value),
        mensajeError
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_Puestos_CRUD", parametros);

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new PuestosModel
            {
                IdPuesto = Convert.ToInt32(row["idPuesto"]),
                Puesto = row["Puesto"].ToString(),
                Descripcion = row["Descripcion"]?.ToString(),
                IdDependencia = row["idDependencia"] != DBNull.Value ? Convert.ToInt32(row["idDependencia"]) : 0
              
            };
        }


        public List<PuestosModel> ListarPuesto()
        {

            var mensajeError = new SqlParameter("@MensajeError", SqlDbType.VarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            var parametros = new SqlParameter[]
            {
        new SqlParameter("@Operacion", "R"),
        new SqlParameter("@idPuesto", DBNull.Value),
        new SqlParameter("@Puesto", DBNull.Value),
        new SqlParameter("@idDependencia", DBNull.Value),
        new SqlParameter("@Descripcion", DBNull.Value),
        mensajeError
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_Puestos_CRUD", parametros);

            var lista = new List<PuestosModel>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new PuestosModel
                {
                    IdPuesto = Convert.ToInt32(row["idPuesto"]),
                    IdDependencia = Convert.ToInt32(row["idDependencia"]),
                    Puesto = row["Puesto"].ToString(),
                    Descripcion = row["Descripcion"].ToString(),

                });
            }


            return lista;
        }


       

        public void CrearPuesto(PuestosModel puesto)
        {
            var mensajeError = new SqlParameter("@MensajeError", SqlDbType.VarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            var parametros = new SqlParameter[]
            {
        new SqlParameter("@Operacion", "C"),
        new SqlParameter("@idPuesto", DBNull.Value),
        new SqlParameter("@Puesto", puesto.Puesto ?? (object)DBNull.Value),
        new SqlParameter("@idDependencia", puesto.IdDependencia),
        new SqlParameter("@Descripcion", puesto.Descripcion ?? (object)DBNull.Value),
        mensajeError
            };

            _accesoBD.EjecutarSPconDT("sp_Puestos_CRUD", parametros);


            var resultado = mensajeError.Value?.ToString();
            if (!string.IsNullOrEmpty(resultado))
            {

                Console.WriteLine("Mensaje del SP (Crear): " + resultado);
            }
        }

        public void ModificarPuesto(PuestosModel puesto)
        {
            var mensajeError = new SqlParameter("@MensajeError", SqlDbType.VarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            var parametros = new SqlParameter[]
            {
        new SqlParameter("@Operacion", "U"),
        new SqlParameter("@idPuesto", puesto.IdPuesto),
        new SqlParameter("@Puesto", puesto.Puesto ?? (object)DBNull.Value),
        new SqlParameter("@idDependencia", puesto.IdDependencia),
        new SqlParameter("@Descripcion", puesto.Descripcion ?? (object)DBNull.Value),
        mensajeError
            };

            _accesoBD.EjecutarSPconDT("sp_Puestos_CRUD", parametros);


            var resultado = mensajeError.Value?.ToString();
            if (!string.IsNullOrEmpty(resultado))
            {
                Console.WriteLine("Mensaje del SP (Modificar): " + resultado);
            }
        }


        public void EliminarPuesto(int id)
        {
            var mensajeError = new SqlParameter("@MensajeError", SqlDbType.VarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            var parametros = new SqlParameter[]
            {
        new SqlParameter("@Operacion", "D"),
        new SqlParameter("@idPuesto", id),
        new SqlParameter("@Puesto", DBNull.Value),
        new SqlParameter("@idDependencia", DBNull.Value),
        new SqlParameter("@Descripcion", DBNull.Value),
        mensajeError
            };

            _accesoBD.EjecutarSPconDT("sp_Puestos_CRUD", parametros);

            // Opcional: revisar si el SP devolvió algún error
            string mensaje = mensajeError.Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensaje))
            {
                throw new Exception(mensaje); // O manejarlo como prefieras
            }
        }

    

    }
}