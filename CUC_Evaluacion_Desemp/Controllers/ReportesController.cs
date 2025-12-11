using CUC_Evaluacion_Desemp.Filters;
using Entidades;
using iTextSharp.text;
using iTextSharp.text.pdf;
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
        FuncionarioModel FuncionarioEnSesion = FuncionarioLogueado.retornarDatosFunc();

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

        [HttpPost]
        public JsonResult ExportarReportePDFGeneral(string reporteGeneralData)
        {
            try
            {
                if (string.IsNullOrEmpty(reporteGeneralData))
                    throw new Exception("No se recibieron parámetros.");

                var dataEnJson = JsonConvert.DeserializeObject<JObject>(reporteGeneralData);
                var tipoReporte = dataEnJson["tipoReporte"]?.ToString();
                var filtro = dataEnJson["filtro"]?.ToString();
                var periodo = dataEnJson["periodo"]?.ToString();

                if (string.IsNullOrEmpty(tipoReporte) || string.IsNullOrEmpty(periodo))
                    throw new Exception("Faltan parámetros obligatorios.");

                // Obtener los datos
                var resultado = _servicioMantenimientos.ReportesNegocios.ReporteGeneralRH(tipoReporte, filtro, periodo);

                // Convertir el objeto anónimo a una lista dinámica
                var detalleProperty = resultado.GetType().GetProperty("detalle");
                var detalleObj = detalleProperty.GetValue(resultado) as IEnumerable<object>;

                if (detalleObj == null)
                    throw new Exception("El detalle no tiene el formato esperado.");

                var detalle = detalleObj.Cast<dynamic>().ToList();

                if (detalle == null || detalle.Count == 0)
                    throw new Exception("No hay datos para exportar.");

                // Generar el PDF
                return CrearReportePDFReporteGeneralRH(tipoReporte, filtro, periodo, detalle.Cast<dynamic>().ToList());
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new
                {
                    success = false,
                    error = "Ocurrió un error al generar el PDF.",
                    detalle = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Reporte Por Funcioanrio

        [AutorizarRol("Jefatura", "Recursos Humanos")]
        public ActionResult SeleccionarSubalternoReporteEvaluacion()
        {
            try
            {
                var cedula = FuncionarioEnSesion?.Cedula;
                var rol = FuncionarioEnSesion?.Rol;

                if (string.IsNullOrEmpty(cedula))
                {
                    throw new Exception("No se pudo obtener la cédula del usuario en sesión.");
                }

                List<FuncionarioModel> listaSubAlternos;

                if (rol == "Recursos Humanos") 
                {
                    listaSubAlternos = _servicioMantenimientos.Funcionario.ListarSubAlternosConEvaluacionCerradasParaRH();
                }
                else
                {
                    listaSubAlternos = _servicioMantenimientos.Funcionario.ListarSubAlternosConEvaluacionCerradasPorJefe(cedula);
                }

                return View(listaSubAlternos);

            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Error al obtener la lista.";
                return View("Error");
            }
        }//fin

        [AutorizarRol("Jefatura", "Recursos Humanos")]
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

        #region Manuales de Usuario
        [HttpGet]
        public ActionResult ManualesUsuario()
        {
            try
            {
                var rutaCarpeta = Server.MapPath("~/sources/docs");

                if (!Directory.Exists(rutaCarpeta))
                {
                    ViewBag.Reportes = new List<ReportePdfViewModel>();
                    return View();
                }

                var archivos = Directory.GetFiles(rutaCarpeta, "*.pdf", SearchOption.TopDirectoryOnly);

                var listaManuales = archivos
                    .Select(fullPath => new ReportePdfViewModel
                    {
                        NombreArchivo = Path.GetFileName(fullPath),
                        RutaRelativa = Url.Content("~/sources/docs/" + Path.GetFileName(fullPath)),
                        FechaCreacion = System.IO.File.GetCreationTime(fullPath)
                    })
                    .OrderByDescending(r => r.FechaCreacion)
                    .ToList();

                ViewBag.Reportes = listaManuales;

                return View();
            }
            catch (Exception)
            {
                TempData["MensajeError"] = "Error al cargar los Manuales PDF.";
                return RedirectToAction(nameof(SeleccionarSubalternoReporteEvaluacion));
            }
        }

        #endregion

        #region Metodos Internos

        private JsonResult CrearReportePDFReporteGeneralRH(string tipoReporte, string filtro, string periodo, List<dynamic> detalle)
        {
            try
            {
                var usuarioActual = FuncionarioLogueado.retornarDatosFunc();
                var nombreUsuario = usuarioActual != null
                    ? $"{usuarioActual.Nombre} {usuarioActual.Apellido1} {usuarioActual.Apellido2}"
                    : "Usuario no identificado";

                var periodoInfo = _servicioMantenimientos.Periodos.ObtenerPeriodoID(Convert.ToInt32(periodo));

                // Obtener descripción del filtro
                string descripcionFiltro = ObtenerDescripcionFiltro(tipoReporte, filtro);

                var dir = Server.MapPath("~/Reportes/ReportesRH");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                var nombreArchivo = $"ReporteDesempeno_{tipoReporte}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                var ruta = Path.Combine(dir, nombreArchivo);

                using (var fs = new FileStream(ruta, FileMode.Create, FileAccess.Write, FileShare.None))
                using (var doc = new Document(PageSize.LETTER, 36, 36, 36, 36))
                {
                    var w = PdfWriter.GetInstance(doc, fs);
                    doc.Open();

                    var azulCUC = new BaseColor(30, 55, 108);
                    var fTitulo = FontFactory.GetFont("Helvetica", 14, Font.BOLD);
                    var fSub = FontFactory.GetFont("Helvetica", 11, Font.BOLD);
                    var fTxt = FontFactory.GetFont("Helvetica", 10, Font.NORMAL);

                    var logoPath = Server.MapPath("~/sources/img/LogoCUCsinFondo.png");
                    Image logo = null;
                    if (System.IO.File.Exists(logoPath))
                    {
                        logo = Image.GetInstance(logoPath);
                        logo.ScaleToFit(90f, 90f);
                    }

                    //**********************************Encabezado
                    var tblEncabezado = new PdfPTable(2) { WidthPercentage = 100 };
                    tblEncabezado.SetWidths(new float[] { 70, 30 });

                    var titulo = new Phrase("Colegio Universitario de Cartago\n",
                        FontFactory.GetFont("Helvetica", 20, Font.BOLD, azulCUC));

                    var subtitulo = new Phrase("Departamento de Gestión Institucional de Recursos Humanos\n",
                        FontFactory.GetFont("Helvetica", 14, Font.NORMAL, azulCUC));

                    var periodoTexto = (periodoInfo != null)
                        ? $"{periodoInfo.Nombre} ({periodoInfo.FechaInicio:dd/MM/yyyy} - {periodoInfo.FechaFin:dd/MM/yyyy})"
                        : "N/A";

                    var periodoPhrase = new Phrase(periodoTexto,
                        FontFactory.GetFont("Helvetica", 12, Font.NORMAL, azulCUC));

                    var celTexto = new PdfPCell()
                    {
                        Border = 0,
                        PaddingTop = 5f,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        VerticalAlignment = Element.ALIGN_MIDDLE
                    };

                    celTexto.AddElement(titulo);
                    celTexto.AddElement(subtitulo);
                    celTexto.AddElement(periodoPhrase);

                    var celLogo = new PdfPCell()
                    {
                        Border = 0,
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                    };

                    if (logo != null)
                        celLogo.AddElement(logo);

                    tblEncabezado.AddCell(celTexto);
                    tblEncabezado.AddCell(celLogo);

                    doc.Add(tblEncabezado);
                    tblEncabezado.SpacingAfter = 10f;

                    var cb = w.DirectContent;
                    cb.SetColorStroke(azulCUC);
                    cb.SetLineWidth(2f);
                    cb.MoveTo(doc.LeftMargin, doc.Top - 100);
                    cb.LineTo(doc.PageSize.Width - doc.RightMargin, doc.Top - 100);
                    cb.Stroke();

                    // Título del reporte
                    var tituloReporte = ObtenerTituloReporte(tipoReporte);
                    doc.Add(new Paragraph("\n" + tituloReporte, fTitulo));
                    doc.Add(new Paragraph("Fecha de generación: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), fTxt));
                    doc.Add(Chunk.NEWLINE);

                    // Información del reporte
                    var tblInfo = new PdfPTable(2) { WidthPercentage = 100 };
                    tblInfo.SetWidths(new float[] { 30f, 70f });

                    tblInfo.AddCell(new PdfPCell(new Phrase("Generado por:", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    tblInfo.AddCell(new PdfPCell(new Phrase(nombreUsuario, fTxt)));

                    tblInfo.AddCell(new PdfPCell(new Phrase("Tipo de reporte:", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    tblInfo.AddCell(new PdfPCell(new Phrase(ObtenerNombreTipoReporte(tipoReporte), fTxt)));

                    tblInfo.AddCell(new PdfPCell(new Phrase("Filtro aplicado:", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    tblInfo.AddCell(new PdfPCell(new Phrase(descripcionFiltro, fTxt)));

                    tblInfo.AddCell(new PdfPCell(new Phrase("Período:", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    tblInfo.AddCell(new PdfPCell(new Phrase(periodoTexto, fTxt)));

                    tblInfo.AddCell(new PdfPCell(new Phrase("Total registros:", fSub)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    tblInfo.AddCell(new PdfPCell(new Phrase(detalle.Count.ToString(), fTxt)));

                    doc.Add(tblInfo);
                    doc.Add(Chunk.NEWLINE);

                    // Tabla de detalle de evaluaciones
                    var fuenteTituloDetalle = FontFactory.GetFont("Helvetica", 12, Font.BOLD | Font.UNDERLINE, azulCUC);
                    doc.Add(new Paragraph("Detalle de Evaluaciones", fuenteTituloDetalle) { SpacingAfter = 8f });

                    var tablaDetalle = new PdfPTable(6) { WidthPercentage = 100 };
                    tablaDetalle.SetWidths(new float[] { 23f, 10f, 18f, 17f, 18f, 14f });
                    tablaDetalle.SpacingBefore = 3f;

                    // Encabezados
                    tablaDetalle.AddCell(new PdfPCell(new Phrase("Funcionario", fSub))
                    {
                        BackgroundColor = BaseColor.LIGHT_GRAY,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    });
                    tablaDetalle.AddCell(new PdfPCell(new Phrase("Nota Final", fSub))
                    {
                        BackgroundColor = BaseColor.LIGHT_GRAY,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    });
                    tablaDetalle.AddCell(new PdfPCell(new Phrase("Conglomerado de la Evaluación", fSub))
                    {
                        BackgroundColor = BaseColor.LIGHT_GRAY,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    });
                    tablaDetalle.AddCell(new PdfPCell(new Phrase("Nivel Desempeño", fSub))
                    {
                        BackgroundColor = BaseColor.LIGHT_GRAY,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    });
                    tablaDetalle.AddCell(new PdfPCell(new Phrase("Descripción", fSub))
                    {
                        BackgroundColor = BaseColor.LIGHT_GRAY,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    });
                    tablaDetalle.AddCell(new PdfPCell(new Phrase("Observaciones", fSub))
                    {
                        BackgroundColor = BaseColor.LIGHT_GRAY,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    });

                    // Datos
                    foreach (var item in detalle)
                    {
                        var tipo = item.GetType();

                        string funcionario = tipo.GetProperty("Funcionario")?.GetValue(item)?.ToString() ?? "";
                        string notaFinal = tipo.GetProperty("NotaFinal")?.GetValue(item)?.ToString() ?? "";
                        string conglomerado = tipo.GetProperty("NombreConglomerado")?.GetValue(item)?.ToString() ?? "";
                        string nivel = tipo.GetProperty("NivelDesempeno")?.GetValue(item)?.ToString() ?? "";
                        string descripcion = tipo.GetProperty("DescripcionRubro")?.GetValue(item)?.ToString() ?? "";
                        string observaciones = tipo.GetProperty("Observaciones")?.GetValue(item)?.ToString() ?? "";

                        tablaDetalle.AddCell(new PdfPCell(new Phrase(funcionario, fTxt)));

                        tablaDetalle.AddCell(new PdfPCell(new Phrase(notaFinal, fTxt))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER
                        });
                        tablaDetalle.AddCell(new PdfPCell(new Phrase(conglomerado, fTxt))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER
                        });
                        tablaDetalle.AddCell(new PdfPCell(new Phrase(nivel, fTxt))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER
                        });

                        tablaDetalle.AddCell(new PdfPCell(new Phrase(descripcion,
                            FontFactory.GetFont("Helvetica", 8, Font.NORMAL))));

                        tablaDetalle.AddCell(new PdfPCell(new Phrase(observaciones,
                            FontFactory.GetFont("Helvetica", 8, Font.NORMAL))));
                    }


                    doc.Add(tablaDetalle);
                    doc.Add(Chunk.NEWLINE);
                    doc.Add(Chunk.NEWLINE);

                    // Firma del responsable
                    doc.Add(new Paragraph("\n\n\n"));
                    var firmas = new PdfPTable(1) { WidthPercentage = 50, HorizontalAlignment = Element.ALIGN_CENTER };
                    firmas.SpacingBefore = 25f;

                    firmas.AddCell(new PdfPCell(new Phrase("Firma Responsable:\n\n\n\n\n\n________________________________", fSub))
                    {
                        Border = 0,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        PaddingTop = 15f
                    });

                    doc.Add(firmas);
                    doc.Close();
                }

                return Json(new
                {
                    success = true,
                    message = "Reporte generado correctamente.",
                    nombreArchivo = nombreArchivo,
                    rutaDescarga = Url.Content($"~/Reportes/ReportesRH/{nombreArchivo}")
                });
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
        }//fn

        // Métodos auxiliares al reporte
        private string ObtenerTituloReporte(string tipoReporte)
        {
            switch (tipoReporte.ToLower())
            {
                case "departamento":
                    return "Reporte de Evaluación de Desempeño por Departamento";
                case "funcionario":
                    return "Reporte de Evaluación de Desempeño Individual";
                case "conglomerado":
                    return "Reporte de Evaluación de Desempeño por Conglomerado";
                case "puesto":
                    return "Reporte de Evaluación de Desempeño por Puesto";
                default:
                    return "Reporte de Evaluación de Desempeño";
            }
        }

        private string ObtenerNombreTipoReporte(string tipoReporte)
        {
            switch (tipoReporte.ToLower())
            {
                case "departamento": return "Departamento";
                case "funcionario": return "Funcionario";
                case "conglomerado": return "Conglomerado";
                case "puesto": return "Puesto";
                default: return tipoReporte;
            }
        }

        private string ObtenerDescripcionFiltro(string tipoReporte, string filtro)
        {
            switch (tipoReporte.ToLower())
            {
                case "departamento":
                    var dep = _servicioMantenimientos.Dependencias.ConsultarDependenciaID(Convert.ToInt32(filtro));
                    return dep?.Dependencia ?? filtro;
                case "funcionario":
                    var func = _servicioMantenimientos.Funcionario.ConsultarFuncionarioID(filtro);
                    return func != null ? $"{func.Nombre} {func.Apellido1} {func.Apellido2} ({filtro})" : filtro;
                case "conglomerado":
                    var cong = _servicioMantenimientos.Conglomerados.ConsultarConglomeradoID(Convert.ToInt32(filtro));
                    return cong?.NombreConglomerado ?? filtro;
                case "puesto":
                    var puesto = _servicioMantenimientos.Puestos.ConsultarPuestoID(Convert.ToInt32(filtro));
                    return puesto?.Puesto ?? filtro;
                default:
                    return filtro;
            }
        }
        #endregion
    }//fin class
}//fin Controller