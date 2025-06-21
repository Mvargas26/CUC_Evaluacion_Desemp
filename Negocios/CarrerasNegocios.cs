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
    public class CarrerasNegocios
    {
        private readonly IAccesoBD _accesoBD;

        public CarrerasNegocios(IAccesoBD accesoBD)
        {
            _accesoBD = accesoBD;
        }

        public List<CarrerasModel> ListarCarreras()
        {
            List<CarrerasModel> lista = new List<CarrerasModel>();

            try
            {
                var parametros = new SqlParameter[]
                {
            new SqlParameter("@Accion", "READ"),
            new SqlParameter("@idCarrera", DBNull.Value)
                };

                DataTable dt = _accesoBD.EjecutarSPconDT("sp_CarrerasCRUD", parametros);

                foreach (DataRow row in dt.Rows)
                {
                    lista.Add(new CarrerasModel
                    {
                        idCarrera = Convert.ToInt32(row["idCarrera"]),
                        NombreCarrera = row["nombreCarrera"].ToString(),
                        Descripcion = row["descripcion"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar las carreras: " + ex.Message);
            }

            return lista;
        }

        public CarrerasModel ConsultarCarreraID(int idCarrera)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
            new SqlParameter("@Accion", "READ"),
            new SqlParameter("@idCarrera", idCarrera)
                };

                DataTable dt = _accesoBD.EjecutarSPconDT("sp_CarrerasCRUD", parametros);

                if (dt.Rows.Count == 0)
                    return null;

                DataRow row = dt.Rows[0];

                return new CarrerasModel
                {
                    idCarrera = Convert.ToInt32(row["idCarrera"]),
                    NombreCarrera = row["nombreCarrera"].ToString(),
                    Descripcion = row["descripcion"].ToString()
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar la carrera por ID: " + ex.Message);
            }
        }

        public void CrearCarrera(CarrerasModel carrera)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
            new SqlParameter("@Accion", "CREATE"),
            new SqlParameter("@nombreCarrera", carrera.NombreCarrera),
            new SqlParameter("@descripcion", carrera.Descripcion)
                };

                _accesoBD.EjecutarSPconDT("sp_CarrerasCRUD", parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear la carrera: " + ex.Message);
            }
        }

        public void ModificarCarrera(CarrerasModel carrera)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@Accion", "UPDATE"),
                    new SqlParameter("@idCarrera", carrera.idCarrera),
                    new SqlParameter("@nombreCarrera", carrera.NombreCarrera),
                    new SqlParameter("@descripcion", carrera.Descripcion)
                };

                _accesoBD.EjecutarSPconDT("sp_CarrerasCRUD", parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar la carrera: " + ex.Message);
            }
        }
        public void EliminarCarrera(int id)
        {
            try
            {
                var parametros = new SqlParameter[]
                {
                    new SqlParameter("@Accion", "DELETE"),
                    new SqlParameter("@idCarrera", id)
                };

                _accesoBD.EjecutarSPconDT("sp_CarrerasCRUD", parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar : " + ex.Message);
            }
        }

    }//fin class
}
