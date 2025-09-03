using Entidades;
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
                return View("Error");
            }
        }//fin SeleccionarSubalterno

        [HttpPost]
        public ActionResult SeleccionarSubalterno(string cedulaSeleccionada)
        {
            try
            {
                if (string.IsNullOrEmpty(cedulaSeleccionada))
                {
                    TempData["MensajeError"] = "Debe seleccionar un subalterno.";
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
                if (string.IsNullOrEmpty(cedulaSeleccionada))
                {
                    TempData["MensajeError"] = "Debe seleccionar un Conglomerado para el funcionario a evaluar.";
                    return RedirectToAction("SeleccionarSubalterno");
                }

                ViewBag.cedulaSeleccionada = cedulaSeleccionada;

                var listaConglomerados = _servicioMantenimientos.Funcionario.ConglomeradosPorFunc(cedulaSeleccionada);
                    return View(listaConglomerados);
            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Error al obtener la lista.";
                return RedirectToAction(nameof(SeleccionarSubalterno));
            }
        
        }

        [HttpPost]
        public ActionResult SeleccionarPeriodo(FormCollection coleccion)
        {
            try
            {
                string cedulaSeleccionada = coleccion["cedulaSeleccionada"];
                string idConglomerado = coleccion["idConglomerado"];

                ViewBag.cedulaSeleccionada = cedulaSeleccionada;
                ViewBag.idConglomerado = idConglomerado;

                var periodos = _servicioMantenimientos.Periodos.ListarPeriodosAnioActual();
                return View(periodos);
            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Error al obtener la lista.";
                return RedirectToAction(nameof(SeleccionarSubalterno));
            }
        }

        [HttpPost]
        public ActionResult PlanificarEvaluacion(FormCollection coleccion)
        {
            try
            {
                string cedulaSeleccionada = coleccion["cedulaSeleccionada"];
                int idConglomerado = Convert.ToInt32(coleccion["idConglomerado"]);
                string idPeriodo = coleccion["idPeriodo"];

                if (string.IsNullOrEmpty(cedulaSeleccionada) || string.IsNullOrEmpty(coleccion["idConglomerado"]))
                {
                    TempData["MensajeError"] = "Debe seleccionar un funcionario y un conglomerado.";
                    return RedirectToAction("SeleccionarSubalterno");
                }

                //obtenemos el funcionario
                var subalterno = _servicioMantenimientos.Funcionario.ConsultarFuncionarioID(cedulaSeleccionada);
                //obtenemos los pesos de su conglomerado
                var PesosConglomerados = _servicioMantenimientos.Conglomerados.ConsultarPesosXConglomerado(idConglomerado);
                // Obtenemos tipos de Objetivos
                ViewData["ListaTiposObjetivos"] = _servicioMantenimientos.TiposObjetivos.ListarTiposObjetivos();
                //Obtenemos tipos de competencias
                ViewData["ListaTiposCompetencias"] = _servicioMantenimientos.TiposCompetencias.ListarTiposCompetencias();
                //Obtenemos la lista de conglomerados
                ViewData["ListaConglomerados"] = _servicioMantenimientos.Conglomerados.ListarConglomerados();
                //obtenemos los objetivos y competencias relacionadas a este congloemrado
                var (listaObjetivos, listaCompetencias) = _servicioMantenimientos.Evaluaciones.ListarObjYCompetenciasXConglomerado(idConglomerado);

                //Obtenemos las competencias, comportamientos y descrp Transversales id 2500
                var transversales = _servicioMantenimientos.ObtenerComportamientosYDescripciones.ListarComportamientosYDescripcionesNegocios(2500, "PorTipo");

                var CompetenciasDelConglomerado = _servicioMantenimientos.ObtenerComportamientosYDescripciones.ListarComportamientosYDescripcionesNegociosXCOnglo(idConglomerado);
                var tiposdeObjetivos = _servicioMantenimientos.TiposObjetivos.ListarTiposObjetivos();
                //pasamos todo a la vista
                ViewBag.ListaObjetivos = listaObjetivos;
                ViewBag.ListaCompetencias = listaCompetencias;
                ViewBag.PesosConglomerados = PesosConglomerados;
                ViewBag.IdConglomerado = idConglomerado;
                ViewBag.transversales = transversales;
                ViewBag.CompetenciasDelConglomerado = CompetenciasDelConglomerado;
                ViewBag.tiposdeObjetivos = tiposdeObjetivos;


                return View(subalterno);

            }
            catch (Exception )
            {
                TempData["MensajeError"] = "Error al obtener las competencias.";
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult GuardarPlanificacion(string evaluacionData )
        {
            try
            {
                //Convertimos de string a Json lo que viene de la vista planificacion para tratarla
                var dataEnJSon = JsonConvert.DeserializeObject<JObject>(evaluacionData);

                var cedFuncionario = dataEnJSon["cedFuncionario"]?.ToString();
                var idConglo = dataEnJSon["idConglo"]?.ToString();
                var observaciones = dataEnJSon["observaciones"]?.ToString();

                var objetivos = dataEnJSon["objetivos"];
                var competenciasTransversales = dataEnJSon["competenciasTransversales"];
                var competencias = dataEnJSon["competencias"];

                //Crear un objeto tipo Evaluacion y Guardarlo
                EvaluacionModel evaluacionGuardada = _servicioMantenimientos.Evaluaciones.CrearEvaluacion(new EvaluacionModel
                {
                    IdFuncionario = cedFuncionario.ToString(),
                    Observaciones = observaciones.ToString(),
                    FechaCreacion = DateTime.Now,
                    EstadoEvaluacion = 1, // planificada
                    IdConglomerado = Convert.ToInt32(idConglo)
                });

                // Guardamos los objetivos ya teniendo el id de la eva
                foreach (var objetivo in objetivos)
                {
                    var evaluacionXObjetivo = new EvaluacionXObjetivoModel
                    {
                        IdEvaluacion = evaluacionGuardada.IdEvaluacion,
                        IdObjetivo = Convert.ToInt32(objetivo["id"]),
                        ValorObtenido = Convert.ToDecimal(objetivo["actual"]),
                        Peso = Convert.ToDecimal(objetivo["peso"]),
                        Meta = objetivo["meta"].ToString()
                    };

                    _servicioMantenimientos.EvaluacionXobjetivos.CrearEvaluacionXObjetivo(evaluacionXObjetivo);
                }

                //guardamos las competencias
                foreach (var competencia in competenciasTransversales)
                {
                    var evaluacionXCompetencia = new EvaluacionXcompetenciaModel
                    {
                        IdEvaluacion = evaluacionGuardada.IdEvaluacion,
                        IdCompetencia = Convert.ToInt32(competencia["id"]),
                        ValorObtenido = Convert.ToDecimal(competencia["actual"]),
                        Peso = Convert.ToDecimal(competencia["peso"]),
                        Meta = competencia["meta"].ToString()
                    };

                    _servicioMantenimientos.EvaluacionXcompetencia.CrearEvaluacionXCompetencia(evaluacionXCompetencia);
                }



                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Home");
            }

        }
        #endregion

        #region EvaFuncionario



        #endregion

        #region AprobarComoJefe

        #endregion








    }//fin class
}