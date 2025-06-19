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
    public class AreasNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public AreasNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public List<AreasModel> ListarAreas()
        {
            List<AreasModel> lista = new List<AreasModel>();

            try
            {
                var parametros = new SqlParameter[]
                {
                new SqlParameter("@Operacion", "SELECT")
                };

                DataTable dt = _accesoBD.EjecutarSPconDT("sp_AreasCRUD", parametros);

                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(new AreasModel
                    {
                        idArea = Convert.ToInt32(row["idArea"]),
                        nombreArea = row["NombreArea"].ToString(),
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

    }//fin class
}//fin space
