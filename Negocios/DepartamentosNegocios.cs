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
    public class DepartamentosNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public DepartamentosNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public DepartamentoModel ConsultarDepartamentoID(int id)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "CONSULTAID"),
            new SqlParameter("@idDepartamento", id)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_CrudDepartamentos", parametros);

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new DepartamentoModel
            {
                IdDepartamento = Convert.ToInt32(row["idDepartamento"]),
                Departamento = row["Departamento"].ToString()
            };
        }

        public List<DepartamentoModel> ListarDepartamentos()
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                new SqlParameter("@Accion", "SELECT")
                };

                DataTable dt = _accesoBD.EjecutarSPconDT("sp_CrudDepartamentos", parametros);
                List<DepartamentoModel> lista = new List<DepartamentoModel>();

                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(new DepartamentoModel
                    {
                        IdDepartamento = Convert.ToInt32(row["idDepartamento"]),
                        Departamento = row["Departamento"].ToString()
                    });
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar los departamentos: " + ex.Message);
            }
        }

        public void CrearDepartamento(DepartamentoModel departamento)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                new SqlParameter("@Accion", "INSERT"),
                new SqlParameter("@Departamento", departamento.Departamento)
                };

                _accesoBD.EjecutarSPconDT("sp_CrudDepartamentos", parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el departamento: " + ex.Message);
            }
        }

        public void ModificarDepartamento(DepartamentoModel departamento)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                new SqlParameter("@Accion", "UPDATE"),
                new SqlParameter("@idDepartamento", departamento.IdDepartamento),
                new SqlParameter("@Departamento", departamento.Departamento)
                };

                _accesoBD.EjecutarSPconDT("sp_CrudDepartamentos", parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar el departamento: " + ex.Message);
            }
        }

        public void EliminarDepartamento(int id)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                new SqlParameter("@Accion", "DELETE"),
                new SqlParameter("@idDepartamento", id)
                };

                _accesoBD.EjecutarSPconDT("sp_CrudDepartamentos", parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el departamento: " + ex.Message);
            }
        }
    }//fin class
}//fin space
