using Entidades;
using Negocios;
using Negocios.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CUC_Evaluacion_Desemp.Controllers
{
    public class ReportesController : Controller
    {
        private readonly IMantenimientosService _servicioMantenimientos;
        public ReportesController(IMantenimientosService servicio)
        {
            _servicioMantenimientos = servicio;

        }

        #region Reporte GEneral RH
        public ActionResult ReporteGeneralRH()
        {
            try
            {
                var listaDeptos = _servicioMantenimientos.Dependencias.ListarDependencias();
                var listaConglos = _servicioMantenimientos.Conglomerados.ListarConglomerados();
                var listaPuestos = _servicioMantenimientos.Puestos.ListarPuesto();
                var listaPeriodos = _servicioMantenimientos.Periodos.ListarPeriodos();

                ViewBag.Departamentos = listaDeptos
                    .Select(d => new SelectListItem
                    {
                        Value = d.IdDependencia.ToString(),
                        Text = d.Dependencia
                    })
                    .ToList();

                ViewBag.Conglomerados = listaConglos
                    .Select(c => new SelectListItem
                    {
                        Value = c.IdConglomerado.ToString(),
                        Text = c.NombreConglomerado
                    })
                    .ToList();

                ViewBag.Puestos = listaPuestos
                    .Select(p => new SelectListItem
                    {
                        Value = p.IdPuesto.ToString(),
                        Text = p.Puesto
                    })
                    .ToList();

                ViewBag.Periodos = listaPeriodos
                    .Select(pe => new SelectListItem
                    {
                        Value = pe.idPeriodo.ToString(),
                        Text = pe.Nombre
                    })
                    .ToList();

                ViewBag.ErrorCarga = null;
            }
            catch (Exception ex)
            {
                ViewBag.Departamentos = new List<SelectListItem>();
                ViewBag.Conglomerados = new List<SelectListItem>();
                ViewBag.Puestos = new List<SelectListItem>();
                ViewBag.Periodos = new List<SelectListItem>();

                ViewBag.ErrorCarga = "Ocurrió un error al cargar los datos para el reporte de RRHH.";
            }

            return View("ReporteGeneralRH");
        }

        [HttpPost]
        public JsonResult TraerReporteGeneralRH(string reporteGeneralData)
        {
            try
            {
                if (string.IsNullOrEmpty(reporteGeneralData))
                    throw new Exception("No se recibieron parámetros de búsqueda.");

                var dataEnJson = JsonConvert.DeserializeObject<JObject>(reporteGeneralData);

                var tipoReporte = dataEnJson["tipoReporte"]?.ToString();
                var filtro = dataEnJson["filtro"]?.ToString();
                var periodo = dataEnJson["periodo"]?.ToString();

                if (string.IsNullOrEmpty(tipoReporte))
                    throw new Exception("El tipo de reporte es obligatorio.");

                if (string.IsNullOrEmpty(periodo))
                    throw new Exception("El período es obligatorio.");

                var resultado = _servicioMantenimientos.ReportesNegocios.ReporteGeneralRH(tipoReporte,filtro,periodo);

                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new
                {
                    error = "Ocurrió un error al procesar la búsqueda de reportes.",
                    detalle = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult BuscarFuncionariosPorCedONombre(string criterio)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(criterio))
                {
                    return Json(new { error = "Criterio requerido" }, JsonRequestBehavior.AllowGet);
                }

                var lista = _servicioMantenimientos.Funcionario.BuscarFuncionariosPorCedONombre(criterio);

                var resultado = lista.Select(f => new
                {
                    cedula = f.Cedula,
                    nombreCompleto = f.Nombre + " " + f.Apellido1 + " " + f.Apellido2
                }).ToList();

                return Json(new { data = resultado }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { error = "Error interno al buscar funcionarios" }, JsonRequestBehavior.AllowGet);
            }
        }//BuscarFuncionariosPorCedONombre

        #endregion

        #region Reporte Por Funcioanrio
        public ActionResult SeleccionarSubalternoReporteEvaluacion()
        {
            try
            {
                var listaSubAlternos = _servicioMantenimientos.Funcionario.ListarSubAlternosConEvaluacionCerradasPorJefe("44444444");

                return View(listaSubAlternos);


            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Error al obtener la lista.";
                return View("Error");
            }
        }//fin

        [HttpPost]
        public ActionResult ReportesPDFPorFuncionario(string cedulaSeleccionada)
        {
            try
            {
                if (string.IsNullOrEmpty(cedulaSeleccionada))
                {
                    TempData["MensajeError"] = "Debe seleccionar un subalterno.";
                    return RedirectToAction("SeleccionarSubalternoReporteEvaluacion");
                }

                return RedirectToAction(nameof(ListarReportesFuncionario), new { cedula = cedulaSeleccionada });
            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Error al seleccionar Funcionario.";
                return RedirectToAction(nameof(SeleccionarSubalternoReporteEvaluacion));
            }
        }//ReportesPDFPorFuncionario

        [HttpGet]
        public ActionResult ListarReportesFuncionario(string cedula)
        {
            try
            {
                if (string.IsNullOrEmpty(cedula))
                {
                    TempData["MensajeError"] = "No se recibió la cédula del funcionario.";
                    return RedirectToAction(nameof(SeleccionarSubalternoReporteEvaluacion));
                }

                var rutaCarpeta = Server.MapPath("~/Reportes/" + cedula);

                if (!Directory.Exists(rutaCarpeta))
                {
                    ViewBag.Cedula = cedula;
                    ViewBag.Reportes = new List<ReportePdfViewModel>();
                    return View();
                }

                var archivos = Directory.GetFiles(rutaCarpeta, "*.pdf", SearchOption.TopDirectoryOnly);

                var listaReportes = archivos
                    .Select(fullPath => new ReportePdfViewModel
                    {
                        NombreArchivo = Path.GetFileName(fullPath),
                        RutaRelativa = Url.Content("~/Reportes/" + cedula + "/" + Path.GetFileName(fullPath)),
                        FechaCreacion = System.IO.File.GetCreationTime(fullPath)
                    })
                    .OrderByDescending(r => r.FechaCreacion)
                    .ToList();

                ViewBag.Cedula = cedula;
                ViewBag.Reportes = listaReportes;

                return View();
            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Error al cargar los reportes PDF.";
                return RedirectToAction(nameof(SeleccionarSubalternoReporteEvaluacion));
            }
        }

        #endregion


    }//fin class
}//fin Controller