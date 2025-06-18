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
        // -------------------------------------------------VARIABLES
        private readonly IMantenimientosService _servicioMantenimientos;

        //Constructor
        public MantenimientosController(IMantenimientosService servicio)
        {
            _servicioMantenimientos = servicio;
        }

        // GET: Mantenimientos
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
                var puestos = _servicioMantenimientos.Puestos.ObtenerPuestos();
                var conglomerados = _servicioMantenimientos.Conglomerados.ListarConglomerados();
                var departamentos = _servicioMantenimientos.Departamentos.ListarDepartamentos();
                var roles = _servicioMantenimientos.Roles.ListarRoles(); 
                var funcionario = new FuncionarioModel();
                var estadosFunc = _servicioMantenimientos.EstadoFuncionariosNegocios.ListarEstadosFuncionario();

                FuncionarioViewModel newFuncionarioViewModel = new FuncionarioViewModel
                {
                    Funcionario = funcionario,
                    Puestos = puestos,
                    Conglomerados = conglomerados,
                    Departamentos = departamentos,
                    Roles = roles,
                    EstadosFuncionario = estadosFunc
                };


                return View(newFuncionarioViewModel);

            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al cargar la vista: {ex.Message}";
                return View();
            }
        }

        #endregion


    }//fin class
}// fin space