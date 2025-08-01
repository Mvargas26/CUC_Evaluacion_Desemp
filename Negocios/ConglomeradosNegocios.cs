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
    public class ConglomeradosNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public ConglomeradosNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public ConglomeradoModel ConsultarConglomeradoID(int id)
        {

            var parametros = new SqlParameter[]
            {
             new SqlParameter("@Operacion", "R"),
            new SqlParameter("@idConglomerado", id),
            new SqlParameter("@nombreConglomerado", DBNull.Value),
            new SqlParameter("@descripcion", DBNull.Value),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_Conglomerados_CRUD", parametros);
            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }

            if (dt.Rows.Count == 0) return null;

            DataRow row = dt.Rows[0];

            return new ConglomeradoModel
            {
                IdConglomerado = Convert.ToInt32(row["idConglomerado"]),
                NombreConglomerado = row["nombreConglomerado"].ToString(),
                Descripcion = row["Descripcion"].ToString()
            };

        }

        public List<ConglomeradoModel> ListarConglomerados()
        {

            var parametros = new SqlParameter[]
            {
                new SqlParameter("@Operacion", "R"),
            new SqlParameter("@idConglomerado", DBNull.Value),
            new SqlParameter("@nombreConglomerado", DBNull.Value),
            new SqlParameter("@descripcion", DBNull.Value),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_Conglomerados_CRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }


            var lista = new List<ConglomeradoModel>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new ConglomeradoModel
                {
                    IdConglomerado = Convert.ToInt32(row["idConglomerado"]),
                    NombreConglomerado = row["nombreConglomerado"].ToString(),
                    Descripcion = row["Descripcion"].ToString()
                });
            }

            return lista;
        }

        public void CrearConglomerado(ConglomeradoModel Conglomerado)
        {
            var parametros = new SqlParameter[]
            {
              new SqlParameter("@Operacion", "C"),
        new SqlParameter("@idConglomerado", DBNull.Value),
        new SqlParameter("@nombreConglomerado", (object)Conglomerado.NombreConglomerado ?? DBNull.Value),
        new SqlParameter("@descripcion", (object)Conglomerado.Descripcion ?? DBNull.Value),
        new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }

            };

            _accesoBD.EjecutarSPconDT("sp_Conglomerados_CRUD", parametros);
            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }

        public void ModificarConglomerado(ConglomeradoModel Conglomerado)
        {
            var parametros = new SqlParameter[]
            {
             new SqlParameter("@Operacion", "U"),
           new SqlParameter("@idConglomerado", (object)Conglomerado.IdConglomerado ?? DBNull.Value),

        new SqlParameter("@nombreConglomerado", (object)Conglomerado.NombreConglomerado ?? DBNull.Value),
        new SqlParameter("@descripcion", (object)Conglomerado.Descripcion ?? DBNull.Value),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_Conglomerados_CRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }

        public void EliminarConglomerado(int id)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "D"),
            new SqlParameter("@idConglomerado", id),
            new SqlParameter("@MensajeError", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output }
            };

            _accesoBD.EjecutarSPconDT("sp_Conglomerados_CRUD", parametros);

            string mensajeError = parametros.Last().Value?.ToString();
            if (!string.IsNullOrWhiteSpace(mensajeError))
            {
                throw new Exception("Error SP: " + mensajeError);
            }
        }
        public List<PesosConglomeradoModel> ConsultarPesosXConglomerado(int id)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                new SqlParameter("@conglomeradoID", id)
                };

                DataTable dt = _accesoBD.EjecutarSPconDT("SP_PesosXConglomerado", parametros);
                List<PesosConglomeradoModel> lista = new List<PesosConglomeradoModel>();

                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(new PesosConglomeradoModel
                    {
                        IdPesoXConglomerado = Convert.ToInt32(row["idPesoXConglomerado"]),
                        IdConglomerado = Convert.ToInt32(row["idConglomerado"]),
                        IdTipoObjetivo = row["idTipoObjetivo"] as int?,
                        IdTipoCompetencia = row["idTipoCompetencia"] as int?,
                        Porcentaje = Convert.ToDecimal(row["Porcentaje"])
                    });
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Fallo en PesosConglomerado Negocios " + ex.Message);
            }
        }

        public List<FuncionarioXConglomeradoModel> ConsultarConglomeradoXFuncionario(string idFuncionario)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                new SqlParameter("@idFuncionario", idFuncionario)
                };

                DataTable dt = _accesoBD.EjecutarSPconDT("SP_ConsultarConglomeradoXFuncionario", parametros);
                List<FuncionarioXConglomeradoModel> lista = new List<FuncionarioXConglomeradoModel>();

                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(new FuncionarioXConglomeradoModel
                    {
                        IdFuncXConglo = Convert.ToInt32(row["idFuncXConglo"]),
                        IdFuncionario = row["idFuncionario"].ToString(),
                        IdConglomerado = Convert.ToInt32(row["idConglomerado"])
                    });
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Fallo en ConsultarConglomeradoXFuncionario " + ex.Message);
            }
        }
    }//fin class
}//fin space
