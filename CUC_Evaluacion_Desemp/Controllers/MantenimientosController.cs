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
                var departamentos = _servicioMantenimientos.Departamentos.ListarDepartamentos();
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
                    Departamentos = departamentos,
                    Roles = roles,
                    EstadosFuncionario = estadosFunc,
                    Areas = Areas,
                    Jefes = Jefes
                };


                return View(newFuncionarioViewModel);

            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al cargar la vista: {ex.Message}";
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
             //   _servicioMantenimientos.Funcionario.CrearFuncionario(newFuncionario);

                var conglomeradosSeleccionados = collection["IdConglomeradosSeleccionados"];

                foreach (var id in conglomeradosSeleccionados)
                {
                    FuncionarioXConglomeradoModel relacion = new FuncionarioXConglomeradoModel
                    {
                        IdFuncionario = idFuncionario,
                        IdConglomerado = Convert.ToInt32(id)
                    };

                    //_servicioMantenimientos.FuncionarioXConglomerado.CrearFuncionarioXConglomerado(relacion);
                }


                var areasSeleccionadas = collection["IdAreasSeleccionadas"];

                foreach (var id in areasSeleccionadas)
                {
                    FuncionarioPorAreaModel relacion = new FuncionarioPorAreaModel
                    {
                        cedulaFuncionario = idFuncionario,
                        idArea = Convert.ToInt32(id)
                    };

                    //_servicioMantenimientos.FuncionarioPorArea.CrearFuncionarioPorArea(relacion);
                }


                if (ModelState.IsValid)
                {
                    TempData["MensajeExito"] = "Funcionario creado exitosamente";
                    return RedirectToAction("ManteniFuncionarios");
                }

                CargarListas(model);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al crear funcionario: {ex.Message}";
                CargarListas(model);
                return View(model);
            }
        }

        private void CargarListas(FuncionarioViewModel model)
        {
            model.Puestos = _servicioMantenimientos.Puestos.ListarPuesto();
            model.Conglomerados = _servicioMantenimientos.Conglomerados.ListarConglomerados();
            model.Departamentos = _servicioMantenimientos.Departamentos.ListarDepartamentos();
            model.Roles = _servicioMantenimientos.Roles.ListarRoles();
            model.EstadosFuncionario = _servicioMantenimientos.EstadoFuncionarios.ListarEstadosFuncionario();
            model.Areas = _servicioMantenimientos.Areas.ListarArea();
            model.Jefes = _servicioMantenimientos.Funcionario.ListarJefes();
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
                return View(new List<PuestoModel>());
            }
        }

        public ActionResult CreaPuesto()
        {
            return View("CreaPuesto", new PuestoModel());
        }

  
        [HttpPost]
        public ActionResult CreaPuesto(PuestoModel nuevoPuesto)
        {
            try
            {
                if (ModelState.IsValid)
                {
              
                   _servicioMantenimientos.Puestos.CrearPuesto(nuevoPuesto);

                
                    TempData["MensajeExito"] = $"Puesto {nuevoPuesto.Puesto} creado correctamente.";
                    return RedirectToAction(nameof(ManteniPuesto));
                }
                else
                {
                   
                    return View("CreaPuesto", nuevoPuesto); 
                }
            }
            catch (Exception ex)
            {
                
                TempData["MensajeError"] = $"Error al crear el puesto: {ex.Message}";
                return View("CreaPuesto", nuevoPuesto); 
            }
        }

    
        public ActionResult EditaPuesto(int id)
        {
            return View(_servicioMantenimientos.Puestos.ConsultarPuestoID(id));
        }

    
        [HttpPost]
        public ActionResult EditaPuesto(PuestoModel puestoModificado)
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

    }

}

