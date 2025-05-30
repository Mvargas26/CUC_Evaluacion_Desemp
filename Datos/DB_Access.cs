using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Datos
{
    public class AccesoBD
    {
        private readonly string _cadenaConexion;
        private readonly ILogger<AccesoBD> _registro;

        public AccesoBD(string cadenaConexion, ILogger<AccesoBD> registro)
        {
            _cadenaConexion = string.IsNullOrWhiteSpace(cadenaConexion)
                ? throw new ArgumentNullException(nameof(cadenaConexion), "Cadena de conexión no válida.")
                : cadenaConexion;

            _registro = registro ?? throw new ArgumentNullException(nameof(registro));
        }

        public SqlConnection CrearConexion()
        {
            try
            {
                var conexion = new SqlConnection(_cadenaConexion);
                conexion.Open();
                return conexion;
            }
            catch (SqlException ex)
            {
                _registro.LogError(ex, "Error SQL al abrir la conexión");
                throw;
            }
            catch (Exception ex)
            {
                _registro.LogError(ex, "Error inesperado al abrir la conexión");
                throw;
            }
        }

        public void ProbarConexion()
        {
            try
            {
                using (var conexion = CrearConexion())
                {
                }
            }
            catch (Exception ex)
            {
                _registro.LogError(ex, "Fallo en prueba de conexión");
                throw;
            }
        }

        public DataTable EjecutarConsultaSQL(string consulta)
        {
            try
            {
                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(consulta, conexion))
                using (var adaptador = new SqlDataAdapter(comando))
                {
                    var resultado = new DataTable();
                    adaptador.Fill(resultado);
                    return resultado;
                }
            }
            catch (SqlException ex)
            {
                _registro.LogError(ex, "Error SQL en EjecutarConsulta");
                throw;
            }
            catch (Exception ex)
            {
                _registro.LogError(ex, "Error inesperado en EjecutarConsulta");
                throw;
            }
        }

        public DataTable EjecutarSPconDT(string nombreProcedimiento, params SqlParameter[] parametros)
        {
            try
            {
                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(nombreProcedimiento, conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.Parameters.AddRange(parametros);

                    using (var adaptador = new SqlDataAdapter(comando))
                    {
                        var resultado = new DataTable();
                        adaptador.Fill(resultado);
                        return resultado;
                    }
                }
            }
            catch (SqlException ex)
            {
                _registro.LogError(ex, "Error SQL en EjecutarProcedimientoConsulta");
                throw;
            }
            catch (Exception ex)
            {
                _registro.LogError(ex, "Error inesperado en EjecutarProcedimientoConsulta");
                throw;
            }
        }

        public DataSet EjecutarSPconDS(string nombreProcedimiento, params SqlParameter[] parametros)
        {
            try
            {
                using (var conexion = CrearConexion())
                using (var comando = new SqlCommand(nombreProcedimiento, conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.Parameters.AddRange(parametros);

                    using (var adaptador = new SqlDataAdapter(comando))
                    {
                        var resultado = new DataSet();
                        adaptador.Fill(resultado);
                        return resultado;
                    }
                }
            }
            catch (SqlException ex)
            {
                _registro.LogError(ex, "Error SQL en EjecutarProcedimientoConjuntoDatos");
                throw;
            }
            catch (Exception ex)
            {
                _registro.LogError(ex, "Error inesperado en EjecutarProcedimientoConjuntoDatos");
                throw;
            }
        }
    }//fin class
}//fin space

