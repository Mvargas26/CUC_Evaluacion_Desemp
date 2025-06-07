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
            try
            {
                var parametros = new SqlParameter[]
                {
                new SqlParameter("@Operacion", "SELECT"),
                new SqlParameter("@idConglomerado", id)
                };

                DataTable dt = _accesoBD.EjecutarSPconDT("sp_Conglomerados_CRUD", parametros);

                if (dt.Rows.Count == 0) return null;

                DataRow row = dt.Rows[0];

                return new ConglomeradoModel
                {
                    IdConglomerado = Convert.ToInt32(row["idConglomerado"]),
                    NombreConglomerado = row["nombreConglomerado"].ToString(),
                    Descripcion = row["Descripcion"].ToString()
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Fallo en Conglomerado Negocios " + ex.Message);
            }
        }

        public List<ConglomeradoModel> ListarConglomerados()
        {
            List<ConglomeradoModel> lista = new List<ConglomeradoModel>();

            try
            {
                var parametros = new SqlParameter[]
                {
                new SqlParameter("@Operacion", "SELECT"),
                new SqlParameter("@idConglomerado", DBNull.Value)
                };

                DataTable dt = _accesoBD.EjecutarSPconDT("sp_Conglomerados_CRUD", parametros);

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
            catch (Exception ex)
            {
                throw new Exception("Fallo en Conglomerado Negocios " + ex.Message);
            }
        }

        public void CrearConglomerado(ConglomeradoModel conglomerado)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                new SqlParameter("@Operacion", "INSERT"),
                new SqlParameter("@nombreConglomerado", conglomerado.NombreConglomerado),
                new SqlParameter("@Descripcion", conglomerado.Descripcion)
                };

                _accesoBD.EjecutarSPconDT("sp_Conglomerados_CRUD", parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Fallo en Conglomerado Negocios " + ex.Message);
            }
        }

        public void ModificarConglomerado(ConglomeradoModel conglomerado)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                new SqlParameter("@Operacion", "UPDATE"),
                new SqlParameter("@idConglomerado", conglomerado.IdConglomerado),
                new SqlParameter("@nombreConglomerado", conglomerado.NombreConglomerado),
                new SqlParameter("@Descripcion", conglomerado.Descripcion)
                };

                _accesoBD.EjecutarSPconDT("sp_Conglomerados_CRUD", parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Fallo en Conglomerado Negocios " + ex.Message);
            }
        }

        public void EliminarConglomerado(int id)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                new SqlParameter("@Operacion", "D"),
                new SqlParameter("@idConglomerado", id)
                };

                _accesoBD.EjecutarSPconDT("sp_Conglomerados_CRUD", parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Fallo en Conglomerado Negocios " + ex.Message);
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
}//fn space
