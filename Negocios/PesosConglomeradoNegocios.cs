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

        public void Crear(PesosConglomeradoModel model)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "INSERT"),
            new SqlParameter("@idConglomerado", model.IdConglomerado),
            new SqlParameter("@idTipoObjetivo", (object)model.IdTipoObjetivo ?? DBNull.Value),
            new SqlParameter("@idTipoCompetencia", (object)model.IdTipoCompetencia ?? DBNull.Value),
            new SqlParameter("@Porcentaje", model.Porcentaje)
            };

            _accesoBD.EjecutarSPconDT("SP_PesoXConglomeradoCRUD", parametros);
        }

        public void Modificar(PesosConglomeradoModel model)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "UPDATE"),
            new SqlParameter("@idPesoXConglomerado", model.IdPesoXConglomerado),
            new SqlParameter("@idConglomerado", model.IdConglomerado),
            new SqlParameter("@idTipoObjetivo", (object)model.IdTipoObjetivo ?? DBNull.Value),
            new SqlParameter("@idTipoCompetencia", (object)model.IdTipoCompetencia ?? DBNull.Value),
            new SqlParameter("@Porcentaje", model.Porcentaje)
            };

            _accesoBD.EjecutarSPconDT("SP_PesoXConglomeradoCRUD", parametros);
        }

        public void Eliminar(int id)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "DELETE"),
            new SqlParameter("@idPesoXConglomerado", id)
            };

            _accesoBD.EjecutarSPconDT("SP_PesoXConglomeradoCRUD", parametros);
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


    }
}//fin space
