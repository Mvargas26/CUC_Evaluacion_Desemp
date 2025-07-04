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
                var carreras = _servicioMantenimientos.Carreras.ListarCarreras();

                FuncionarioViewModel newFuncionarioViewModel = new FuncionarioViewModel
                {
                    Funcionario = funcionario,
                    Puestos = puestos,
                    Conglomerados = conglomerados,
                    Dependencias = dependencias,
                    Roles = roles,
                    EstadosFuncionario = estadosFunc,
                    Areas = Areas,
                    Jefes = Jefes,
                    Carreras = carreras
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

                if (!string.IsNullOrEmpty(conglomeradosSeleccionados))
                {
                    var ids = conglomeradosSeleccionados.Split(',');

                    foreach (var id in ids)
                    {
                        if (int.TryParse(id, out int idConglomerado))
                        {
                            FuncionarioXConglomeradoModel relacion = new FuncionarioXConglomeradoModel
                            {
                                IdFuncionario = idFuncionario,
                                IdConglomerado = idConglomerado
                            };

                            _servicioMantenimientos.FuncionarioXConglomerado.CrearFuncionarioXConglomerado(relacion);
                        }
                    }
                }


                var areasSeleccionadas = collection["IdAreasSeleccionadas"];

                if (!string.IsNullOrEmpty(areasSeleccionadas))
                {
                    var ids = areasSeleccionadas.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                               .Select(x => x.Trim())
                                               .ToArray();

                    foreach (var id in ids)
                    {
                        if (int.TryParse(id, out int idArea))
                        {
                            FuncionarioPorAreaModel relacion = new FuncionarioPorAreaModel
                            {
                                cedulaFuncionario = idFuncionario,  
                                idArea = idArea
                            };

                            _servicioMantenimientos.FuncionarioPorArea.CrearFuncionarioPorArea(relacion);
                        }
                    }
                }

                TempData["MensajeExito"] = $"Creado correctamente.";
                return RedirectToAction("ManteniFuncionarios");
            }
            catch (Exception )
            {
                TempData["MensajeError"] = $"Error al crear.";
                return RedirectToAction("ManteniFuncionarios");
            }
        }

        // GET: Cargar vista para modificar funcionario
        public ActionResult ModificarFuncionario(string cedula)
        {
            try
            {
                var funcionario = _servicioMantenimientos.Funcionario.ConsultarFuncionarioID(cedula);
                if (funcionario == null)
                {
                    TempData["MensajeError"] = "Funcionario no encontrado.";
                    return RedirectToAction("ManteniFuncionarios");
                }

                var puestos = _servicioMantenimientos.Puestos.ListarPuesto();
                var conglomerados = _servicioMantenimientos.Conglomerados.ListarConglomerados();
                var dependencias = _servicioMantenimientos.Dependencias.ListarDependencias();
                var roles = _servicioMantenimientos.Roles.ListarRoles();
                var estadosFunc = _servicioMantenimientos.EstadoFuncionarios.ListarEstadosFuncionario();
                var areas = _servicioMantenimientos.Areas.ListarArea();
                var jefes = _servicioMantenimientos.Funcionario.ListarJefes();
                var carreras = _servicioMantenimientos.Carreras.ListarCarreras();

                var congActuales = _servicioMantenimientos.Funcionario.ConglomeradosPorFunc(cedula);
                //var areasActuales = _servicioMantenimientos.Funcionario.AreasPorFuncionario(cedula);

                FuncionarioViewModel viewModel = new FuncionarioViewModel
                {
                    Funcionario = funcionario,
                    Puestos = puestos,
                    Conglomerados = conglomerados,
                    Dependencias = dependencias,
                    Roles = roles,
                    EstadosFuncionario = estadosFunc,
                    Areas = areas,
                    Jefes = jefes,
                    Carreras = carreras,
                    IdConglomeradosSeleccionados = congActuales.Select(c => c.IdConglomerado).ToList(),

                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al cargar datos del funcionario: {ex.Message}";
                return RedirectToAction("ManteniFuncionarios");
            }
        }

        // POST: Guardar cambios al funcionario
        [HttpPost]
        public ActionResult ModificarFuncionario(FuncionarioViewModel model, FormCollection collection)
        {
            try
            {
                string cedula = model.Funcionario.Cedula;

                // ✅ Si el campo de contraseña viene vacío, recuperamos la actual
                if (string.IsNullOrWhiteSpace(model.Funcionario.Password))
                {
                    var actual = _servicioMantenimientos.Funcionario.ConsultarFuncionarioID(cedula);
                    if (actual != null)
                    {
                        model.Funcionario.Password = actual.Password;
                    }
                }

                // Actualiza datos principales
                _servicioMantenimientos.Funcionario.ModificarFuncionario(model.Funcionario);

                // Actualiza conglomerados
                _servicioMantenimientos.FuncionarioXConglomerado.EliminarPorFuncionario(cedula);
                var conglomeradosSeleccionados = collection["IdConglomeradosSeleccionados"];
                if (!string.IsNullOrEmpty(conglomeradosSeleccionados))
                {
                    foreach (var id in conglomeradosSeleccionados.Split(','))
                    {
                        if (int.TryParse(id, out int idConglomerado))
                        {
                            _servicioMantenimientos.FuncionarioXConglomerado.CrearFuncionarioXConglomerado(new FuncionarioXConglomeradoModel
                            {
                                IdFuncionario = cedula,
                                IdConglomerado = idConglomerado
                            });
                        }
                    }
                }

                // Actualiza áreas
                _servicioMantenimientos.Funcionario.EliminarFuncionario(cedula);
                var areasSeleccionadas = collection["IdAreasSeleccionadas"];
                if (!string.IsNullOrEmpty(areasSeleccionadas))
                {
                    foreach (var id in areasSeleccionadas.Split(','))
                    {
                        if (int.TryParse(id, out int idArea))
                        {
                            _servicioMantenimientos.FuncionarioPorArea.CrearFuncionarioPorArea(new FuncionarioPorAreaModel
                            {
                                cedulaFuncionario = cedula,
                                idArea = idArea
                            });
                        }
                    }
                }

                TempData["MensajeExito"] = "Funcionario modificado correctamente.";
                return RedirectToAction("ManteniFuncionarios");
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al modificar: {ex.Message}";
                return RedirectToAction("ManteniFuncionarios");
            }
        }



        #endregion

        #region Puestos

        public ActionResult ManteniPuesto()
        {
            try
            {
                ViewBag.ListarDependencias = _servicioMantenimientos.Dependencias.ListarDependencias();
                var puestos = _servicioMantenimientos.Puestos.ListarPuesto();
              


                return View(puestos);
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al obtener los puestos: {ex.Message}";
                return View(new List<PuestosModel>());
            }
        }

        [HttpPost]
        public ActionResult CreaPuesto(PuestosModel nuevoPuesto)
        {
            try
            {
                if (ModelState.IsValid)
                {
            
                   _servicioMantenimientos.Puestos.CrearPuesto(nuevoPuesto);
                    ViewBag.ListaAreas = _servicioMantenimientos.Dependencias.ListarDependencias();


                    TempData["MensajeExito"] = $"Puesto {nuevoPuesto.Puesto} creado correctamente.";
                    return RedirectToAction(nameof(ManteniPuesto));
                }
                else
                {
                    return RedirectToAction(nameof(ManteniPuesto));
                }
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = ex.Message.Contains("UNIQUE KEY")
                   ? "¡ ALERTA! Este registro ya existe."
                   : "Error al procesar la solicitud.";

                return RedirectToAction(nameof(ManteniPuesto));
            }
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
                    return RedirectToAction(nameof(ManteniPuesto));
                }
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = ex.Message.Contains("UNIQUE KEY")
                   ? "¡ ALERTA! Este registro ya existe."
                   : "Error al procesar la solicitud.";

                return RedirectToAction(nameof(ManteniPuesto));
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
                    TempData["MensajeExito"] = $"Puesto eliminado correctamente.";
                }
                return RedirectToAction(nameof(ManteniPuesto));
            }
            catch
            {
                TempData["MensajeError"] = "No puede borrar este puesto,esta siendo utilizado.";
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
            catch (Exception )
            {
                TempData["MensajeError"] = "Error al obtener las areas.";
                return RedirectToAction(nameof(ManteniArea));
            }
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

                    return View("ManteniArea", nuevoArea);
                }
            }
            catch (Exception )
            {

                TempData["MensajeError"] = "Error al crear el area.";
                return View("ManteniArea", nuevoArea);
            }
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
                    return View("ManteniArea", areaModificado);
                }
            }
            catch (Exception )
            {
                TempData["MensajeError"] = $"Error al actualizar la area.";
                return RedirectToAction(nameof(ManteniArea));
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
                TempData["MensajeError"] = ex.Message.Contains("UNIQUE KEY")
                   ? "¡ ALERTA! Este registro ya existe."
                   : "Error al procesar la solicitud.";

                return RedirectToAction(nameof(ManteniDependencias));

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
            catch (Exception)
            {
                TempData["MensajeError"] = "No se puede eliminar porque esta siendo utilizada. ";
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
                TempData["MensajeError"] = ex.Message.Contains("UNIQUE KEY")
                   ? "¡ ALERTA! Este registro ya existe."
                   : "Error al procesar la solicitud.";

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
            catch (Exception )
            {
                TempData["MensajeError"] = $"Error al obtener la lista.";
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
            catch (Exception)
            {

                TempData["MensajeError"] = $"Error al crear.";
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
            catch (Exception)
            {
                TempData["MensajeError"] = "No se puede eliminar porque esta siendo utilizada.";
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
            catch (Exception )
            {
                TempData["MensajeError"] = "Error al editar. ";
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
            catch (Exception )
            {
                TempData["MensajeError"] = $"Error al obtener la lista:";
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
            catch (Exception )
            {

                TempData["MensajeError"] = $"Error al crear:";
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
            catch (Exception )
            {
                TempData["MensajeError"] = "No se puede eliminar porque esta siendo utilizada." ;
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
            catch (Exception )
            {
                TempData["MensajeError"] = "Error al editar. " ;
                return RedirectToAction(nameof(ManteniCarreras));
            }
        }
        #endregion
    }//fin class

}

