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

        public List<PesosConglomeradoModel> ObtenerTodos()
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "SELECT")
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("SP_PesoXConglomeradoCRUD", parametros);
            var lista = new List<PesosConglomeradoModel>();

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

        public PesosConglomeradoModel ConsultarPorID(int id)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "SELECT"),
            new SqlParameter("@idPesoXConglomerado", id)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("SP_PesoXConglomeradoCRUD", parametros);
            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];
            return new PesosConglomeradoModel
            {
                IdPesoXConglomerado = Convert.ToInt32(row["idPesoXConglomerado"]),
                IdConglomerado = Convert.ToInt32(row["idConglomerado"]),
                IdTipoObjetivo = row["idTipoObjetivo"] != DBNull.Value ? Convert.ToInt32(row["idTipoObjetivo"]) : (int?)null,
                IdTipoCompetencia = row["idTipoCompetencia"] != DBNull.Value ? Convert.ToInt32(row["idTipoCompetencia"]) : (int?)null,
                Porcentaje = Convert.ToDecimal(row["Porcentaje"])
            };
        }

        public List<PesosConglomeradoModel> ListarPorIdConglomerado(int idConglomerado)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "SELECT"),
            new SqlParameter("@idConglomerado", idConglomerado)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("SP_PesoXConglomeradoCRUD", parametros);
            var lista = new List<PesosConglomeradoModel>();

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
    }
}//fin space
