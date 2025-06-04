using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Services
{
    public interface IAccesoBD
    {
        SqlConnection CrearConexion();
        void ProbarConexion();
        int EjecutarComando(string consulta);
        DataTable EjecutarConsultaSQL(string consulta);
        DataTable EjecutarSPconDT(string nombreProcedimiento, params SqlParameter[] parametros);
        DataSet EjecutarSPconDS(string nombreProcedimiento, params SqlParameter[] parametros);

    }//fin Interface
}//fin spcae
