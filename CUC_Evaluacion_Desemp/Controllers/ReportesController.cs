using Negocios;
using Negocios.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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


    }//fin class
}//fin Controller