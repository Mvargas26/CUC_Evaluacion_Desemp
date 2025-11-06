using Entidades;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Negocios.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Font = iTextSharp.text.Font;
using Image = iTextSharp.text.Image;

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

                if (string.IsNullOrEmpty(idConglomerado))
                {
                    TempData["MensajeError"] = "Debe seleccionar un Conglomerado.";
                    return RedirectToAction("ConglomeradosPorFunc", new { cedulaSeleccionada = cedulaSeleccionada });
                }

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
                //Obtenemos la fase de planificacion para pintarla de titulo
                EstadoEvaluacionModel faseActual = _servicioMantenimientos.EstadoEvaluacion.ConsultarEstadoPorID(1);

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
                ViewBag.faseActual = faseActual;
                ViewBag.idPeriodo = idPeriodo;

                return View(subalterno);

            }
            catch (Exception )
            {
                TempData["MensajeError"] = "Error al obtener las listas para planificar.";
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
                var idPeriodo = dataEnJSon["idPeriodo"]?.ToString();
                var notaFinal = dataEnJSon["notaFinal"]?.ToString();
                var observaciones = dataEnJSon["observaciones"]?.ToString();

                var objetivos = dataEnJSon["objetivos"];
                var competenciasTransversales = dataEnJSon["competenciasTransversales"];
                var competencias = dataEnJSon["competencias"];

                EstadoEvaluacionModel faseActual = _servicioMantenimientos.EstadoEvaluacion.ConsultarEstadoPorID(1);

                //Crear un objeto tipo Evaluacion y Guardarlo
                EvaluacionModel evaluacionGuardada = _servicioMantenimientos.Evaluaciones.CrearEvaluacion(new EvaluacionModel
                {
                    IdFuncionario = cedFuncionario.ToString(),
                    Observaciones = observaciones.ToString(),
                    FechaCreacion = DateTime.Now,
                    EstadoEvaluacion = 1, // planificada
                    IdConglomerado = Convert.ToInt32(idConglo),
                    IdPeriodo = Convert.ToInt32(idPeriodo),
                    NotaFinal=Convert.ToDecimal(notaFinal)
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

                //guardamos las competencias Transversales
                foreach (var competencia in competenciasTransversales)
                {
                    var idCompetencia = Convert.ToInt32(competencia["idCompetencia"]);
                    var idTipoCompetencia = Convert.ToInt32(competencia["idTipoCompetencia"]);

                    foreach (var comportamiento in competencia["comportamientos"])
                    {
                        var idComportamiento = Convert.ToInt32(comportamiento["idComportamiento"]);

                        foreach (var nivel in comportamiento["niveles"])
                        {
                            var idNivel = Convert.ToInt32(nivel["idNivel"]);

                            var evaluacionXCompetencia = new EvaluacionXcompetenciaModel
                            {
                                IdEvaluacion = evaluacionGuardada.IdEvaluacion,
                                IdCompetencia = idCompetencia,
                                IdComportamiento = idComportamiento,
                                IdNivel = idNivel,
                                ValorObtenido = 0 
                            };

                            _servicioMantenimientos.EvaluacionXcompetencia.CrearEvaluacionXCompetencia(evaluacionXCompetencia);
                        }
                    }
                }

                //guardamos las competencias Normales asignadas del combo
                foreach (var competencia in competencias)
                {
                    var idCompetencia = Convert.ToInt32(competencia["idCompetencia"]);
                    var idTipoCompetencia = Convert.ToInt32(competencia["idTipoCompetencia"]);

                    foreach (var comportamiento in competencia["comportamientos"])
                    {
                        var idComportamiento = Convert.ToInt32(comportamiento["idComportamiento"]);

                        foreach (var nivel in comportamiento["niveles"])
                        {
                            var idNivel = Convert.ToInt32(nivel["idNivel"]);

                            var evaluacionXCompetencia = new EvaluacionXcompetenciaModel
                            {
                                IdEvaluacion = evaluacionGuardada.IdEvaluacion,
                                IdCompetencia = idCompetencia,
                                IdComportamiento = idComportamiento,
                                IdNivel = idNivel,
                                ValorObtenido = 0 // si después necesitas ponerle nota real, aquí la cargas
                            };

                            _servicioMantenimientos.EvaluacionXcompetencia.CrearEvaluacionXCompetencia(evaluacionXCompetencia);
                        }
                    }
                }

                // Construimos la URL del PDF para mostrarlo al usuario
                var fechaNorm = DateTime.Now.ToString("yyyyMMdd_HHmm");
                var nombreArchivo = $"planificar_{cedFuncionario}_{fechaNorm}.pdf";
                CrearReportePDFEvaluacion(dataEnJSon, evaluacionGuardada, nombreArchivo,faseActual);

                var urlArchivo = Url.Content("~/Reportes/"+cedFuncionario+"/" + nombreArchivo);
                return Json(new { ok = true, pdfUrl = urlArchivo, fileName = nombreArchivo, message = "Evaluación planificada correctamente" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Home");
            }

        }// fin GuardarPlanificacion

        #endregion


        #region EvaluarFuncionario
        public ActionResult SeleccionarSubalternoParaEvaluar()
        {
            try
            {
                var listaSubAlternos = _servicioMantenimientos.Funcionario.ListarSubAlternosConEvaluacionPorJefe("44444444");

                return View(listaSubAlternos);


            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Error al obtener la lista.";
                return View("Error");
            }
        }//fin

        [HttpPost]
        public ActionResult SeleccionarSubalternoParaEvaluar(string cedulaSeleccionada)
        {
            try
            {
                if (string.IsNullOrEmpty(cedulaSeleccionada))
                {
                    TempData["MensajeError"] = "Debe seleccionar un subalterno.";
                    return RedirectToAction("SeleccionarSubalternoParaEvaluar");
                }

                return RedirectToAction("ConglomeradosPorFuncAParaEvaluar", new { cedulaSeleccionada = cedulaSeleccionada });

            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Error al seleccionar Funcionario.";
                return RedirectToAction(nameof(SeleccionarSubalterno));
            }
        }

        public ActionResult ConglomeradosPorFuncAParaEvaluar(string cedulaSeleccionada)
        {
            try
            {
                if (string.IsNullOrEmpty(cedulaSeleccionada))
                {
                    TempData["MensajeError"] = "Debe seleccionar un Conglomerado para el funcionario a evaluar.";
                    return RedirectToAction("SeleccionarSubalternoParaEvaluar");
                }

                ViewBag.cedulaSeleccionada = cedulaSeleccionada;

                var listaConglomerados = _servicioMantenimientos.Funcionario.ConglomeradosPorFunc(cedulaSeleccionada);
                return View(listaConglomerados);
            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Error al obtener la lista.";
                return RedirectToAction(nameof(SeleccionarSubalternoParaEvaluar));
            }

        }

        [HttpPost]
        public ActionResult SelecPeriodoPorFunParaEvaluar(FormCollection coleccion)
        {
            try
            {
                string cedulaSeleccionada = coleccion["cedulaSeleccionada"];
                string idConglomerado = coleccion["idConglomerado"];

                ViewBag.cedulaSeleccionada = cedulaSeleccionada;
                ViewBag.idConglomerado = idConglomerado;

                if (string.IsNullOrEmpty(idConglomerado))
                {
                    TempData["MensajeError"] = "Debe seleccionar un Conglomerado.";
                    return RedirectToAction("ConglomeradosPorFuncAParaEvaluar", new { cedulaSeleccionada = cedulaSeleccionada });
                }

                var periodos = _servicioMantenimientos.Periodos.ListarPeriodosAnioActual();
                return View(periodos);
            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Error al obtener la lista.";
                return RedirectToAction(nameof(SeleccionarSubalternoParaEvaluar));
            }
        }

        [HttpPost]
        public ActionResult EvaluarSubAlterno(FormCollection coleccion)
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

                var subalterno = _servicioMantenimientos.Funcionario.ConsultarFuncionarioID(cedulaSeleccionada);
                var PesosConglomerados = _servicioMantenimientos.Conglomerados.ConsultarPesosXConglomerado(idConglomerado);
                ViewData["ListaTiposObjetivos"] = _servicioMantenimientos.TiposObjetivos.ListarTiposObjetivos();
                ViewData["ListaTiposCompetencias"] = _servicioMantenimientos.TiposCompetencias.ListarTiposCompetencias();
                ViewData["ListaConglomerados"] = _servicioMantenimientos.Conglomerados.ListarConglomerados();

                EstadoEvaluacionModel faseActual = _servicioMantenimientos.EstadoEvaluacion.ConsultarEstadoPorID(2);

                var ultimaEvaluacionFuncionario = _servicioMantenimientos.Evaluaciones.ConsultarEvaluacionComoFuncionario(cedulaSeleccionada, idConglomerado);

                if (ultimaEvaluacionFuncionario == null)
                {
                    TempData["AlertMessage"] = "No hay una evaluación para usted en este conglomerado.Por favor contacte a su Jefatura para planificarla.";
                    return RedirectToAction("Index", "Home");
                }

                var listaObjetivos = _servicioMantenimientos.Evaluaciones.Listar_objetivosXEvaluacion(ultimaEvaluacionFuncionario.IdEvaluacion);

                var CompetenciasPlanificadas = _servicioMantenimientos.ObtenerComportamientosYDescripciones.ListarComportamientosYDescripcionesNegocios(ultimaEvaluacionFuncionario.IdEvaluacion, "PorEvaluacion");

                var Transversales = CompetenciasPlanificadas
                    .Where(c => Convert.ToInt32(c.idTipoCompetencia) == 2500)
                    .ToList();

                var Competencias = CompetenciasPlanificadas
                    .Where(c => Convert.ToInt32(c.idTipoCompetencia) != 2500)
                    .ToList();

                var transversalesAgrupadas = Transversales
                    .GroupBy(x => new { x.idCompetencia, x.Competencia, x.DescriCompetencia, x.idTipoCompetencia, x.Tipo })
                    .Select(g => new CompetenciasModel
                    {
                        IdCompetencia = g.Key.idCompetencia,
                        Competencia = g.Key.Competencia,
                        Descripcion = g.Key.DescriCompetencia,
                        IdTipoCompetencia = g.Key.idTipoCompetencia,
                        TipoCompetencia = new TiposCompetenciasModel { IdTipoCompetencia = g.Key.idTipoCompetencia, Tipo = g.Key.Tipo },
                        Comportamientos = g
                            .GroupBy(k => new { k.idComport, k.Comportamiento })
                            .Select(cg => new ComportamientoModel
                            {
                                idComport = cg.Key.idComport,
                                Nombre = cg.Key.Comportamiento,
                                Niveles = cg
                                    .GroupBy(n => new { n.idNivel, n.Nivel, n.Descripcion, n.valorNivel, n.idNivelElegido }) 
                                    .Select(ng => new NivelComportamientoModel
                                    {
                                        idNivel = ng.Key.idNivel,
                                        nombre = ng.Key.Nivel,
                                        descripcion = ng.Key.Descripcion,
                                        valor = ng.Key.valorNivel,
                                        idNivelElegido = ng.Key.idNivelElegido,
                                        idEvaxComp = ng.Select(z => z.idEvaxComp).FirstOrDefault(v => v > 0)
                                    }).ToList()
                            }).ToList()
                    }).ToList();

                var CompetenciasAgrupadas = Competencias
                 .GroupBy(x => new { x.idCompetencia, x.Competencia, x.DescriCompetencia, x.idTipoCompetencia, x.Tipo })
                 .Select(g => new Entidades.CompetenciasModel
                 {
                     IdCompetencia = g.Key.idCompetencia,
                     Competencia = g.Key.Competencia,
                     Descripcion = g.Key.DescriCompetencia,
                     IdTipoCompetencia = g.Key.idTipoCompetencia,
                     TipoCompetencia = new Entidades.TiposCompetenciasModel { IdTipoCompetencia = g.Key.idTipoCompetencia, Tipo = g.Key.Tipo },
                     Comportamientos = g
                         .GroupBy(k => new { k.idComport, k.Comportamiento })
                         .Select(cg => new Entidades.ComportamientoModel
                         {
                             idComport = cg.Key.idComport,
                             Nombre = cg.Key.Comportamiento,
                             Niveles = cg
                                 .GroupBy(n => new { n.idNivel, n.Nivel, n.Descripcion, n.valorNivel, n.idNivelElegido }) 
                                 .Select(ng => new Entidades.NivelComportamientoModel
                                 {
                                     idNivel = ng.Key.idNivel,
                                     nombre = ng.Key.Nivel,
                                     descripcion = ng.Key.Descripcion,
                                     valor = ng.Key.valorNivel,
                                     idNivelElegido = ng.Key.idNivelElegido,
                                     idEvaxComp = ng.Select(z => z.idEvaxComp).FirstOrDefault(v => v > 0)
                                 }).ToList()
                         }).ToList()
                 }).ToList();

                var listaNiveles = _servicioMantenimientos.NivelesComportamientos.ListarNivelesComportamientos();
                var valorPorNivel = listaNiveles.ToDictionary(n => n.idNivel, n => n.valor);

                int Val(int idNivel) => valorPorNivel.TryGetValue(idNivel, out var v) ? v : 0;

                var maxPorTransversal = (Transversales ?? Enumerable.Empty<ObtenerComportamientosYDescripcionesModel>())
                    .GroupBy(x => x.idComport)
                    .Select(g => Val(g.Max(e => e.idNivel)));

                var maxPorCompetencia = (Competencias ?? Enumerable.Empty<ObtenerComportamientosYDescripcionesModel>())
                    .GroupBy(x => x.idComport)
                    .Select(g => Val(g.Max(e => e.idNivel)));

                int MaximoPuntosCompetencias = maxPorTransversal.Sum() + maxPorCompetencia.Sum();

                ViewBag.ListaObjetivos = listaObjetivos;
                ViewBag.Transversales = transversalesAgrupadas;
                ViewBag.CompetenciasAgrupadas = CompetenciasAgrupadas;
                ViewBag.PesosConglomerados = PesosConglomerados;
                ViewBag.IdConglomerado = idConglomerado;
                ViewBag.MaximoPuntosCompetencias = MaximoPuntosCompetencias;
                ViewBag.ultimaEvaluacionFuncionario = ultimaEvaluacionFuncionario;
                ViewBag.faseActual = faseActual;
                ViewBag.idPeriodo = idPeriodo;

                return View(subalterno);
            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Error al cargar la vista para evaluar.";
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult GuardarSeguimiento(string evaluacionData)
        {
            try
            {
                //Convertimos de string a Json lo que viene de la vista para tratarla
                var dataEnJSon = JsonConvert.DeserializeObject<JObject>(evaluacionData);

                var cedFuncionario = dataEnJSon["cedFuncionario"]?.ToString();
                var idConglo = dataEnJSon["idConglo"]?.ToString();
                var observaciones = dataEnJSon["observaciones"]?.ToString();

                var objetivos = dataEnJSon["objetivos"];
                var competenciasTransversales = dataEnJSon["competenciasTransversales"];
                var competencias = dataEnJSon["competencias"];
                var notaFinal = dataEnJSon["notaFinal"]?.ToString();
                var idPeriodo = dataEnJSon["idPeriodo"]?.ToString();



                EstadoEvaluacionModel faseActual = _servicioMantenimientos.EstadoEvaluacion.ConsultarEstadoPorID(2);

                //consultamos la ultima evaluacion 
                var ultimaEvaluacionFuncionario = _servicioMantenimientos.Evaluaciones.ConsultarEvaluacionComoFuncionario(cedFuncionario, Convert.ToInt32(idConglo));

                if (ultimaEvaluacionFuncionario == null)
                {
                    TempData["AlertMessage"] = "No hay una evaluación para usted en este conglomerado.Por favor contacte a su Jefatura para planificarla.";
                    return RedirectToAction("Index", "Home");
                }

                //actualizamos su estado y lo guardamos
                ultimaEvaluacionFuncionario.EstadoEvaluacion = 2; //"Estado 2 = Por aprobar"
                ultimaEvaluacionFuncionario.Observaciones = observaciones;
                ultimaEvaluacionFuncionario.NotaFinal = Convert.ToDecimal(notaFinal);
                ultimaEvaluacionFuncionario.IdPeriodo = Convert.ToInt32(idPeriodo);
                _servicioMantenimientos.Evaluaciones.ModificarEvaluacion(ultimaEvaluacionFuncionario);

                // Actualizamos el actual de los obj
                foreach (var objetivo in objetivos)
                {
                    var evaluacionXObjetivo = new EvaluacionXObjetivoModel
                    {
                        IdEvaxObj = Convert.ToInt32(objetivo["idEvaxObj"]),
                        IdEvaluacion = ultimaEvaluacionFuncionario.IdEvaluacion,
                        IdObjetivo = Convert.ToInt32(objetivo["id"]),
                        ValorObtenido = Convert.ToDecimal(objetivo["actual"]),
                        Peso = Convert.ToDecimal(objetivo["peso"]),
                        Meta = objetivo["meta"].ToString()
                    };

                    _servicioMantenimientos.EvaluacionXobjetivos.ModificarEvaluacionXObjetivo(evaluacionXObjetivo);
                }
                //******************************************************************************************************************************
                // Actualizamos competencias Transversales
                foreach (var competencia in competenciasTransversales)
                {
                    var idCompetencia = Convert.ToInt32(competencia["idCompetencia"]);

                    foreach (var comportamiento in competencia["comportamientos"])
                    {
                        var idComportamiento = Convert.ToInt32(comportamiento["idComportamiento"]);

                        int idNivelElegido = 0;

                        foreach (var n in comportamiento["niveles"])
                            if (n["idNivelElegido"] != null && int.TryParse(n["idNivelElegido"]?.ToString(), out var e) && e > 0) { idNivelElegido = e; break; }

                        if (idNivelElegido == 0)
                            foreach (var n in comportamiento["niveles"])
                                if (n["idEvaxComp"] != null && int.TryParse(n["idEvaxComp"]?.ToString(), out var evx) && evx > 0) { idNivelElegido = Convert.ToInt32(n["idNivel"]); break; }

                        if (idNivelElegido == 0)
                            foreach (var n in comportamiento["niveles"])
                                if (decimal.TryParse(n["valor"]?.ToString(), out var val) && val > 0) { idNivelElegido = Convert.ToInt32(n["idNivel"]); break; }

                        foreach (var nivel in comportamiento["niveles"])
                        {
                            var registro = new EvaluacionXcompetenciaModel
                            {
                                IdEvaxComp = nivel["idEvaxComp"] != null ? Convert.ToInt32(nivel["idEvaxComp"]) : 0,
                                IdEvaluacion = ultimaEvaluacionFuncionario.IdEvaluacion,
                                IdCompetencia = idCompetencia,
                                IdComportamiento = idComportamiento,
                                IdNivel = Convert.ToInt32(nivel["idNivel"]),
                                ValorObtenido = nivel["valor"] != null ? Convert.ToDecimal(nivel["valor"]) : 0m
                            };

                            _servicioMantenimientos.EvaluacionXcompetencia.ActualizarEvaluacionXCompetencia(registro);
                        }

                        // Ponemos el elegido a TODAS las filas del grupo )osea a los niveles relacionados a esa comeptencia:
                        _servicioMantenimientos.EvaluacionXcompetencia
                            .ActualizarNivelElegidoPorGrupo(
                                ultimaEvaluacionFuncionario.IdEvaluacion,
                                idCompetencia,
                                idComportamiento,
                                idNivelElegido
                            );
                    }
                }
                //******************************************************************************************************************************
                // Actualizamos competencias (no transversales)
                foreach (var competencia in competencias)
                {
                    var idCompetencia = Convert.ToInt32(competencia["idCompetencia"]);

                    foreach (var comportamiento in competencia["comportamientos"])
                    {
                        var idComportamiento = Convert.ToInt32(comportamiento["idComportamiento"]);

                        int idNivelElegido = 0;

                        foreach (var n in comportamiento["niveles"])
                            if (n["idNivelElegido"] != null && int.TryParse(n["idNivelElegido"]?.ToString(), out var e) && e > 0) { idNivelElegido = e; break; }

                        if (idNivelElegido == 0)
                            foreach (var n in comportamiento["niveles"])
                                if (n["idEvaxComp"] != null && int.TryParse(n["idEvaxComp"]?.ToString(), out var evx) && evx > 0) { idNivelElegido = Convert.ToInt32(n["idNivel"]); break; }

                        if (idNivelElegido == 0)
                            foreach (var n in comportamiento["niveles"])
                                if (decimal.TryParse(n["valor"]?.ToString(), out var val) && val > 0) { idNivelElegido = Convert.ToInt32(n["idNivel"]); break; }

                        foreach (var nivel in comportamiento["niveles"])
                        {
                            var registro = new EvaluacionXcompetenciaModel
                            {
                                IdEvaxComp = nivel["idEvaxComp"] != null ? Convert.ToInt32(nivel["idEvaxComp"]) : 0,
                                IdEvaluacion = ultimaEvaluacionFuncionario.IdEvaluacion,
                                IdCompetencia = idCompetencia,
                                IdComportamiento = idComportamiento,
                                IdNivel = Convert.ToInt32(nivel["idNivel"]),
                                ValorObtenido = nivel["valor"] != null ? Convert.ToDecimal(nivel["valor"]) : 0m
                            };

                            _servicioMantenimientos.EvaluacionXcompetencia.ActualizarEvaluacionXCompetencia(registro);
                        }

                        _servicioMantenimientos.EvaluacionXcompetencia
                            .ActualizarNivelElegidoPorGrupo(
                                ultimaEvaluacionFuncionario.IdEvaluacion,
                                idCompetencia,
                                idComportamiento,
                                idNivelElegido
                            );
                    }
                }
                var fechaNorm = DateTime.Now.ToString("yyyyMMdd_HHmm");
                var nombreArchivo = $"seguimiento_{cedFuncionario}_{fechaNorm}.pdf";
                CrearReportePDFEvaluacion(dataEnJSon, ultimaEvaluacionFuncionario, nombreArchivo,faseActual);

                var urlArchivo = Url.Content("~/Reportes/" + cedFuncionario + "/" + nombreArchivo);
                return Json(new { ok = true, pdfUrl = urlArchivo, fileName = nombreArchivo, message = "Seguimiento guardado correctamente" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }//fin GuardarSeguimiento

        #endregion

        #region CierreEvaluacion
        public ActionResult SeleccionarSubalternoCierreEvaluacion()
        {
            try
            {
                var listaSubAlternos = _servicioMantenimientos.Funcionario.ListarSubAlternosConEvaluacionPorJefe("44444444");

                return View(listaSubAlternos);


            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Error al obtener la lista.";
                return View("Error");
            }
        }//fin

        [HttpPost]
        public ActionResult SeleccionarSubalternoCierreEvaluacion(string cedulaSeleccionada)
        {
            try
            {
                if (string.IsNullOrEmpty(cedulaSeleccionada))
                {
                    TempData["MensajeError"] = "Debe seleccionar un subalterno.";
                    return RedirectToAction("SeleccionarSubalternoCierreEvaluacion");
                }

                return RedirectToAction("ConglomeradosPorFuncCierreEvaluacion", new { cedulaSeleccionada = cedulaSeleccionada });

            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Error al seleccionar Funcionario.";
                return RedirectToAction(nameof(SeleccionarSubalternoCierreEvaluacion));
            }
        }

        public ActionResult ConglomeradosPorFuncCierreEvaluacion(string cedulaSeleccionada)
        {
            try
            {
                if (string.IsNullOrEmpty(cedulaSeleccionada))
                {
                    TempData["MensajeError"] = "Debe seleccionar un Conglomerado para el funcionario a evaluar.";
                    return RedirectToAction("SeleccionarSubalternoCierreEvaluacion");
                }

                ViewBag.cedulaSeleccionada = cedulaSeleccionada;

                var listaConglomerados = _servicioMantenimientos.Funcionario.ConglomeradosPorFunc(cedulaSeleccionada);
                return View(listaConglomerados);
            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Error al obtener la lista.";
                return RedirectToAction(nameof(SeleccionarSubalternoCierreEvaluacion));
            }

        }

        [HttpPost]
        public ActionResult SelecPeriodoPorFunCierreEvaluacion(FormCollection coleccion)
        {
            try
            {
                string cedulaSeleccionada = coleccion["cedulaSeleccionada"];
                string idConglomerado = coleccion["idConglomerado"];

                ViewBag.cedulaSeleccionada = cedulaSeleccionada;
                ViewBag.idConglomerado = idConglomerado;

                if (string.IsNullOrEmpty(idConglomerado))
                {
                    TempData["MensajeError"] = "Debe seleccionar un Conglomerado.";
                    return RedirectToAction("SeleccionarSubalternoCierreEvaluacion", new { cedulaSeleccionada = cedulaSeleccionada });
                }

                var periodos = _servicioMantenimientos.Periodos.ListarPeriodosAnioActual();
                return View(periodos);
            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Error al obtener la lista.";
                return RedirectToAction(nameof(SeleccionarSubalternoCierreEvaluacion));
            }
        }

        [HttpPost]
        public ActionResult CierreEvaluacion(FormCollection coleccion)
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

                var subalterno = _servicioMantenimientos.Funcionario.ConsultarFuncionarioID(cedulaSeleccionada);
                var PesosConglomerados = _servicioMantenimientos.Conglomerados.ConsultarPesosXConglomerado(idConglomerado);
                ViewData["ListaTiposObjetivos"] = _servicioMantenimientos.TiposObjetivos.ListarTiposObjetivos();
                ViewData["ListaTiposCompetencias"] = _servicioMantenimientos.TiposCompetencias.ListarTiposCompetencias();
                ViewData["ListaConglomerados"] = _servicioMantenimientos.Conglomerados.ListarConglomerados();

                EstadoEvaluacionModel faseActual = _servicioMantenimientos.EstadoEvaluacion.ConsultarEstadoPorID(3);

                var ultimaEvaluacionFuncionario = _servicioMantenimientos.Evaluaciones.ConsultarEvaluacionComoFuncionario(cedulaSeleccionada, idConglomerado);

                if (ultimaEvaluacionFuncionario == null)
                {
                    TempData["AlertMessage"] = "No hay una evaluación para usted en este conglomerado.Por favor contacte a su Jefatura para planificarla.";
                    return RedirectToAction("Index", "Home");
                }

                var listaObjetivos = _servicioMantenimientos.Evaluaciones.Listar_objetivosXEvaluacion(ultimaEvaluacionFuncionario.IdEvaluacion);

                var CompetenciasPlanificadas = _servicioMantenimientos.ObtenerComportamientosYDescripciones.
                    ListarComportamientosYDescripcionesNegocios(ultimaEvaluacionFuncionario.IdEvaluacion, "PorEvaluacion");

                var Transversales = CompetenciasPlanificadas
                    .Where(c => Convert.ToInt32(c.idTipoCompetencia) == 2500)
                    .ToList();

                var Competencias = CompetenciasPlanificadas
                    .Where(c => Convert.ToInt32(c.idTipoCompetencia) != 2500)
                    .ToList();

                var transversalesAgrupadas = Transversales
                    .GroupBy(x => new { x.idCompetencia, x.Competencia, x.DescriCompetencia, x.idTipoCompetencia, x.Tipo })
                    .Select(g => new CompetenciasModel
                    {
                        IdCompetencia = g.Key.idCompetencia,
                        Competencia = g.Key.Competencia,
                        Descripcion = g.Key.DescriCompetencia,
                        IdTipoCompetencia = g.Key.idTipoCompetencia,
                        TipoCompetencia = new TiposCompetenciasModel { IdTipoCompetencia = g.Key.idTipoCompetencia, Tipo = g.Key.Tipo },
                        Comportamientos = g
                            .GroupBy(k => new { k.idComport, k.Comportamiento })
                            .Select(cg => new ComportamientoModel
                            {
                                idComport = cg.Key.idComport,
                                Nombre = cg.Key.Comportamiento,
                                Niveles = cg
                                    .GroupBy(n => new { n.idNivel, n.Nivel, n.Descripcion, n.valorNivel, n.idNivelElegido }) 
                                    .Select(ng => new NivelComportamientoModel
                                    {
                                        idNivel = ng.Key.idNivel,
                                        nombre = ng.Key.Nivel,
                                        descripcion = ng.Key.Descripcion,
                                        valor = ng.Key.valorNivel,
                                        idNivelElegido = ng.Key.idNivelElegido, 
                                        idEvaxComp = ng.Select(z => z.idEvaxComp).FirstOrDefault(v => v > 0)
                                    }).ToList()
                            }).ToList()
                    }).ToList();

                var CompetenciasAgrupadas = Competencias
                 .GroupBy(x => new { x.idCompetencia, x.Competencia, x.DescriCompetencia, x.idTipoCompetencia, x.Tipo })
                 .Select(g => new Entidades.CompetenciasModel
                 {
                     IdCompetencia = g.Key.idCompetencia,
                     Competencia = g.Key.Competencia,
                     Descripcion = g.Key.DescriCompetencia,
                     IdTipoCompetencia = g.Key.idTipoCompetencia,
                     TipoCompetencia = new Entidades.TiposCompetenciasModel { IdTipoCompetencia = g.Key.idTipoCompetencia, Tipo = g.Key.Tipo },
                     Comportamientos = g
                         .GroupBy(k => new { k.idComport, k.Comportamiento })
                         .Select(cg => new Entidades.ComportamientoModel
                         {
                             idComport = cg.Key.idComport,
                             Nombre = cg.Key.Comportamiento,
                             Niveles = cg
                                 .GroupBy(n => new { n.idNivel, n.Nivel, n.Descripcion, n.valorNivel, n.idNivelElegido })
                                 .Select(ng => new Entidades.NivelComportamientoModel
                                 {
                                     idNivel = ng.Key.idNivel,
                                     nombre = ng.Key.Nivel,
                                     descripcion = ng.Key.Descripcion,
                                     valor = ng.Key.valorNivel,
                                     idNivelElegido = ng.Key.idNivelElegido,
                                     idEvaxComp = ng.Select(z => z.idEvaxComp).FirstOrDefault(v => v > 0)
                                 }).ToList()
                         }).ToList()
                 }).ToList();

                var listaNiveles = _servicioMantenimientos.NivelesComportamientos.ListarNivelesComportamientos();
                var valorPorNivel = listaNiveles.ToDictionary(n => n.idNivel, n => n.valor);

                int Val(int idNivel) => valorPorNivel.TryGetValue(idNivel, out var v) ? v : 0;

                var maxPorTransversal = (Transversales ?? Enumerable.Empty<ObtenerComportamientosYDescripcionesModel>())
                    .GroupBy(x => x.idComport)
                    .Select(g => Val(g.Max(e => e.idNivel)));

                var maxPorCompetencia = (Competencias ?? Enumerable.Empty<ObtenerComportamientosYDescripcionesModel>())
                    .GroupBy(x => x.idComport)
                    .Select(g => Val(g.Max(e => e.idNivel)));

                int MaximoPuntosCompetencias = maxPorTransversal.Sum() + maxPorCompetencia.Sum();

                ViewBag.ListaObjetivos = listaObjetivos;
                ViewBag.Transversales = transversalesAgrupadas;
                ViewBag.CompetenciasAgrupadas = CompetenciasAgrupadas;
                ViewBag.PesosConglomerados = PesosConglomerados;
                ViewBag.IdConglomerado = idConglomerado;
                ViewBag.MaximoPuntosCompetencias = MaximoPuntosCompetencias;
                ViewBag.ultimaEvaluacionFuncionario = ultimaEvaluacionFuncionario;
                ViewBag.faseActual = faseActual;
                ViewBag.idPeriodo = idPeriodo;

                return View(subalterno);
            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Error al cargar la vista para evaluar.";
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult GuardarCierreEvaluacion(string evaluacionData)
        {
            try
            {
                //Convertimos de string a Json lo que viene de la vista para tratarla
                var dataEnJSon = JsonConvert.DeserializeObject<JObject>(evaluacionData);

                var cedFuncionario = dataEnJSon["cedFuncionario"]?.ToString();
                var idConglo = dataEnJSon["idConglo"]?.ToString();
                var observaciones = dataEnJSon["observaciones"]?.ToString();

                var objetivos = dataEnJSon["objetivos"];
                var competenciasTransversales = dataEnJSon["competenciasTransversales"];
                var competencias = dataEnJSon["competencias"];

                var idPeriodo = dataEnJSon["idPeriodo"]?.ToString();
                var notaFinal = dataEnJSon["notaFinal"]?.ToString();

                EstadoEvaluacionModel faseActual = _servicioMantenimientos.EstadoEvaluacion.ConsultarEstadoPorID(3);

                //consultamos la ultima evaluacion 
                var ultimaEvaluacionFuncionario = _servicioMantenimientos.Evaluaciones.ConsultarEvaluacionComoFuncionario(cedFuncionario, Convert.ToInt32(idConglo));

                if (ultimaEvaluacionFuncionario == null)
                {
                    TempData["AlertMessage"] = "No hay una evaluación para usted en este conglomerado.Por favor contacte a su Jefatura para planificarla.";
                    return RedirectToAction("Index", "Home");
                }

                //actualizamos su estado y lo guardamos
                ultimaEvaluacionFuncionario.EstadoEvaluacion = 3; //"Estado 3 = Cierre de evaluación"
                ultimaEvaluacionFuncionario.Observaciones = observaciones;
                ultimaEvaluacionFuncionario.NotaFinal = Convert.ToDecimal(notaFinal);
                ultimaEvaluacionFuncionario.IdPeriodo = Convert.ToInt32(idPeriodo);
                _servicioMantenimientos.Evaluaciones.ModificarEvaluacion(ultimaEvaluacionFuncionario);

                // Actualizamos el actual de los obj
                foreach (var objetivo in objetivos)
                {
                    var evaluacionXObjetivo = new EvaluacionXObjetivoModel
                    {
                        IdEvaxObj = Convert.ToInt32(objetivo["idEvaxObj"]),
                        IdEvaluacion = ultimaEvaluacionFuncionario.IdEvaluacion,
                        IdObjetivo = Convert.ToInt32(objetivo["id"]),
                        ValorObtenido = Convert.ToDecimal(objetivo["actual"]),
                        Peso = Convert.ToDecimal(objetivo["peso"]),
                        Meta = objetivo["meta"].ToString()
                    };

                    _servicioMantenimientos.EvaluacionXobjetivos.ModificarEvaluacionXObjetivo(evaluacionXObjetivo);
                }
                //******************************************************************************************************************************
                // Actualizamos competencias Transversales
                foreach (var competencia in competenciasTransversales)
                {
                    var idCompetencia = Convert.ToInt32(competencia["idCompetencia"]);

                    foreach (var comportamiento in competencia["comportamientos"])
                    {
                        var idComportamiento = Convert.ToInt32(comportamiento["idComportamiento"]);

                        int idNivelElegido = 0;

                        foreach (var n in comportamiento["niveles"])
                            if (n["idNivelElegido"] != null && int.TryParse(n["idNivelElegido"]?.ToString(), out var e) && e > 0) { idNivelElegido = e; break; }

                        if (idNivelElegido == 0)
                            foreach (var n in comportamiento["niveles"])
                                if (n["idEvaxComp"] != null && int.TryParse(n["idEvaxComp"]?.ToString(), out var evx) && evx > 0) { idNivelElegido = Convert.ToInt32(n["idNivel"]); break; }

                        if (idNivelElegido == 0)
                            foreach (var n in comportamiento["niveles"])
                                if (decimal.TryParse(n["valor"]?.ToString(), out var val) && val > 0) { idNivelElegido = Convert.ToInt32(n["idNivel"]); break; }

                        foreach (var nivel in comportamiento["niveles"])
                        {
                            var registro = new EvaluacionXcompetenciaModel
                            {
                                IdEvaxComp = nivel["idEvaxComp"] != null ? Convert.ToInt32(nivel["idEvaxComp"]) : 0,
                                IdEvaluacion = ultimaEvaluacionFuncionario.IdEvaluacion,
                                IdCompetencia = idCompetencia,
                                IdComportamiento = idComportamiento,
                                IdNivel = Convert.ToInt32(nivel["idNivel"]),
                                ValorObtenido = nivel["valor"] != null ? Convert.ToDecimal(nivel["valor"]) : 0m
                            };

                            _servicioMantenimientos.EvaluacionXcompetencia.ActualizarEvaluacionXCompetencia(registro);
                        }

                        // Ponemos el elegido a TODAS las filas del grupo )osea a los niveles relacionados a esa comeptencia:
                        _servicioMantenimientos.EvaluacionXcompetencia
                            .ActualizarNivelElegidoPorGrupo(
                                ultimaEvaluacionFuncionario.IdEvaluacion,
                                idCompetencia,
                                idComportamiento,
                                idNivelElegido
                            );
                    }
                }
                //******************************************************************************************************************************
                // Actualizamos competencias (no transversales)
                foreach (var competencia in competencias)
                {
                    var idCompetencia = Convert.ToInt32(competencia["idCompetencia"]);

                    foreach (var comportamiento in competencia["comportamientos"])
                    {
                        var idComportamiento = Convert.ToInt32(comportamiento["idComportamiento"]);

                        int idNivelElegido = 0;

                        foreach (var n in comportamiento["niveles"])
                            if (n["idNivelElegido"] != null && int.TryParse(n["idNivelElegido"]?.ToString(), out var e) && e > 0) { idNivelElegido = e; break; }

                        if (idNivelElegido == 0)
                            foreach (var n in comportamiento["niveles"])
                                if (n["idEvaxComp"] != null && int.TryParse(n["idEvaxComp"]?.ToString(), out var evx) && evx > 0) { idNivelElegido = Convert.ToInt32(n["idNivel"]); break; }

                        if (idNivelElegido == 0)
                            foreach (var n in comportamiento["niveles"])
                                if (decimal.TryParse(n["valor"]?.ToString(), out var val) && val > 0) { idNivelElegido = Convert.ToInt32(n["idNivel"]); break; }

                        foreach (var nivel in comportamiento["niveles"])
                        {
                            var registro = new EvaluacionXcompetenciaModel
                            {
                                IdEvaxComp = nivel["idEvaxComp"] != null ? Convert.ToInt32(nivel["idEvaxComp"]) : 0,
                                IdEvaluacion = ultimaEvaluacionFuncionario.IdEvaluacion,
                                IdCompetencia = idCompetencia,
                                IdComportamiento = idComportamiento,
                                IdNivel = Convert.ToInt32(nivel["idNivel"]),
                                ValorObtenido = nivel["valor"] != null ? Convert.ToDecimal(nivel["valor"]) : 0m
                            };

                            _servicioMantenimientos.EvaluacionXcompetencia.ActualizarEvaluacionXCompetencia(registro);
                        }

                        _servicioMantenimientos.EvaluacionXcompetencia
                            .ActualizarNivelElegidoPorGrupo(
                                ultimaEvaluacionFuncionario.IdEvaluacion,
                                idCompetencia,
                                idComportamiento,
                                idNivelElegido
                            );
                    }
                }
                var fechaNorm = DateTime.Now.ToString("yyyyMMdd_HHmm");
                var nombreArchivo = $"cierreEvaluacion_{cedFuncionario}_{fechaNorm}.pdf";
                CrearReportePDFEvaluacion(dataEnJSon, ultimaEvaluacionFuncionario, nombreArchivo,faseActual);

                var urlArchivo = Url.Content("~/Reportes/" + cedFuncionario + "/" + nombreArchivo);
                return Json(new { ok = true, pdfUrl = urlArchivo, fileName = nombreArchivo, message = "Seguimiento guardado correctamente" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }//fin GuardarSeguimiento

        #endregion

        #region Metodos Internos
        private JsonResult CrearReportePDFEvaluacion(JObject data, EvaluacionModel eva, string nombreArchivo,EstadoEvaluacionModel faseActual)
        {
            try
            {
                var idConglo = data["idConglo"]?.ToString();
                var Conglo = _servicioMantenimientos.Conglomerados.ConsultarConglomeradoID(Convert.ToInt32(idConglo));
                var dir = Server.MapPath("~/Reportes/" + data["cedFuncionario"]?.ToString());
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                var ced = data["cedFuncionario"]?.ToString() ?? eva.IdFuncionario ?? "sincedula";
                var ruta = Path.Combine(dir, nombreArchivo);
                var incluirNivelAsignado = faseActual != null && faseActual.IdEstado != 1;

                using (var fs = new FileStream(ruta, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var doc = new Document(PageSize.LETTER, 36, 36, 36, 36))
                {
                    var w = PdfWriter.GetInstance(doc, fs);
                    doc.Open();

                    var azulCUC = new BaseColor(30, 55, 108);
                    var fTitulo = FontFactory.GetFont("Helvetica", 14, Font.BOLD);
                    var fSub = FontFactory.GetFont("Helvetica", 11, Font.BOLD);
                    var fTxt = FontFactory.GetFont("Helvetica", 10, Font.NORMAL);

                    var tblEncabezado = new PdfPTable(2) { WidthPercentage = 100 };
                    tblEncabezado.SetWidths(new float[] { 70, 30 });
                    var logoPath = Server.MapPath("~/sources/img/LogoCUCsinFondo.png");
                    Image logo = null;
                    if (System.IO.File.Exists(logoPath))
                    {
                        logo = Image.GetInstance(logoPath);
                        logo.ScaleToFit(90f, 90f);
                    }

                    var celTexto = new PdfPCell(new Phrase("Colegio Universitario de Cartago", FontFactory.GetFont("Helvetica", 20, Font.BOLD, azulCUC)))
                    {
                        Border = 0,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        PaddingTop = 10f
                    };

                    var celLogo = new PdfPCell { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, VerticalAlignment = Element.ALIGN_MIDDLE };
                    if (logo != null) celLogo.AddElement(logo);

                    tblEncabezado.AddCell(celTexto);
                    tblEncabezado.AddCell(celLogo);
                    doc.Add(tblEncabezado);

                    var cb = w.DirectContent;
                    cb.SetColorStroke(azulCUC);
                    cb.SetLineWidth(2f);
                    cb.MoveTo(doc.LeftMargin, doc.Top - 100);
                    cb.LineTo(doc.PageSize.Width - doc.RightMargin, doc.Top - 100);
                    cb.Stroke();

                    var tituloPrincipal = ((faseActual?.EstadoEvaluacion) ?? "Evaluación") + " de Evaluación";
                    doc.Add(new Paragraph("\n" + tituloPrincipal, fTitulo));
                    doc.Add(new Paragraph("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), fTxt));
                    doc.Add(Chunk.NEWLINE);

                    var tblInfo = new PdfPTable(2) { WidthPercentage = 100 };
                    tblInfo.SetWidths(new float[] { 30f, 70f });

                    if (faseActual != null && faseActual.IdEstado == 1)
                    {
                        var Funcionario = _servicioMantenimientos.Funcionario.ConsultarFuncionarioID(ced);
                        tblInfo.AddCell(new PdfPCell(new Phrase("Funcionario:", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                        var txtFun = Funcionario != null ? $"{ced} - {Funcionario.Nombre} {Funcionario.Apellido1} {Funcionario.Apellido2}" : ced;
                        tblInfo.AddCell(new PdfPCell(new Phrase(txtFun, fTxt)));
                    }
                    else
                    {
                        tblInfo.AddCell(new PdfPCell(new Phrase("Cédula:", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                        tblInfo.AddCell(new PdfPCell(new Phrase(ced, fTxt)));
                    }

                    tblInfo.AddCell(new PdfPCell(new Phrase("Id Evaluación:", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    tblInfo.AddCell(new PdfPCell(new Phrase(eva.IdEvaluacion.ToString(), fTxt)));
                    tblInfo.AddCell(new PdfPCell(new Phrase("Conglomerado:", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    tblInfo.AddCell(new PdfPCell(new Phrase(Conglo?.NombreConglomerado ?? "", fTxt)));
                    doc.Add(tblInfo);
                    doc.Add(Chunk.NEWLINE);

                    var obs = data["observaciones"]?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(obs))
                    {
                        doc.Add(new Paragraph("Observaciones", fSub));
                        doc.Add(new Paragraph(obs, fTxt));
                        doc.Add(Chunk.NEWLINE);
                    }

                    var objetivos = data["objetivos"] as JArray;
                    if (objetivos != null && objetivos.Count > 0)
                    {
                        var esPlanificacion = faseActual != null && faseActual.IdEstado == 1;
                        var tituloObjetivos = esPlanificacion ? "Objetivos" : "Resultados de Objetivos";
                        var fuenteTituloObj = esPlanificacion ? fSub : FontFactory.GetFont("Helvetica", 12, Font.BOLD | Font.UNDERLINE, azulCUC);
                        doc.Add(new Paragraph(tituloObjetivos, fuenteTituloObj) { SpacingAfter = 8f });

                        PdfPTable t;
                        if (esPlanificacion)
                        {
                            t = new PdfPTable(4) { WidthPercentage = 100 };
                            t.SpacingBefore = 3f;
                            t.SetWidths(new float[] { 8f, 48f, 24f, 20f });
                            t.AddCell(new PdfPCell(new Phrase("Id", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                            t.AddCell(new PdfPCell(new Phrase("Objetivo", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                            t.AddCell(new PdfPCell(new Phrase("Meta", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                            t.AddCell(new PdfPCell(new Phrase("Peso", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                            foreach (var o in objetivos)
                            {
                                t.AddCell(new PdfPCell(new Phrase(o["id"]?.ToString() ?? "", fTxt)));
                                t.AddCell(new PdfPCell(new Phrase(o["nombre"]?.ToString() ?? "", fTxt)));
                                t.AddCell(new PdfPCell(new Phrase(o["meta"]?.ToString() ?? "", fTxt)));
                                t.AddCell(new PdfPCell(new Phrase(o["peso"]?.ToString() ?? "", fTxt)));
                            }
                        }
                        else
                        {
                            t = new PdfPTable(5) { WidthPercentage = 100 };
                            t.SpacingBefore = 3f;
                            t.SetWidths(new float[] { 8f, 40f, 22f, 15f, 15f });
                            t.AddCell(new PdfPCell(new Phrase("Id", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                            t.AddCell(new PdfPCell(new Phrase("Objetivo", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                            t.AddCell(new PdfPCell(new Phrase("Meta", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                            t.AddCell(new PdfPCell(new Phrase("Peso", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                            t.AddCell(new PdfPCell(new Phrase("Actual", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                            foreach (var o in objetivos)
                            {
                                t.AddCell(new PdfPCell(new Phrase(o["id"]?.ToString() ?? "", fTxt)));
                                t.AddCell(new PdfPCell(new Phrase(o["nombre"]?.ToString() ?? "", fTxt)));
                                t.AddCell(new PdfPCell(new Phrase(o["meta"]?.ToString() ?? "", fTxt)));
                                t.AddCell(new PdfPCell(new Phrase(o["peso"]?.ToString() ?? "", fTxt)));
                                t.AddCell(new PdfPCell(new Phrase(o["actual"]?.ToString() ?? "", fTxt)));
                            }
                        }
                        doc.Add(t);
                        doc.Add(Chunk.NEWLINE);
                    }

                    var competenciasPlanificadas = _servicioMantenimientos.ObtenerComportamientosYDescripciones.ListarComportamientosYDescripcionesNegocios(eva.IdEvaluacion, "PorEvaluacion");
                    var transversales = competenciasPlanificadas.Where(c => Convert.ToInt32(c.idTipoCompetencia) == 2500).ToList();
                    var competenciasNoTrans = competenciasPlanificadas.Where(c => Convert.ToInt32(c.idTipoCompetencia) != 2500).ToList();
                    var transversalesAgrupadas = AgruparCompetencias(transversales, faseActual.IdEstado);
                    var competenciasAgrupadas = AgruparCompetencias(competenciasNoTrans, faseActual.IdEstado);

                    var tituloCompetencias = (faseActual != null && faseActual.IdEstado == 1) ? "Competencias del Conglomerado" : "Competencias";

                    AgregarCompetenciasAgrupadas(doc, w, "Competencias Transversales", transversalesAgrupadas, fSub, fTxt, faseActual.IdEstado);
                    AgregarCompetenciasAgrupadas(doc, w, tituloCompetencias, competenciasAgrupadas, fSub, fTxt, faseActual.IdEstado);

                    //****************************************************************************
                    //**********************Resumen FInal ****************************************
                    var resumenFinal = data["resumenFinal"] as JArray;
                    if (resumenFinal != null && resumenFinal.Count > 0)
                    {
                        var fuenteTituloResumen = FontFactory.GetFont("Helvetica", 12, Font.BOLD | Font.UNDERLINE, azulCUC);

                        // Título con salto de línea
                        doc.Add(new Paragraph("Resumen Final", fuenteTituloResumen) { SpacingAfter = 8f });
                        doc.Add(Chunk.NEWLINE);

                        var tablaResumen = new PdfPTable(3) { WidthPercentage = 80, HorizontalAlignment = Element.ALIGN_CENTER };
                        tablaResumen.SetWidths(new float[] { 50f, 25f, 25f });
                        tablaResumen.SpacingBefore = 4f;
                        tablaResumen.SpacingAfter = 6f;

                        tablaResumen.AddCell(new PdfPCell(new Phrase("Tipo", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_CENTER });
                        tablaResumen.AddCell(new PdfPCell(new Phrase("Porcentaje", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_CENTER });
                        tablaResumen.AddCell(new PdfPCell(new Phrase("Valor", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY, HorizontalAlignment = Element.ALIGN_CENTER });

                        decimal sumaPorcentaje = 0m;
                        decimal sumaValor = 0m;

                        foreach (var item in resumenFinal)
                        {
                            var tipo = item["tipo"]?.ToString() ?? "";
                            var porcentajeRaw = item["porcentaje"]?.ToString() ?? "";
                            var valorRaw = item["valor"]?.ToString() ?? "";

                            tablaResumen.AddCell(new PdfPCell(new Phrase(tipo, fTxt)));
                            tablaResumen.AddCell(new PdfPCell(new Phrase(porcentajeRaw, fTxt)) { HorizontalAlignment = Element.ALIGN_CENTER });
                            tablaResumen.AddCell(new PdfPCell(new Phrase(valorRaw, fTxt)) { HorizontalAlignment = Element.ALIGN_CENTER });

                            // Suma de porcentajes
                            var pStr = porcentajeRaw.Replace("%", "").Trim();
                            if (decimal.TryParse(pStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var pVal))
                                sumaPorcentaje += pVal;
                            else if (decimal.TryParse(pStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, out var pValLocal))
                                sumaPorcentaje += pValLocal;

                            // Suma de valores
                            if (decimal.TryParse(valorRaw, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var vVal))
                                sumaValor += vVal;
                            else if (decimal.TryParse(valorRaw, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, out var vValLocal))
                                sumaValor += vValLocal;
                        }

                        // Fila Resultado (suma total)
                        var celResultado = new PdfPCell(new Phrase("Resultado:", fSub))
                        {
                            BackgroundColor = BaseColor.LIGHT_GRAY,
                            HorizontalAlignment = Element.ALIGN_LEFT
                        };
                        tablaResumen.AddCell(celResultado);
                        tablaResumen.AddCell(new PdfPCell(new Phrase(sumaPorcentaje.ToString("0") + "%", fTxt)) { HorizontalAlignment = Element.ALIGN_CENTER });
                        tablaResumen.AddCell(new PdfPCell(new Phrase(sumaValor.ToString("0.##"), fTxt)) { HorizontalAlignment = Element.ALIGN_CENTER });

                        doc.Add(tablaResumen);
                        doc.Add(Chunk.NEWLINE);
                    }

                    //**********************************************************************************************
                    //************************* Comentario Final si es cierre ****************************************
                    if (faseActual != null && faseActual.IdEstado == 3)
                    {
                        doc.Add(Chunk.NEWLINE);

                        // ====== Comentarios Finales ======
                        var colorAzul = new BaseColor(30, 55, 108);
                        var celdaTituloComentarios = new PdfPCell(new Phrase("Comentarios Finales", FontFactory.GetFont("Helvetica", 11, Font.BOLD, BaseColor.WHITE)))
                        {
                            BackgroundColor = colorAzul,
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Padding = 6f,
                            Border = 0
                        };

                        var tablaComentarios = new PdfPTable(1) { WidthPercentage = 100 };
                        tablaComentarios.AddCell(celdaTituloComentarios);
                        tablaComentarios.AddCell(new PdfPCell(new Phrase("\n\n\n", fTxt)) { FixedHeight = 60f }); // espacio para escribir
                        doc.Add(tablaComentarios);

                        doc.Add(Chunk.NEWLINE);

                        // ====== Notificación de la Evaluación ======
                        var celdaTituloNotif = new PdfPCell(new Phrase("NOTIFICACIÓN DE LA EVALUACIÓN DEL DESEMPEÑO", FontFactory.GetFont("Helvetica", 11, Font.BOLD, BaseColor.WHITE)))
                        {
                            BackgroundColor = colorAzul,
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Padding = 6f,
                            Border = 0
                        };

                        var tablaNotif = new PdfPTable(1) { WidthPercentage = 100 };
                        tablaNotif.AddCell(celdaTituloNotif);

                        string textoNotif =
                            "Hago constar que he leído y discutido la presente evaluación de mi desempeño laboral y me doy por enterado(a) del contenido de la misma el día _____ / _____ / ______.\n\n" +
                            "_____ De acuerdo\n" +
                            "_____ En desacuerdo (proceda a completarse el formulario de manifestación de disconformidad)";

                        var celdaTextoNotif = new PdfPCell(new Phrase(textoNotif, fTxt))
                        {
                            PaddingTop = 10f,
                            PaddingBottom = 10f,
                            Border = 0
                        };

                        tablaNotif.AddCell(celdaTextoNotif);
                        doc.Add(tablaNotif);

                        doc.Add(Chunk.NEWLINE);
                    }




                    doc.Add(new Paragraph("\n\n\n"));
                    var firmas = new PdfPTable(2) { WidthPercentage = 100 };
                    firmas.SetWidths(new float[] { 50f, 50f });
                    firmas.SpacingBefore = 15f;

                    firmas.AddCell(new PdfPCell(new Phrase("Firma Jefatura:\n\n\n\n\n\n________________________________", fSub))
                    {
                        Border = 0,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        PaddingTop = 15f
                    });

                    firmas.AddCell(new PdfPCell(new Phrase("Firma Funcionario(a):\n\n\n\n\n\n____________________________", fSub))
                    {
                        Border = 0,
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        PaddingTop = 15f
                    });

                    doc.Add(firmas);
                    doc.Close();
                }
                return Json(new { success = true, message = "Reporte generado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error al generar el reporte.",
                    error = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }//fin
        private List<CompetenciasModel> AgruparCompetencias(List<ObtenerComportamientosYDescripcionesModel> competencias, int idEstado)
        {
            bool incluirIdNivelElegido = idEstado != 1;

            if (incluirIdNivelElegido)
            {
                return competencias
                    .GroupBy(x => new { x.idCompetencia, x.Competencia, x.DescriCompetencia, x.idTipoCompetencia, x.Tipo })
                    .Select(g => new CompetenciasModel
                    {
                        IdCompetencia = g.Key.idCompetencia,
                        Competencia = g.Key.Competencia,
                        Descripcion = g.Key.DescriCompetencia,
                        IdTipoCompetencia = g.Key.idTipoCompetencia,
                        TipoCompetencia = new TiposCompetenciasModel
                        {
                            IdTipoCompetencia = g.Key.idTipoCompetencia,
                            Tipo = g.Key.Tipo
                        },
                        Comportamientos = g
                            .GroupBy(k => new { k.idComport, k.Comportamiento })
                            .Select(cg => new ComportamientoModel
                            {
                                idComport = cg.Key.idComport,
                                Nombre = cg.Key.Comportamiento,
                                Niveles = cg
                                    .GroupBy(n => new { n.idNivel, n.Nivel, n.Descripcion, n.valorNivel, n.idNivelElegido })
                                    .Select(ng => new NivelComportamientoModel
                                    {
                                        idNivel = ng.Key.idNivel,
                                        nombre = ng.Key.Nivel,
                                        descripcion = ng.Key.Descripcion,
                                        valor = ng.Key.valorNivel,
                                        idNivelElegido = ng.Key.idNivelElegido,
                                        idEvaxComp = ng.Select(z => z.idEvaxComp).FirstOrDefault(v => v > 0)
                                    }).OrderBy(x => x.valor).ToList()
                            }).ToList()
                    }).ToList();
            }
            else
            {
                return competencias
                    .GroupBy(x => new { x.idCompetencia, x.Competencia, x.DescriCompetencia, x.idTipoCompetencia, x.Tipo })
                    .Select(g => new CompetenciasModel
                    {
                        IdCompetencia = g.Key.idCompetencia,
                        Competencia = g.Key.Competencia,
                        Descripcion = g.Key.DescriCompetencia,
                        IdTipoCompetencia = g.Key.idTipoCompetencia,
                        TipoCompetencia = new TiposCompetenciasModel
                        {
                            IdTipoCompetencia = g.Key.idTipoCompetencia,
                            Tipo = g.Key.Tipo
                        },
                        Comportamientos = g
                            .GroupBy(k => new { k.idComport, k.Comportamiento })
                            .Select(cg => new ComportamientoModel
                            {
                                idComport = cg.Key.idComport,
                                Nombre = cg.Key.Comportamiento,
                                Niveles = cg
                                    .GroupBy(n => new { n.idNivel, n.Nivel, n.Descripcion, n.valorNivel })
                                    .Select(ng => new NivelComportamientoModel
                                    {
                                        idNivel = ng.Key.idNivel,
                                        nombre = ng.Key.Nivel,
                                        descripcion = ng.Key.Descripcion,
                                        valor = ng.Key.valorNivel,
                                        idEvaxComp = ng.Select(z => z.idEvaxComp).FirstOrDefault(v => v > 0)
                                    }).OrderBy(x => x.valor).ToList()
                            }).ToList()
                    }).ToList();
            }
        }
        private static void AgregarCompetenciasAgrupadas(Document doc, PdfWriter writer, string tituloSeccion,
        List<CompetenciasModel> lista, Font fSub, Font fTxt, int idEstado)
            {
                if (lista == null || lista.Count == 0) return;

                var fTituloSeccion = FontFactory.GetFont("Helvetica", 18, Font.BOLD | Font.UNDERLINE, new BaseColor(30, 55, 108));
                var fCompTitulo = FontFactory.GetFont("Helvetica", 12, Font.BOLD | Font.UNDERLINE);
                var fComport = FontFactory.GetFont("Helvetica", 11, Font.BOLD);

                var titulo = new Paragraph(tituloSeccion, fTituloSeccion) { SpacingBefore = 10f, SpacingAfter = 8f };
                doc.Add(titulo);

                foreach (var comp in lista)
                {
                    var pComp = new Paragraph(comp.Competencia, fCompTitulo) { SpacingAfter = 2f };
                    doc.Add(pComp);

                    if (!string.IsNullOrWhiteSpace(comp.Descripcion))
                    {
                        var pDesc = new Paragraph(comp.Descripcion, fTxt) { SpacingAfter = 6f };
                        doc.Add(pDesc);
                    }

                    PdfPTable t;
                    if (idEstado != 1)
                    {
                        t = new PdfPTable(3) { WidthPercentage = 100 };
                        t.SetWidths(new float[] { 35f, 50f, 15f });
                        t.AddCell(new PdfPCell(new Phrase("Comportamiento", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                        t.AddCell(new PdfPCell(new Phrase("Niveles esperados", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                        t.AddCell(new PdfPCell(new Phrase("Nivel actual asignado", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    }
                    else
                    {
                        t = new PdfPTable(2) { WidthPercentage = 100 };
                        t.SetWidths(new float[] { 35f, 65f });
                        t.AddCell(new PdfPCell(new Phrase("Comportamiento", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                        t.AddCell(new PdfPCell(new Phrase("Niveles esperados", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    }

                    foreach (var compo in comp.Comportamientos)
                    {
                        var celComp = new PdfPCell(new Phrase(compo.Nombre ?? "", fComport));
                        var nivelesTexto = string.Join("\n", compo.Niveles.Select(n =>
                            (string.IsNullOrWhiteSpace(n.nombre) ? "" : n.nombre) +
                            (n.valor > 0 ? " (" + n.valor + ")" : "") +
                            (string.IsNullOrWhiteSpace(n.descripcion) ? "" : ": " + n.descripcion)
                        ));
                        var celNiv = new PdfPCell(new Phrase(nivelesTexto, fTxt));

                        t.AddCell(celComp);
                        t.AddCell(celNiv);

                        if (idEstado != 1)
                        {
                            var asignado = compo.Niveles.FirstOrDefault(n => n.idNivelElegido > 0 && n.idNivel == n.idNivelElegido);
                            if (asignado == null) asignado = compo.Niveles.FirstOrDefault(n => n.idEvaxComp > 0);
                            var textoAsignado = asignado != null
                                ? $"{asignado.nombre}{(asignado.valor > 0 ? " (" + asignado.valor + ")" : "")}"
                                : "No asignado";
                            t.AddCell(new PdfPCell(new Phrase(textoAsignado, fTxt)));
                        }
                    }

                    doc.Add(t);
                    doc.Add(new Paragraph(" ") { SpacingAfter = 6f });
                }
            }

        #endregion






    }//fin class
}