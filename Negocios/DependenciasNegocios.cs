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
    public class DependenciasNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public DependenciasNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public DependenciasModel ConsultarDependenciaID(int id)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Operacion ", "S"),
                new SqlParameter("@idDependencia ", id),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }

            };

            DataTable dt = _accesoBD.EjecutarSPconDT("idDependencia", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new DependenciasModel
            {
                IdDependencia = Convert.ToInt32(row["idDepartamento"]),
                Dependencia = row["Departamento"].ToString()
            };
        }

        public List<DependenciasModel> ListarDependencias()
        {
            try
            {
                var parametros = new SqlParameter[]
              {
                new SqlParameter("@Operacion", "R"),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }

              };

                DataTable dt = _accesoBD.EjecutarSPconDT("sp_DependenciasCRUD", parametros);

                string mensajeError = parametros.Last().Value?.ToString();
                if (!string.IsNullOrWhiteSpace(mensajeError))
                {
                    throw new Exception("Error SP: " + mensajeError);
                }

                List<DependenciasModel> lista = new List<DependenciasModel>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(new DependenciasModel
                    {
                        IdDependencia = Convert.ToInt32(row["idDependencia"]),
                        Dependencia = row["Dependencia"].ToString()
                    });
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar los departamentos: " + ex.Message);
            }
        }

        public void CrearDependencia(DependenciasModel departamento)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@Operacion", "C"),
                    new SqlParameter("@Dependencia", departamento.Dependencia),
                    new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }

                };

                _accesoBD.EjecutarSPconDT("sp_DependenciasCRUD", parametros);

                string mensajeError = parametros.Last().Value?.ToString();
                if (!string.IsNullOrWhiteSpace(mensajeError))
                {
                    throw new Exception("Error SP: " + mensajeError);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear : " + ex.Message);
            }
        }

        public void ModificarDependencia(DependenciasModel departamento)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@Operacion ", "U"),
                    new SqlParameter("@idDependencia ", departamento.IdDependencia),
                    new SqlParameter("@Dependencia ", departamento.Dependencia),
                    new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }

                };

                _accesoBD.EjecutarSPconDT("sp_DependenciasCRUD ", parametros);

                string mensajeError = parametros.Last().Value?.ToString();
                if (!string.IsNullOrWhiteSpace(mensajeError))
                {
                    throw new Exception("Error SP: " + mensajeError);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar: " + ex.Message);
            }
        }

        public void EliminarDependencia(int id)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@Operacion", "D"),
                    new SqlParameter("@idDependencia", id),
                    new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
                };

                _accesoBD.EjecutarSPconDT("sp_DependenciasCRUD ", parametros);

                string mensajeError = parametros.Last().Value?.ToString();
                if (!string.IsNullOrWhiteSpace(mensajeError))
                {
                    throw new Exception("Error SP: " + mensajeError);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar: " + ex.Message);
            }
        }
    }//fin class
}//fin space
