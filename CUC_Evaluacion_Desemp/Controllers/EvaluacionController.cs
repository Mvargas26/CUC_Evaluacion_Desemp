using Entidades;
using Negocios.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CUC_Evaluacion_Desemp.Controllers
{
    public class EvaluacionController : Controller
    {
        private readonly IMantenimientosService _servicioMantenimientos;

        public EvaluacionController(IMantenimientosService servicio)
        {
            _servicioMantenimientos = servicio;

        }

        #region Planificacion

        public ActionResult SeleccionarSubalterno() 
        {
            try
            {
                var listaSubAlternos = _servicioMantenimientos.Funcionario.ListarSubAlternosPorJefe("44444444");

                return View(listaSubAlternos);


            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Error al obtener la lista.";
                return RedirectToAction(nameof(SeleccionarSubalterno));
            }
        }//fin SeleccionarSubalterno

        [HttpPost]
        public ActionResult SeleccionarSubalterno(string cedulaSeleccionada)
        {
            try
            {
                if (string.IsNullOrEmpty(cedulaSeleccionada))
                {
                    TempData["Error"] = "Debe seleccionar un subalterno.";
                    return RedirectToAction("SeleccionarSubalterno");
                }

                return RedirectToAction("ConglomeradosPorFunc", new { cedulaSeleccionada = cedulaSeleccionada });

            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Error al seleccionar Funcionario.";
                return RedirectToAction(nameof(SeleccionarSubalterno));
            }
        }

        public ActionResult ConglomeradosPorFunc(string cedulaSeleccionada) 
        {
            try
            {
                var listaConglomerados = _servicioMantenimientos.Funcionario.ConglomeradosPorFunc(cedulaSeleccionada);
                    return View(listaConglomerados);
            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Error al obtener la lista.";
                return RedirectToAction(nameof(SeleccionarSubalterno));
            }
        
        }

        #endregion

        #region EvaFuncionario

        public ActionResult EvaluacionSubalterno()
        {
            return View();
        }

        #endregion

        #region AprobarComoJefe

        #endregion








    }//fin class
}