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



                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Home");
            }

        }
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








    }//fin class
}