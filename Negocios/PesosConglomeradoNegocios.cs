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
    public class PesosConglomeradoNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public PesosConglomeradoNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

         public List<PesosConglomeradoModel> ConsultarPesosXConglomerado(int id)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@Operacion", "PORIDCONGLOMERADO"),
                    new SqlParameter("@idPesoXConglomerado", DBNull.Value),
                    new SqlParameter("@idConglomerado", id),
                    new SqlParameter("@idTipoObjetivo", DBNull.Value),
                    new SqlParameter("@idTipoCompetencia", DBNull.Value),
                    new SqlParameter("@Porcentaje", DBNull.Value),
                    new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
                };

                DataTable dt = _accesoBD.EjecutarSPconDT("SP_PesoXConglomeradoCRUD", parametros);

                string mensajeError = parametros.Last().Value?.ToString();
                if (!string.IsNullOrWhiteSpace(mensajeError))
                {
                    throw new Exception("Error SP: " + mensajeError);
                }

                List<PesosConglomeradoModel> lista = new List<PesosConglomeradoModel>();

                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(new PesosConglomeradoModel
                    {
                        IdPesoXConglomerado = Convert.ToInt32(row["idPesoXConglomerado"]),
                        IdConglomerado = Convert.ToInt32(row["idConglomerado"]),
                        IdTipoObjetivo = row["idTipoObjetivo"] != DBNull.Value ? Convert.ToInt32(row["idTipoObjetivo"]) : (int?)null,
                        IdTipoCompetencia = row["idTipoCompetencia"] != DBNull.Value ? Convert.ToInt32(row["idTipoCompetencia"]) : (int?)null,
                        Porcentaje = Convert.ToDecimal(row["Porcentaje"])
                    });
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Fallo en PesosConglomerado Negocios: " + ex.Message);
            }
        }//ConsultarPesosXConglomerado

        public void CrearPesoXConglomerado(PesosConglomeradoModel nuevoPeso)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Operacion", "INSERT"),
                new SqlParameter("@idPesoXConglomerado", DBNull.Value),
                new SqlParameter("@idConglomerado", nuevoPeso.IdConglomerado),
                new SqlParameter("@idTipoObjetivo", (object)(nuevoPeso.IdTipoObjetivo ?? (object)DBNull.Value)),
                new SqlParameter("@idTipoCompetencia", (object)(nuevoPeso.IdTipoCompetencia ?? (object)DBNull.Value)),
                new SqlParameter("@Porcentaje", nuevoPeso.Porcentaje),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("SP_PesoXConglomeradoCRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }//CrearPesoXConglomerado

        public void ModificarPesoXConglomerado(PesosConglomeradoModel pesoModificado)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Operacion", "UPDATE"),
                new SqlParameter("@idPesoXConglomerado", pesoModificado.IdPesoXConglomerado),
                new SqlParameter("@idConglomerado", pesoModificado.IdConglomerado),
        new SqlParameter("@idTipoObjetivo", (object)(pesoModificado.IdTipoObjetivo ?? (object)DBNull.Value)),
        new SqlParameter("@idTipoCompetencia", (object)(pesoModificado.IdTipoCompetencia ?? (object)DBNull.Value)),
                new SqlParameter("@Porcentaje", pesoModificado.Porcentaje),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("SP_PesoXConglomeradoCRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }//ModificarPesoXConglomerado

        public void EliminarPesoXConglomerado(int idPesoXConglomerado)
        {
            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Operacion", "DELETE"),
                new SqlParameter("@idPesoXConglomerado", idPesoXConglomerado),
                new SqlParameter("@idConglomerado", DBNull.Value),
                new SqlParameter("@idTipoObjetivo", DBNull.Value),
                new SqlParameter("@idTipoCompetencia", DBNull.Value),
                new SqlParameter("@Porcentaje", DBNull.Value),
                new SqlParameter("@MensajeError", SqlDbType.VarChar, 500) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("SP_PesoXConglomeradoCRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }

    }
}//fin space
