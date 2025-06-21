using Entidades;
using Negocios;
using Negocios.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CUC_Evaluacion_Desemp.Controllers
{
    public class MantenimientosController : Controller
    {
     
        private readonly IMantenimientosService _servicioMantenimientos;

      
        public MantenimientosController(IMantenimientosService servicio)
        {
            _servicioMantenimientos = servicio;
            
        }

        public ActionResult Index()
        {
            return View();
        }


        #region mantenimiento Funcionarios

        public ActionResult ManteniFuncionarios()
        {
            var lista = _servicioMantenimientos.Funcionario.ListarFuncionarios();
            return View(lista);
        }

        public ActionResult CrearFuncionario()
        {
            try
            {
                var puestos = _servicioMantenimientos.Puestos.ListarPuesto();
                var conglomerados = _servicioMantenimientos.Conglomerados.ListarConglomerados();
                var dependencias = _servicioMantenimientos.Dependencias.ListarDependencias();
                var roles = _servicioMantenimientos.Roles.ListarRoles(); 
                var funcionario = new FuncionarioModel();
                var estadosFunc = _servicioMantenimientos.EstadoFuncionarios.ListarEstadosFuncionario();
                var Areas = _servicioMantenimientos.Areas.ListarArea();
                var Jefes = _servicioMantenimientos.Funcionario.ListarJefes();

                FuncionarioViewModel newFuncionarioViewModel = new FuncionarioViewModel
                {
                    Funcionario = funcionario,
                    Puestos = puestos,
                    Conglomerados = conglomerados,
                    Dependencias = dependencias,
                    Roles = roles,
                    EstadosFuncionario = estadosFunc,
                    Areas = Areas,
                    Jefes = Jefes
                };

                return View(newFuncionarioViewModel);

            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al crear: {ex.Message}";
                return View();
            }
        }

        [HttpPost]
        public ActionResult CrearFuncionario(FuncionarioViewModel model, FormCollection collection )
        {
            try
            {
                FuncionarioModel newFuncionario = new FuncionarioModel();
                newFuncionario = model.Funcionario;
                string idFuncionario = newFuncionario.Cedula;
                //Guardamos el nuevo func
                _servicioMantenimientos.Funcionario.CrearFuncionario(newFuncionario);

                var conglomeradosSeleccionados = collection["IdConglomeradosSeleccionados"];

                foreach (var id in conglomeradosSeleccionados)
                {
                    FuncionarioXConglomeradoModel relacion = new FuncionarioXConglomeradoModel
                    {
                        IdFuncionario = idFuncionario,
                        IdConglomerado = Convert.ToInt32(id)
                    };

                    _servicioMantenimientos.FuncionarioXConglomerado.CrearFuncionarioXConglomerado(relacion);
                }


                var areasSeleccionadas = collection["IdAreasSeleccionadas"];

                foreach (var id in areasSeleccionadas)
                {
                    FuncionarioPorAreaModel relacion = new FuncionarioPorAreaModel
                    {
                        cedulaFuncionario = idFuncionario,
                        idArea = Convert.ToInt32(id)
                    };

                    _servicioMantenimientos.FuncionarioPorArea.CrearFuncionarioPorArea(relacion);
                }

                TempData["MensajeExito"] = $"Creado correctamente.";
                return RedirectToAction("ManteniFuncionarios");
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al crear: {ex.Message}";
                return RedirectToAction("ManteniFuncionarios");
            }
        }
        #endregion


        #region Puestos

        public ActionResult ManteniPuesto()
        {
            try
            {
                var puesto = _servicioMantenimientos.Puestos.ListarPuesto();
                return View(puesto);
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al obtener los puestos: {ex.Message}";
                return View(new List<PuestosModel>());
            }
        }

        public ActionResult CreaPuesto()
        {
            return View("CreaPuesto", new PuestosModel());
        }

  
        [HttpPost]
        public ActionResult CreaPuesto(PuestosModel nuevoPuesto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Aquí llamas al método para agregar el puesto a la base de datos
                   _servicioMantenimientos.Puestos.CrearPuesto(nuevoPuesto);

                    // Si la inserción fue exitosa, muestras el mensaje de éxito
                    TempData["MensajeExito"] = $"Puesto {nuevoPuesto.Puesto} creado correctamente.";
                    return RedirectToAction(nameof(ManteniPuesto));
                }
                else
                {
                    // Si el modelo no es válido, vuelve a la vista original con los datos
                    return View("CreaPuesto", nuevoPuesto); // Usa el mismo nombre de vista aquí
                }
            }
            catch (Exception ex)
            {
                // Aquí atrapas el error y lo muestras en la vista usando TempData
                TempData["MensajeError"] = $"Error al crear el puesto: {ex.Message}";
                return View("CreaPuesto", nuevoPuesto); // Usa el mismo nombre de vista aquí
            }
        }

    
        public ActionResult EditaPuesto(int id)
        {
            return View(_servicioMantenimientos.Puestos.ConsultarPuestoID(id));
        }

    
        [HttpPost]
        public ActionResult EditaPuesto(PuestosModel puestoModificado)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _servicioMantenimientos.Puestos.ModificarPuesto(puestoModificado);
                    TempData["MensajeExito"] = $"Puesto {puestoModificado.Puesto} modificado correctamente.";
                    return RedirectToAction(nameof(ManteniPuesto));
                }
                else
                {
                    return View(puestoModificado);
                }
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al actualizar el puesto: {ex.Message}";
                return View(puestoModificado);
            }
        }

        public ActionResult EliminarPuesto(int id)
        {
            try
            {
                var puesto = _servicioMantenimientos.Puestos.ConsultarPuestoID(id);
                if (puesto == null)
                {
                    TempData["MensajeError"] = $"El puesto con ID {id} no fue encontrado.";
                }
                else
                {
                    _servicioMantenimientos.Puestos.EliminarPuesto(id);
                    TempData["MensajeExito"] = $"Puesto {puesto.Puesto} eliminado correctamente.";
                }
                return RedirectToAction(nameof(ManteniPuesto));
            }
            catch
            {
                TempData["MensajeError"] = "No puede borrar este puesto, verifique las relaciones.";
                return RedirectToAction(nameof(ManteniPuesto));
            }
        }


        #endregion

        #region Areas

        public ActionResult ManteniArea()
        {
            try
            {
                var area = _servicioMantenimientos.Areas.ListarArea();
                return View(area);
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al obtener las areas: {ex.Message}";
                return View(new List<AreasModel>());
            }
        }

        public ActionResult CreaArea()
        {
            return View("CreaArea", new AreasModel());
        }


        [HttpPost]
        public ActionResult CreaArea(AreasModel nuevoArea)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    _servicioMantenimientos.Areas.CrearArea(nuevoArea);


                    TempData["MensajeExito"] = $"Area {nuevoArea.NombreArea} creado correctamente.";
                    return RedirectToAction(nameof(ManteniArea));
                }
                else
                {

                    return View("CreaArea", nuevoArea);
                }
            }
            catch (Exception ex)
            {

                TempData["MensajeError"] = $"Error al crear la area: {ex.Message}";
                return View("CreaArea", nuevoArea);
            }
        }


        public ActionResult EditaArea(int id)
        {
            return View(_servicioMantenimientos.Areas.ConsultarAreaID(id));
        }


        [HttpPost]
        public ActionResult EditaArea(AreasModel areaModificado)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _servicioMantenimientos.Areas.ModificarArea(areaModificado);
                    TempData["MensajeExito"] = $" {areaModificado.NombreArea} fue modificado correctamente.";
                    return RedirectToAction(nameof(ManteniArea));
                }
                else
                {
                    return View("EditaArea", areaModificado);
                }
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al actualizar la area: {ex.Message}";
                return View("EditaArea", areaModificado);
            }
        }

        public ActionResult EliminarArea(int id)
        {
            try
            {
                var area = _servicioMantenimientos.Areas.ConsultarAreaID(id);
                if (area == null)
                {
                    TempData["MensajeError"] = $"El area con ID {id} no fue encontrado.";
                }
                else
                {
                    _servicioMantenimientos.Areas.EliminarArea(id);
                    TempData["MensajeExito"] = $"Area {area.NombreArea} eliminado correctamente.";
                }
                return RedirectToAction(nameof(ManteniArea));
            }
            catch
            {
                TempData["MensajeError"] = "No puede borrar esta area, verifique las relaciones.";
                return RedirectToAction(nameof(ManteniArea));
            }
        }

        #endregion
        #region Dependencias
        public ActionResult ManteniDependencias()
        {
            try
            {
                var Dependencias = _servicioMantenimientos.Dependencias.ListarDependencias();
                return View(Dependencias);
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al obtener los puestos: {ex.Message}";
                return View(new List<PuestosModel>());
            }
        }

        [HttpPost]
        public ActionResult CrearDependencia(DependenciasModel newDependencia)
        {
            try
            {
                _servicioMantenimientos.Dependencias.CrearDependencia(newDependencia);
                TempData["MensajeExito"] = $"{newDependencia.Dependencia} creado correctamente.";
                return RedirectToAction(nameof(ManteniDependencias));
            }
            catch (Exception ex)
            {

                TempData["MensajeError"] = $"Error al crear: {ex.Message}";
                return View("CrearDependencia", newDependencia);
            }
        }
        
        [HttpPost]
        public ActionResult EliminarDependencia(int id)
        {
            try
            {
                _servicioMantenimientos.Dependencias.EliminarDependencia(id);
                TempData["MensajeExito"] = $"Eliminado correctamente.";
                return RedirectToAction(nameof(ManteniDependencias));
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "No se puede eliminar porque esta siendo utilizada. "+ex.Message;
                return RedirectToAction(nameof(ManteniDependencias));
            }
        }

        [HttpPost]
        public ActionResult EditarDependencia(DependenciasModel editDependencia)
        {
            try
            {
                _servicioMantenimientos.Dependencias.ModificarDependencia(editDependencia);
                TempData["MensajeExito"] = $"Editado correctamente.";
                return RedirectToAction(nameof(ManteniDependencias));

            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al editar."+ex.Message;
                return RedirectToAction(nameof(ManteniDependencias));
            }
        }
        #endregion

        #region mantenimiento Roles
        public ActionResult ManteniRoles()
        {
            try
            {
                var Roles = _servicioMantenimientos.Roles.ListarRoles();
                return View(Roles);
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al obtener la lista: {ex.Message}";
                return View(new List<PuestosModel>());
            }
        }

        [HttpPost]
        public ActionResult CrearRol(RolesModel newRol)
        {
            try
            {
                _servicioMantenimientos.Roles.CrearRol(newRol);
                TempData["MensajeExito"] = $"{newRol.Rol} creado correctamente.";
                return RedirectToAction(nameof(ManteniRoles));
            }
            catch (Exception ex)
            {

                TempData["MensajeError"] = $"Error al crear: {ex.Message}";
                return View("ManteniRoles", newRol);
            }
        }

        [HttpPost]
        public ActionResult EliminarRol(int id)
        {
            try
            {
                _servicioMantenimientos.Roles.EliminarRol(id);
                TempData["MensajeExito"] = $"Eliminado correctamente.";
                return RedirectToAction(nameof(ManteniRoles));
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "No se puede eliminar porque esta siendo utilizada."+ ex.Message;
                return RedirectToAction(nameof(ManteniRoles));
            }
        }

        [HttpPost]
        public ActionResult EditarRol(RolesModel editRol)
        {
            try
            {
                _servicioMantenimientos.Roles.ActualizarRol(editRol);
                TempData["MensajeExito"] = $"Editado correctamente.";
                return RedirectToAction(nameof(ManteniRoles));

            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al editar. "+ ex.Message;
                return RedirectToAction(nameof(ManteniRoles));
            }
        }
        #endregion

        #region Carreras
        public ActionResult ManteniCarreras()
        {
            try
            {
                var Carreras = _servicioMantenimientos.Carreras.ListarCarreras();
                return View(Carreras);
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al obtener la lista: {ex.Message}";
                return View(new List<CarrerasModel>());
            }
        }

        [HttpPost]
        public ActionResult CrearCarrera(CarrerasModel newCarrera)
        {
            try
            {
                _servicioMantenimientos.Carreras.CrearCarrera(newCarrera);
                TempData["MensajeExito"] = $"{newCarrera.NombreCarrera} creado correctamente.";
                return RedirectToAction(nameof(ManteniCarreras));
            }
            catch (Exception ex)
            {

                TempData["MensajeError"] = $"Error al crear: {ex.Message}";
                return View("ManteniCarreras", newCarrera);
            }
        }

        [HttpPost]
        public ActionResult EliminarCarrera(int id)
        {
            try
            {
                _servicioMantenimientos.Carreras.EliminarCarrera(id);
                TempData["MensajeExito"] = $"Eliminado correctamente.";
                return RedirectToAction(nameof(ManteniCarreras));
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "No se puede eliminar porque esta siendo utilizada." + ex.Message;
                return RedirectToAction(nameof(ManteniCarreras));
            }
        }

        [HttpPost]
        public ActionResult EditarCarrera(CarrerasModel editCarrera)
        {
            try
            {
                _servicioMantenimientos.Carreras.ModificarCarrera(editCarrera);
                TempData["MensajeExito"] = $"Editado correctamente.";
                return RedirectToAction(nameof(ManteniCarreras));

            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al editar. " + ex.Message;
                return RedirectToAction(nameof(ManteniCarreras));
            }
        }
        #endregion
    }//fin class

}

