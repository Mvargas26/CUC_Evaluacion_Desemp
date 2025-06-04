using Datos.Services;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Datos
{
    public class AccesoBD : IAccesoBD
    {
        private readonly string _cadenaConexion;

        public AccesoBD()
        {
            _cadenaConexion = ConfigurationManager.ConnectionStrings["StringDeConexion"]?.ConnectionString
                ?? throw new InvalidOperationException("Cadena de conexión no encontrada.");
        }

        public SqlConnection CrearConexion()
        {
            var conn = new SqlConnection(_cadenaConexion);
            conn.Open();
            return conn;
        }

        public void ProbarConexion()
        {
            using (var conn = CrearConexion())
            {
                // Si entra la conexión es válida
            }
        }

        public int EjecutarComando(string consulta)
        {
            using (var conn = CrearConexion())
            using (var cmd = new SqlCommand(consulta, conn))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        public DataTable EjecutarConsultaSQL(string consulta)
        {
            using (var conn = CrearConexion())
            using (var cmd = new SqlCommand(consulta, conn))
            using (var adaptador = new SqlDataAdapter(cmd))
            {
                var resultado = new DataTable();
                adaptador.Fill(resultado);
                return resultado;
            }
        }

        public DataTable EjecutarSPconDT(string nombreProcedimiento, params SqlParameter[] parametros)
        {
            using (var conn = CrearConexion())
            using (var cmd = new SqlCommand(nombreProcedimiento, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parametros);
                using (var adaptador = new SqlDataAdapter(cmd))
                {
                    var resultado = new DataTable();
                    adaptador.Fill(resultado);
                    return resultado;
                }
            }
        }

        public DataSet EjecutarSPconDS(string nombreProcedimiento, params SqlParameter[] parametros)
        {
            using (var conn = CrearConexion())
            using (var cmd = new SqlCommand(nombreProcedimiento, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parametros);
                using (var adaptador = new SqlDataAdapter(cmd))
                {
                    var resultado = new DataSet();
                    adaptador.Fill(resultado);
                    return resultado;
                }
            }
        }
    }//fin class
}//fin space
