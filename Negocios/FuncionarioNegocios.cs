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
    public class FuncionarioNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public FuncionarioNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public string GenerarCodigoSeguridad()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        public void EstablecerCodigoSeguridad(string cedula, string codigoSeguridad)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "UPDATE"),
            new SqlParameter("@Cedula", cedula),
            new SqlParameter("@CodigoSeguridad", codigoSeguridad),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_CrudFuncionarios", parametros);
        }

        public FuncionarioModel ConsultarFuncionarioID(string cedula)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "CONSULTAID"),
            new SqlParameter("@Cedula", cedula),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_CrudFuncionarios", parametros);

            if (dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];

            return new FuncionarioModel
            {
                Cedula = row["cedula"].ToString(),
                Nombre = row["nombre"].ToString(),
                Apellido1 = row["apellido1"].ToString(),
                Apellido2 = row["apellido2"].ToString(),
                Correo = row["correo"].ToString(),
                Password = row["password"].ToString(),
                Departamento = row["Departamento"].ToString(),
                Rol = row["Rol"].ToString(),
                Puesto = row["Puesto"].ToString(),
                Estado = row["Estado"].ToString(),
                CodigoSeguridad = row["CodigoSeguridad"]?.ToString(),
                IdDepartamento = Convert.ToInt32(row["idDepartamento"])
            };
        }

        public List<FuncionarioModel> ListarFuncionarios()
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "SELECT"),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_CrudFuncionarios", parametros);

            var lista = new List<FuncionarioModel>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new FuncionarioModel
                {
                    Cedula = row["cedula"].ToString(),
                    Nombre = row["nombre"].ToString(),
                    Apellido1 = row["apellido1"].ToString(),
                    Apellido2 = row["apellido2"].ToString(),
                    Correo = row["correo"].ToString(),
                    Password = row["password"].ToString(),
                    Departamento = row["Departamento"].ToString(),
                    Rol = row["Rol"].ToString(),
                    Puesto = row["Puesto"].ToString(),
                    Estado = row["Estado"].ToString()
                });
            }

            return lista;
        }

        public bool CrearFuncionario(FuncionarioModel funcionario)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "INSERT"),
            new SqlParameter("@Cedula", funcionario.Cedula),
            new SqlParameter("@Nombre", funcionario.Nombre),
            new SqlParameter("@Apellido1", funcionario.Apellido1),
            new SqlParameter("@Apellido2", funcionario.Apellido2),
            new SqlParameter("@Correo", funcionario.Correo),
            new SqlParameter("@Password", funcionario.Password),
            new SqlParameter("@Departamento", funcionario.Departamento),
            new SqlParameter("@Rol", funcionario.Rol),
            new SqlParameter("@Puesto", funcionario.Puesto),
            new SqlParameter("@Estado", funcionario.Estado),
            new SqlParameter("@CodigoSeguridad", funcionario.CodigoSeguridad),
            new SqlParameter("@IdDepartamento", funcionario.IdDepartamento),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_CrudFuncionarios", parametros);

            return true;
        }

        public bool ModificarFuncionario(FuncionarioModel funcionario)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "UPDATE"),
            new SqlParameter("@Cedula", funcionario.Cedula),
            new SqlParameter("@Nombre", funcionario.Nombre),
            new SqlParameter("@Apellido1", funcionario.Apellido1),
            new SqlParameter("@Apellido2", funcionario.Apellido2),
            new SqlParameter("@Correo", funcionario.Correo),
            new SqlParameter("@Password", funcionario.Password),
            new SqlParameter("@Departamento", funcionario.Departamento),
            new SqlParameter("@Rol", funcionario.Rol),
            new SqlParameter("@Puesto", funcionario.Puesto),
            new SqlParameter("@Estado", funcionario.Estado),
            new SqlParameter("@CodigoSeguridad", funcionario.CodigoSeguridad),
            new SqlParameter("@IdDepartamento", funcionario.IdDepartamento),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_CrudFuncionarios", parametros);

            return true;
        }

        public bool EliminarFuncionario(string cedula)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "DELETE"),
            new SqlParameter("@Cedula", cedula),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_CrudFuncionarios", parametros);

            return true;
        }

        public FuncionarioModel Login(string cedula, string password)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "LOGIN"),
            new SqlParameter("@Cedula", cedula),
            new SqlParameter("@Password", password),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_CrudFuncionarios", parametros);

            if (dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];

            return new FuncionarioModel
            {
                Cedula = row["cedula"].ToString(),
                Nombre = row["nombre"].ToString(),
                Apellido1 = row["apellido1"].ToString(),
                Apellido2 = row["apellido2"].ToString(),
                Correo = row["correo"].ToString(),
                Departamento = row["Departamento"].ToString(),
                Rol = row["Rol"].ToString(),
                Puesto = row["Puesto"].ToString(),
                Estado = row["Estado"].ToString(),
                IdDepartamento = Convert.ToInt32(row["idDepartamento"])
            };
        }
    }

}//fin space
