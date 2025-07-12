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
    public class ComportamientoPorNivelNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public ComportamientoPorNivelNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public List<ComportamientoPorNivel> ListarComportamientoPorNivel(int idCompetencia)
        {
            try
            {
                var parametros = new SqlParameter[]
                  {
                        new SqlParameter("@idCompetencia", idCompetencia),
                        new SqlParameter("@idComportamiento", DBNull.Value),
                        new SqlParameter("@idNivel", DBNull.Value),
                        new SqlParameter("@Descripcion", DBNull.Value),
                        new SqlParameter("@Accion", "CONSULTAR"),
                        new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
                  };

                DataTable dt = _accesoBD.EjecutarSPconDT("sp_ComportamientoPorNivel_CRUD", parametros);

                string mensajeError = parametros.Last().Value?.ToString();
                if (!string.IsNullOrWhiteSpace(mensajeError) && mensajeError != "OK")
                {
                    throw new Exception("Error SP: " + mensajeError);
                }

                var lista = new List<ComportamientoPorNivel>();
                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(new ComportamientoPorNivel
                    {
                        idComportamiento = Convert.ToInt32(row["idComportamiento"]),
                        idNivel = Convert.ToInt32(row["idNivel"]),
                        descripcion = row["Descripcion"] != DBNull.Value ? row["Descripcion"].ToString() : null
                    });
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error SP: " + ex.Message);
            }
        }

        public void InsertarComportamientoPorNivel(ComportamientoPorNivel modelo)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@idComportamiento", modelo.idComportamiento),
                    new SqlParameter("@idCompetencia", modelo.idCompetencia),
                    new SqlParameter("@idNivel", modelo.idNivel),
                    new SqlParameter("@Descripcion", (object)modelo.descripcion ?? DBNull.Value),
                    new SqlParameter("@Accion", "INSERTAR"),
                    new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
                };

            
                _accesoBD.EjecutarSPconDT("sp_ComportamientoPorNivel_CRUD", parametros);

                string mensajeError = parametros.Last().Value?.ToString();
                if (!string.IsNullOrWhiteSpace(mensajeError) && mensajeError != "OK")
                {
                    throw new Exception("Error SP: " + mensajeError);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        public void ActualizarComportamientoPorNivel(ComportamientoPorNivel modelo)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@idComportamiento", modelo.idComportamiento),
                    new SqlParameter("@idCompetencia", modelo.idCompetencia),
                    new SqlParameter("@idNivel", modelo.idNivel),
                    new SqlParameter("@Descripcion", (object)modelo.descripcion ?? DBNull.Value),
                    new SqlParameter("@Accion", "ACTUALIZAR"),
                    new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
                };
            
                _accesoBD.EjecutarSPconDT("sp_ComportamientoPorNivel_CRUD", parametros);

                string mensajeError = parametros.Last().Value?.ToString();
                if (!string.IsNullOrWhiteSpace(mensajeError) && mensajeError != "OK")
                {
                    throw new Exception("Error SP: " + mensajeError);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        public void EliminarComportamientoPorNivel(int idComportamiento, int idNivel,int idCompetencia)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@idComportamiento", idComportamiento),
                    new SqlParameter("@idCompetencia", idCompetencia),
                    new SqlParameter("@idNivel", idNivel),
                    new SqlParameter("@Descripcion", DBNull.Value),
                    new SqlParameter("@Accion", "ELIMINAR"),
                    new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
                };

           
                _accesoBD.EjecutarSPconDT("sp_ComportamientoPorNivel_CRUD", parametros);

                string mensajeError = parametros.Last().Value?.ToString();
                if (!string.IsNullOrWhiteSpace(mensajeError) && mensajeError != "OK")
                {
                    throw new Exception("Error SP: " + mensajeError);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

    }//fin class
}
