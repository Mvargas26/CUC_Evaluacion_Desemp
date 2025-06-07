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
    public class FuncionarioXConglomeradoNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public FuncionarioXConglomeradoNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public FuncionarioXConglomeradoModel ConsultarFuncionarioXConglomerado_ID(int idFuncXConglo)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "CONSULTAID"),
            new SqlParameter("@IdFuncXConglo", idFuncXConglo)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_CrudFuncionarioXConglomerado", parametros);

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            return new FuncionarioXConglomeradoModel
            {
                IdFuncXConglo = Convert.ToInt32(row["IdFuncXConglo"]),
                IdFuncionario = row["IdFuncionario"].ToString(),
                IdConglomerado = Convert.ToInt32(row["IdConglomerado"])
            };
        }

        public List<FuncionarioXConglomeradoModel> ListarFuncionarioXConglomerado()
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "SELECT")
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_CrudFuncionarioXConglomerado", parametros);

            var lista = new List<FuncionarioXConglomeradoModel>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new FuncionarioXConglomeradoModel
                {
                    IdFuncXConglo = Convert.ToInt32(row["IdFuncXConglo"]),
                    IdFuncionario = row["IdFuncionario"].ToString(),
                    IdConglomerado = Convert.ToInt32(row["IdConglomerado"])
                });
            }

            return lista;
        }

        public List<FuncionarioXConglomeradoModel> ListarFuncionarioXConglomeradoxIDfuncionario(string idFuncionario)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "CONSULTAIDFUNCIONARIO"),
            new SqlParameter("@IdFuncionario", idFuncionario)
            };

            DataTable dt = _accesoBD.EjecutarSPconDT("sp_CrudFuncionarioXConglomerado", parametros);

            var lista = new List<FuncionarioXConglomeradoModel>();
            foreach (DataRow row in dt.Rows)
            {
                lista.Add(new FuncionarioXConglomeradoModel
                {
                    IdFuncXConglo = Convert.ToInt32(row["IdFuncXConglo"]),
                    IdFuncionario = row["IdFuncionario"].ToString(),
                    IdConglomerado = Convert.ToInt32(row["IdConglomerado"])
                });
            }

            return lista;
        }

        public void CrearFuncionarioXConglomerado(FuncionarioXConglomeradoModel nuevoFuncionarioXConglo)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "INSERT"),
            new SqlParameter("@idFuncionario", nuevoFuncionarioXConglo.IdFuncionario),
            new SqlParameter("@idConglomerado", nuevoFuncionarioXConglo.IdConglomerado)
            };

            _accesoBD.EjecutarSPconDT("sp_FuncionarioXConglomerado_CRUD", parametros);
        }

        public void ModificarFuncionarioXConglomerado(FuncionarioXConglomeradoModel funcionarioXConglo)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "UPDATE"),
            new SqlParameter("@IdFuncXConglo", funcionarioXConglo.IdFuncXConglo),
            new SqlParameter("@IdFuncionario", funcionarioXConglo.IdFuncionario),
            new SqlParameter("@IdConglomerado", funcionarioXConglo.IdConglomerado)
            };

            _accesoBD.EjecutarSPconDT("[adm].[sp_FuncionarioXConglomerado_CRUD]", parametros);
        }

        public void EliminarPorFuncionario(string idFuncionario)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Operacion", "DELETE_POR_FUNCIONARIO"),
            new SqlParameter("@idFuncionario", idFuncionario)
            };

            _accesoBD.EjecutarSPconDT("[adm].[sp_FuncionarioXConglomerado_CRUD]", parametros);
        }

        public void EliminarFuncionarioXConglomerado(int idFuncXConglo)
        {
            var parametros = new SqlParameter[]
            {
            new SqlParameter("@Accion", "DELETE"),
            new SqlParameter("@IdFuncXConglo", idFuncXConglo)
            };

            _accesoBD.EjecutarSPconDT("sp_CrudFuncionarioXConglomerado", parametros);
        }

    }//fin class
}//fin class
