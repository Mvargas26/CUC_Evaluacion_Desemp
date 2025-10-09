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

                //Aqui creamos el reporte
                CrearReportePDFPlanificar(dataEnJSon, evaluacionGuardada);

                return RedirectToAction("Index", "Home");
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

                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Home");
            }

        }//fin GuardarSeguimiento


        #endregion



        #region AprobarComoJefe

        #endregion


        #region Metodos Internos
        private void CrearReportePDFPlanificar(JObject data, EvaluacionModel eva)
        {
            //-------------------------------------------Datos necesarios
            var idConglo = data["idConglo"]?.ToString();
            ConglomeradoModel Conglo = _servicioMantenimientos.Conglomerados.ConsultarConglomeradoID(Convert.ToInt32(idConglo));

            var dir = Server.MapPath("~/Reportes/Planificaciones");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var ced = data["cedFuncionario"]?.ToString() ?? eva.IdFuncionario ?? "sincedula";
            var fechaNorm = DateTime.Now.ToString("yyyyMMdd_HHmm");
            var nombreArchivo = $"planificar_{ced}_{fechaNorm}.pdf";
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

                var celTexto = new PdfPCell(new Phrase("Colegio Universitario de Cartago", FontFactory.GetFont("Helvetica", 20, Font.BOLD)))
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
                cb.SetColorStroke(new BaseColor(30, 55, 108)); // azul institucional
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

                    t.AddCell(new PdfPCell(new Phrase("Id", fSub)));
                    t.AddCell(new PdfPCell(new Phrase("Objetivo", fSub)));
                    t.AddCell(new PdfPCell(new Phrase("Meta", fSub)));
                    t.AddCell(new PdfPCell(new Phrase("Peso", fSub)));
                    t.AddCell(new PdfPCell(new Phrase("Actual", fSub)));

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

                //----------------------------------------------------------------------Seccion Competencias
                Action<JArray, string> tablaComp = (arr, titulo) =>
                {
                    if (arr == null || arr.Count == 0) return;
                    doc.Add(new Paragraph(titulo, fSub));
                    var t = new PdfPTable(4) { WidthPercentage = 100 };
                    t.AddCell(new PdfPCell(new Phrase("IdCompetencia", fSub)));
                    t.AddCell(new PdfPCell(new Phrase("IdTipoCompetencia", fSub)));
                    t.AddCell(new PdfPCell(new Phrase("IdComportamiento", fSub)));
                    t.AddCell(new PdfPCell(new Phrase("IdNivel", fSub)));
                    foreach (var c in arr)
                    {
                        var idC = c["idCompetencia"]?.ToObject<int>() ?? 0;
                        var idTC = c["idTipoCompetencia"]?.ToObject<int>() ?? 0;
                        var comps = c["comportamientos"] as JArray;
                        if (comps == null || comps.Count == 0)
                        {
                            t.AddCell(new PdfPCell(new Phrase(idC.ToString(), fTxt)));
                            t.AddCell(new PdfPCell(new Phrase(idTC.ToString(), fTxt)));
                            t.AddCell(new PdfPCell(new Phrase("", fTxt)));
                            t.AddCell(new PdfPCell(new Phrase("", fTxt)));
                            continue;
                        }
                        foreach (var comp in comps)
                        {
                            var idComp = comp["idComportamiento"]?.ToObject<int>() ?? 0;
                            var nivs = comp["niveles"] as JArray;
                            if (nivs == null || nivs.Count == 0)
                            {
                                t.AddCell(new PdfPCell(new Phrase(idC.ToString(), fTxt)));
                                t.AddCell(new PdfPCell(new Phrase(idTC.ToString(), fTxt)));
                                t.AddCell(new PdfPCell(new Phrase(idComp.ToString(), fTxt)));
                                t.AddCell(new PdfPCell(new Phrase("", fTxt)));
                                continue;
                            }
                            foreach (var n in nivs)
                            {
                                var idN = n["idNivel"]?.ToObject<int>() ?? 0;
                                t.AddCell(new PdfPCell(new Phrase(idC.ToString(), fTxt)));
                                t.AddCell(new PdfPCell(new Phrase(idTC.ToString(), fTxt)));
                                t.AddCell(new PdfPCell(new Phrase(idComp.ToString(), fTxt)));
                                t.AddCell(new PdfPCell(new Phrase(idN.ToString(), fTxt)));
                            }
                        }
                    }
                    doc.Add(t);
                    doc.Add(Chunk.NEWLINE);
                };

                tablaComp(data["competenciasTransversales"] as JArray, "Competencias Transversales");
                tablaComp(data["competencias"] as JArray, "Competencias");

                doc.Close();
            }
        }

        #endregion






    }//fin class
}