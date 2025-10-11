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
                CrearReportePDFPlanificar(dataEnJSon, evaluacionGuardada, nombreArchivo);

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

                //Obtenemos la fase de Evaluacion para pintarla de titulo
                EstadoEvaluacionModel faseActual = _servicioMantenimientos.EstadoEvaluacion.ConsultarEstadoPorID(2);


                var ultimaEvaluacionFuncionario = _servicioMantenimientos.Evaluaciones.ConsultarEvaluacionComoFuncionario(cedulaSeleccionada, idConglomerado);

                //Validamos que tenga una Evaluacion
                if (ultimaEvaluacionFuncionario == null)
                {
                    TempData["AlertMessage"] = "No hay una evaluación para usted en este conglomerado.Por favor contacte a su Jefatura para planificarla.";
                    return RedirectToAction("Index", "Home");
                }

                var listaObjetivos = _servicioMantenimientos.Evaluaciones.Listar_objetivosXEvaluacion(ultimaEvaluacionFuncionario.IdEvaluacion);


                // Traemos las competencias, comportamientos y niveles relacionados a la evaluacion
                var CompetenciasPlanificadas = _servicioMantenimientos.ObtenerComportamientosYDescripciones.ListarComportamientosYDescripcionesNegocios(ultimaEvaluacionFuncionario.IdEvaluacion, "PorEvaluacion");

                //Separamos las transversales de las otras (con LinQ)
                var Transversales = CompetenciasPlanificadas
                    .Where(c => Convert.ToInt32(c.idTipoCompetencia) == 2500)
                    .ToList();

                var Competencias = CompetenciasPlanificadas
                    .Where(c => Convert.ToInt32(c.idTipoCompetencia) != 2500)
                    .ToList();

                //agrupamos por competencia para pintar la tabla como se hizo en el js_planificar
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
                                    .GroupBy(n => new { n.idNivel, n.Nivel, n.Descripcion, n.valorNivel })
                                    .Select(ng => new NivelComportamientoModel
                                    {
                                        idNivel = ng.Key.idNivel,
                                        nombre = ng.Key.Nivel,
                                        descripcion = ng.Key.Descripcion,
                                        valor = ng.Key.valorNivel,
                                        idEvaxComp = ng.Select(z => z.idEvaxComp).FirstOrDefault(v => v > 0)
                                    }).ToList()
                            }).ToList()
                    }).ToList();

                //Agrupamos las que no son trasnversales
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
                                 .GroupBy(n => new { n.idNivel, n.Nivel, n.Descripcion, n.valorNivel })
                                 .Select(ng => new Entidades.NivelComportamientoModel
                                 {
                                     idNivel = ng.Key.idNivel,
                                     nombre = ng.Key.Nivel,
                                     descripcion = ng.Key.Descripcion,
                                     valor = ng.Key.valorNivel,
                                     idEvaxComp = ng.Select(z => z.idEvaxComp).FirstOrDefault(v => v > 0)
                                 }).ToList()
                         }).ToList()
                 }).ToList();


                //*********************************************************************
                // Aqui vamos al calcular el maximo de puntos para las competencias (valor de nivel maximo * cant de comprtamientos)
                //ejemplo : 5 comportamientos a nivel intermedio que vale 2 seria = 10

                //traemos la lista de niveles
                var listaNiveles = _servicioMantenimientos.NivelesComportamientos.ListarNivelesComportamientos();
                var valorPorNivel = listaNiveles.ToDictionary(n => n.idNivel, n => n.valor);

                int Val(int idNivel) => valorPorNivel.TryGetValue(idNivel, out var v) ? v : 0;

                //sacamos el valor de nivel mas alto agrupando por Comportamiento
                var maxPorTransversal = (Transversales ?? Enumerable.Empty<ObtenerComportamientosYDescripcionesModel>())
                    .GroupBy(x => x.idComport)
                    .Select(g => Val(g.Max(e => e.idNivel)));

                var maxPorCompetencia = (Competencias ?? Enumerable.Empty<ObtenerComportamientosYDescripcionesModel>())
                    .GroupBy(x => x.idComport)
                    .Select(g => Val(g.Max(e => e.idNivel)));

                int MaximoPuntosCompetencias = maxPorTransversal.Sum() + maxPorCompetencia.Sum();





                ////pasamos todo a la vista
                ViewBag.ListaObjetivos = listaObjetivos;
                ViewBag.Transversales = transversalesAgrupadas;
                ViewBag.CompetenciasAgrupadas = CompetenciasAgrupadas;
                ViewBag.PesosConglomerados = PesosConglomerados;
                ViewBag.IdConglomerado = idConglomerado;
                ViewBag.MaximoPuntosCompetencias = MaximoPuntosCompetencias;
                ViewBag.ultimaEvaluacionFuncionario = ultimaEvaluacionFuncionario;
                ViewBag.faseActual = faseActual;

                return View(subalterno);

            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Error al obtener las competencias.";
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

                //consultamos la ultima evaluacion 
                var ultimaEvaluacionFuncionario = _servicioMantenimientos.Evaluaciones.ConsultarEvaluacionComoFuncionario(cedFuncionario, Convert.ToInt32(idConglo));

                //actualizamos su estado y lo guardamos
                ultimaEvaluacionFuncionario.EstadoEvaluacion = 2; //"Estado 2 = Por aprobar"
                ultimaEvaluacionFuncionario.Observaciones = observaciones;
                _servicioMantenimientos.Evaluaciones.ModificarEvaluacion(ultimaEvaluacionFuncionario);

                if (ultimaEvaluacionFuncionario == null)
                {
                    TempData["AlertMessage"] = "No hay una evaluación para usted en este conglomerado.Por favor contacte a su Jefatura para planificarla.";
                    return RedirectToAction("Index", "Home");
                }


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

                //Actualizamos el nivel asignado de las competencias Transversales
                foreach (var competencia in competenciasTransversales)
                {
                    var idCompetencia = Convert.ToInt32(competencia["idCompetencia"]);
                    foreach (var comportamiento in competencia["comportamientos"])
                    {
                        var idComportamiento = Convert.ToInt32(comportamiento["idComportamiento"]);
                        foreach (var nivel in comportamiento["niveles"])
                        {
                            var idNivel = Convert.ToInt32(nivel["idNivel"]);
                            var valor = nivel["valor"] != null ? Convert.ToDecimal(nivel["valor"]) : 0m;
                            var idEvaxComp = nivel["idEvaxComp"] != null ? Convert.ToInt32(nivel["idEvaxComp"]) : 0;

                            var registro = new EvaluacionXcompetenciaModel
                            {
                                IdEvaxComp = idEvaxComp,
                                IdEvaluacion = ultimaEvaluacionFuncionario.IdEvaluacion,
                                IdCompetencia = idCompetencia,
                                IdComportamiento = idComportamiento,
                                IdNivel = idNivel,
                                ValorObtenido = valor
                            };
                                _servicioMantenimientos.EvaluacionXcompetencia.ActualizarEvaluacionXCompetencia(registro);

                        }
                    }
                }
                //Actualizamos el nivel asignado de las competencias
                foreach (var competencia in competencias)
                {
                    var idCompetencia = Convert.ToInt32(competencia["idCompetencia"]);
                    foreach (var comportamiento in competencia["comportamientos"])
                    {
                        var idComportamiento = Convert.ToInt32(comportamiento["idComportamiento"]);
                        foreach (var nivel in comportamiento["niveles"])
                        {
                            var idNivel = Convert.ToInt32(nivel["idNivel"]);
                            var valor = nivel["valor"] != null ? Convert.ToDecimal(nivel["valor"]) : 0m;
                            var idEvaxComp = nivel["idEvaxComp"] != null ? Convert.ToInt32(nivel["idEvaxComp"]) : 0;

                            var registro = new EvaluacionXcompetenciaModel
                            {
                                IdEvaxComp = idEvaxComp,
                                IdEvaluacion = ultimaEvaluacionFuncionario.IdEvaluacion,
                                IdCompetencia = idCompetencia,
                                IdComportamiento = idComportamiento,
                                IdNivel = idNivel,
                                ValorObtenido = valor
                            };
                                _servicioMantenimientos.EvaluacionXcompetencia.ActualizarEvaluacionXCompetencia(registro);
                        }
                    }
                }
                var fechaNorm = DateTime.Now.ToString("yyyyMMdd_HHmm");
                var nombreArchivo = $"seguimiento_{cedFuncionario}_{fechaNorm}.pdf";
                CrearReportePDFSeguimiento(dataEnJSon, ultimaEvaluacionFuncionario, nombreArchivo);

                var urlArchivo = Url.Content("~/Reportes/" + cedFuncionario + "/" + nombreArchivo);
                return Json(new { ok = true, pdfUrl = urlArchivo, fileName = nombreArchivo, message = "Seguimiento guardado correctamente" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }//fin GuardarSeguimiento


        #endregion



        #region AprobarComoJefe

        #endregion


        #region Metodos Internos
        private void CrearReportePDFPlanificar(JObject data, EvaluacionModel eva, string nombreArchivo)
        {
            //-------------------------------------------Datos necesarios
            var idConglo = data["idConglo"]?.ToString();
            ConglomeradoModel Conglo = _servicioMantenimientos.Conglomerados.ConsultarConglomeradoID(Convert.ToInt32(idConglo));

            FuncionarioModel Funcionario = _servicioMantenimientos.Funcionario.ConsultarFuncionarioID(data["cedFuncionario"]?.ToString());

            var dir = Server.MapPath("~/Reportes/"+ data["cedFuncionario"]?.ToString());
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var ced = data["cedFuncionario"]?.ToString() ?? eva.IdFuncionario ?? "sincedula";
            var fechaNorm = DateTime.Now.ToString("yyyyMMdd_HHmm");
            var ruta = Path.Combine(dir, nombreArchivo);

            using (var fs = new FileStream(ruta, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var doc = new Document(PageSize.LETTER, 36, 36, 36, 36))
            {
                var w = PdfWriter.GetInstance(doc, fs);
                doc.Open();

                var fTitulo = FontFactory.GetFont("Helvetica", 14, Font.BOLD);
                var fSub = FontFactory.GetFont("Helvetica", 11, Font.BOLD);
                var fTxt = FontFactory.GetFont("Helvetica", 10, Font.NORMAL);

                //----------------------------------------------------------------------Seccion titulo y Logo
                var tblEncabezado = new PdfPTable(2) { WidthPercentage = 100 };
                tblEncabezado.SetWidths(new float[] { 70, 30 });
                var logoPath = Server.MapPath("~/sources/img/LogoCUCsinFondo.png");
                Image logo = null;
                if (System.IO.File.Exists(logoPath))
                {
                    logo = Image.GetInstance(logoPath);
                    logo.ScaleToFit(90f, 90f);
                }

                var azulCUC = new BaseColor(30, 55, 108);
                var celTexto = new PdfPCell(new Phrase("Colegio Universitario de Cartago",
                    FontFactory.GetFont("Helvetica", 20, Font.BOLD, azulCUC)))
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
                cb.SetColorStroke(azulCUC); // azul institucional
                cb.SetLineWidth(2f);
                cb.MoveTo(doc.LeftMargin, doc.Top - 100);
                cb.LineTo(doc.PageSize.Width - doc.RightMargin, doc.Top - 100);
                cb.Stroke();

                //----------------------------------------------------------------------Seccion Estado y fecha
                doc.Add(new Paragraph("\nPlanificación de Evaluación", fTitulo));
                doc.Add(new Paragraph("Fecha de creación: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), fTxt));
                doc.Add(Chunk.NEWLINE);

                //----------------------------------------------------------------------Seccion Informacion Personal
                var tblInfo = new PdfPTable(2) { WidthPercentage = 100 };

                // aqui se asignan el ancho apra cada columna 
                tblInfo.SetWidths(new float[] { 30f, 70f });

                tblInfo.AddCell(new PdfPCell(new Phrase("Funcionario:", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                tblInfo.AddCell(new PdfPCell(new Phrase(ced +" - "+Funcionario.Nombre+" "+Funcionario.Apellido1+" "+Funcionario.Apellido2, fTxt)));
                tblInfo.AddCell(new PdfPCell(new Phrase("Id Evaluación:", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                tblInfo.AddCell(new PdfPCell(new Phrase(eva.IdEvaluacion.ToString(), fTxt)));
                tblInfo.AddCell(new PdfPCell(new Phrase("Conglomerado:", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                tblInfo.AddCell(new PdfPCell(new Phrase(Conglo.NombreConglomerado, fTxt)));
                doc.Add(tblInfo);
                doc.Add(Chunk.NEWLINE);

                //----------------------------------------------------------------------Seccion Observaciones
                var obs = data["observaciones"]?.ToString() ?? "";
                doc.Add(new Paragraph("Observaciones", fSub));
                doc.Add(new Paragraph(obs, fTxt));
                doc.Add(Chunk.NEWLINE);

                //------------------------------------------------------------------ Sección Objetivos
                var objetivos = data["objetivos"] as JArray;
                if (objetivos != null && objetivos.Count > 0)
                {
                    var pTitulo = new Paragraph("Objetivos", fSub);
                    pTitulo.SpacingAfter = 8f;
                    doc.Add(pTitulo);

                    var t = new PdfPTable(5) { WidthPercentage = 100 };
                    t.SpacingBefore = 3f;

                    // aqui se asignan el ancho apra cada columna id,Objetivo,Meta,Peso,Actual
                    t.SetWidths(new float[] { 10f, 40f, 30f, 10f, 10f });

                    //color de fondo para los encabezados
                    var headerColor = BaseColor.LIGHT_GRAY;

                    t.AddCell(new PdfPCell(new Phrase("Id", fSub)) { BackgroundColor = headerColor });
                    t.AddCell(new PdfPCell(new Phrase("Objetivo", fSub)) { BackgroundColor = headerColor });
                    t.AddCell(new PdfPCell(new Phrase("Meta", fSub)) { BackgroundColor = headerColor });
                    t.AddCell(new PdfPCell(new Phrase("Peso", fSub)) { BackgroundColor = headerColor });
                    t.AddCell(new PdfPCell(new Phrase("Actual", fSub)) { BackgroundColor = headerColor });

                    foreach (var o in objetivos)
                    {
                        t.AddCell(new PdfPCell(new Phrase(o["id"]?.ToString() ?? "", fTxt)));
                        t.AddCell(new PdfPCell(new Phrase(o["nombre"]?.ToString() ?? "", fTxt)));
                        t.AddCell(new PdfPCell(new Phrase(o["meta"]?.ToString() ?? "", fTxt)));
                        t.AddCell(new PdfPCell(new Phrase(o["peso"]?.ToString() ?? "", fTxt)));
                        t.AddCell(new PdfPCell(new Phrase(o["actual"]?.ToString() ?? "", fTxt)));
                    }

                    doc.Add(t);
                    doc.Add(Chunk.NEWLINE);
                }

                //----------------------- Sección Competencias (agrupadas)
                var competenciasPlanificadas = _servicioMantenimientos.ObtenerComportamientosYDescripciones
                    .ListarComportamientosYDescripcionesNegocios(eva.IdEvaluacion, "PorEvaluacion");

                var transversales = competenciasPlanificadas
                    .Where(c => Convert.ToInt32(c.idTipoCompetencia) == 2500)
                    .ToList();

                var competenciasNoTrans = competenciasPlanificadas
                    .Where(c => Convert.ToInt32(c.idTipoCompetencia) != 2500)
                    .ToList();

                var transversalesAgrupadas = transversales
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

                var competenciasAgrupadas = competenciasNoTrans
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

                AgregarCompetenciasAgrupadas(doc, w, "Competencias Transversales", transversalesAgrupadas, fSub, fTxt,false);
                AgregarCompetenciasAgrupadas(doc, w, "Competencias del Conglomerado", competenciasAgrupadas, fSub, fTxt,false);

                //------------------------------------------------------------------ Sección Firmas
                // Espacio antes de las firmas
                doc.Add(new Paragraph("\n\n\n"));

                // Sección de firmas institucionales
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
        }//fin
        private static void AgregarCompetenciasAgrupadas(Document doc, PdfWriter writer, string tituloSeccion,
            List<CompetenciasModel> lista, Font fSub, Font fTxt, bool incluirNivelAsignado)
        {
            if (lista == null || lista.Count == 0) return;

            var fTituloSeccion = FontFactory.GetFont("Helvetica", 18, Font.BOLD | Font.UNDERLINE, new BaseColor(30, 55, 108));
            var fCompTitulo = FontFactory.GetFont("Helvetica", 12, Font.BOLD | Font.UNDERLINE);
            var fComport = FontFactory.GetFont("Helvetica", 11, Font.BOLD);

            var titulo = new Paragraph(tituloSeccion, fTituloSeccion)
            {
                SpacingBefore = 10f,
                SpacingAfter = 8f
            };
            doc.Add(titulo);

            foreach (var comp in lista)
            {
                var pComp = new Paragraph(comp.Competencia, fCompTitulo)
                {
                    SpacingAfter = 2f
                };
                doc.Add(pComp);

                if (!string.IsNullOrWhiteSpace(comp.Descripcion))
                {
                    var pDesc = new Paragraph(comp.Descripcion, fTxt)
                    {
                        SpacingAfter = 6f
                    };
                    doc.Add(pDesc);
                }

                PdfPTable t;
                if (incluirNivelAsignado)
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

                    if (incluirNivelAsignado)
                    {
                        var asignado = compo.Niveles.FirstOrDefault(n => (n.idEvaxComp > 0) || (n.valor > 0));
                        var textoAsignado = asignado != null
                            ? $"{asignado.nombre}{(asignado.valor > 0 ? " (" + asignado.valor + ")" : "")}"
                            : "";
                        t.AddCell(new PdfPCell(new Phrase(textoAsignado, fTxt)));
                    }
                }

                doc.Add(t);
                doc.Add(new Paragraph(" ") { SpacingAfter = 6f });
            }
        }//fin
        private void CrearReportePDFSeguimiento(JObject data, EvaluacionModel eva, string nombreArchivo)
        {
            //-------------------------------------------Datos necesarios
            var idConglo = data["idConglo"]?.ToString();
            var Conglo = _servicioMantenimientos.Conglomerados.ConsultarConglomeradoID(Convert.ToInt32(idConglo));
            var dir = Server.MapPath("~/Reportes/" + data["cedFuncionario"]?.ToString());
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var ced = data["cedFuncionario"]?.ToString() ?? eva.IdFuncionario ?? "sincedula";
            var ruta = Path.Combine(dir, nombreArchivo);

            using (var fs = new FileStream(ruta, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var doc = new Document(PageSize.LETTER, 36, 36, 36, 36))
            {
                var w = PdfWriter.GetInstance(doc, fs);
                doc.Open();
                //----------------------------------------------------------------------Seccion titulo y Logo
                var azul = new BaseColor(30, 55, 108);
                var fTitulo = FontFactory.GetFont("Helvetica", 14, Font.BOLD);
                var fSub = FontFactory.GetFont("Helvetica", 11, Font.BOLD);
                var fTxt = FontFactory.GetFont("Helvetica", 10, Font.NORMAL);

                var tblEncabezado = new PdfPTable(2) { WidthPercentage = 100 };
                tblEncabezado.SetWidths(new float[] { 70f, 30f });
                var logoPath = Server.MapPath("~/sources/img/LogoCUCsinFondo.png");
                Image logo = null;
                if (System.IO.File.Exists(logoPath))
                {
                    logo = Image.GetInstance(logoPath);
                    logo.ScaleToFit(90f, 90f);
                }
                var celTexto = new PdfPCell(new Phrase("Colegio Universitario de Cartago", FontFactory.GetFont("Helvetica", 20, Font.BOLD, azul)))
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
                cb.SetColorStroke(azul);
                cb.SetLineWidth(2f);
                cb.MoveTo(doc.LeftMargin, doc.Top - 100);
                cb.LineTo(doc.PageSize.Width - doc.RightMargin, doc.Top - 100);
                cb.Stroke();

                //----------------------------------------------------------------------Seccion Estado y fecha
                doc.Add(new Paragraph("\nSeguimiento de Evaluación", fTitulo));
                doc.Add(new Paragraph("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), fTxt));
                doc.Add(Chunk.NEWLINE);

                //----------------------------------------------------------------------Seccion Informacion Personal
                var tblInfo = new PdfPTable(2) { WidthPercentage = 100 };
                tblInfo.SetWidths(new float[] { 30f, 70f });
                tblInfo.AddCell(new PdfPCell(new Phrase("Cédula:", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                tblInfo.AddCell(new PdfPCell(new Phrase(ced, fTxt)));
                tblInfo.AddCell(new PdfPCell(new Phrase("Id Evaluación:", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                tblInfo.AddCell(new PdfPCell(new Phrase(eva.IdEvaluacion.ToString(), fTxt)));
                tblInfo.AddCell(new PdfPCell(new Phrase("Conglomerado:", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                tblInfo.AddCell(new PdfPCell(new Phrase(Conglo.NombreConglomerado, fTxt)));
                doc.Add(tblInfo);
                doc.Add(Chunk.NEWLINE);

                //----------------------------------------------------------------------Seccion Observaciones
                var obs = data["observaciones"]?.ToString() ?? "";
                var ptObs = new Paragraph("Observaciones", fSub) { SpacingAfter = 6f };
                doc.Add(ptObs);
                doc.Add(new Paragraph(obs, fTxt));
                doc.Add(Chunk.NEWLINE);

                //----------------------------------------------------------------------Seccion Objetivos
                var objetivos = data["objetivos"] as JArray;
                if (objetivos != null && objetivos.Count > 0)
                {
                    var pTitulo = new Paragraph("Resultados de Objetivos", FontFactory.GetFont("Helvetica", 12, Font.BOLD | Font.UNDERLINE, azul)) 
                    { 
                        SpacingAfter = 8f 
                    };
                    doc.Add(pTitulo);

                    var t = new PdfPTable(5) { WidthPercentage = 100 };
                    t.SpacingBefore = 3f;
                    t.SetWidths(new float[] { 8f, 40f, 32f, 10f, 10f});

                    var header = BaseColor.LIGHT_GRAY;
                    t.AddCell(new PdfPCell(new Phrase("Id", fSub)) { BackgroundColor = header });
                    t.AddCell(new PdfPCell(new Phrase("Objetivo", fSub)) { BackgroundColor = header });
                    t.AddCell(new PdfPCell(new Phrase("Meta", fSub)) { BackgroundColor = header });
                    t.AddCell(new PdfPCell(new Phrase("Peso", fSub)) { BackgroundColor = header });
                    t.AddCell(new PdfPCell(new Phrase("Actual", fSub)) { BackgroundColor = header });

                    foreach (var o in objetivos)
                    {
                        var id = o["id"]?.ToString() ?? "";
                        var nombre = o["nombre"]?.ToString() ?? "";
                        var meta = o["meta"]?.ToString() ?? "";
                        var peso = Convert.ToDecimal(o["peso"] ?? 0m);
                        var actual = Convert.ToDecimal(o["actual"] ?? 0m);

                        t.AddCell(new PdfPCell(new Phrase(id, fTxt)));
                        t.AddCell(new PdfPCell(new Phrase(nombre, fTxt)));
                        t.AddCell(new PdfPCell(new Phrase(meta, fTxt)));
                        t.AddCell(new PdfPCell(new Phrase(peso.ToString("0.##"), fTxt)));
                        t.AddCell(new PdfPCell(new Phrase(actual.ToString("0.##"), fTxt)));
                    }

                    doc.Add(t);
                    doc.Add(Chunk.NEWLINE);
                }

                //----------------------------------------------------------------------Seccion Competencias
                var competenciasPlanificadas = _servicioMantenimientos.ObtenerComportamientosYDescripciones
                    .ListarComportamientosYDescripcionesNegocios(eva.IdEvaluacion, "PorEvaluacion");

                var transversales = competenciasPlanificadas.Where(c => Convert.ToInt32(c.idTipoCompetencia) == 2500).ToList();
                var competenciasNoTrans = competenciasPlanificadas.Where(c => Convert.ToInt32(c.idTipoCompetencia) != 2500).ToList();

                var transversalesAgrupadas = transversales
                    .GroupBy(x => new { x.idCompetencia, x.Competencia, x.DescriCompetencia })
                    .Select(g => new CompetenciasModel
                    {
                        IdCompetencia = g.Key.idCompetencia,
                        Competencia = g.Key.Competencia,
                        Descripcion = g.Key.DescriCompetencia,
                        Comportamientos = g.GroupBy(k => new { k.idComport, k.Comportamiento }).Select(cg => new ComportamientoModel
                        {
                            idComport = cg.Key.idComport,
                            Nombre = cg.Key.Comportamiento,
                            Niveles = cg.GroupBy(n => new { n.idNivel, n.Nivel, n.Descripcion, n.valorNivel })
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

                var competenciasAgrupadas = competenciasNoTrans
                    .GroupBy(x => new { x.idCompetencia, x.Competencia, x.DescriCompetencia })
                    .Select(g => new CompetenciasModel
                    {
                        IdCompetencia = g.Key.idCompetencia,
                        Competencia = g.Key.Competencia,
                        Descripcion = g.Key.DescriCompetencia,
                        Comportamientos = g.GroupBy(k => new { k.idComport, k.Comportamiento }).Select(cg => new ComportamientoModel
                        {
                            idComport = cg.Key.idComport,
                            Nombre = cg.Key.Comportamiento,
                            Niveles = cg.GroupBy(n => new { n.idNivel, n.Nivel, n.Descripcion, n.valorNivel })
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

                AgregarCompetenciasAgrupadas(doc, w, "Competencias Transversales", transversalesAgrupadas, fSub, fTxt, true);
                AgregarCompetenciasAgrupadas(doc, w, "Competencias", competenciasAgrupadas, fSub, fTxt, true);

                //------------------------------------------------------------------ Sección Firmas
                doc.Add(new Paragraph("\n\n\n"));
                var firmas = new PdfPTable(2) { WidthPercentage = 100 };
                firmas.SetWidths(new float[] { 50f, 50f });
                firmas.SpacingBefore = 15f;
                firmas.AddCell(new PdfPCell(new Phrase("Firma Jefatura: ________________________________", fSub)) { Border = 0, HorizontalAlignment = Element.ALIGN_LEFT, PaddingTop = 15f });
                firmas.AddCell(new PdfPCell(new Phrase("Firma Funcionario(a): ____________________________", fSub)) { Border = 0, HorizontalAlignment = Element.ALIGN_RIGHT, PaddingTop = 15f });
                doc.Add(firmas);

                doc.Close();
            }
        }

        #endregion






    }//fin class
}