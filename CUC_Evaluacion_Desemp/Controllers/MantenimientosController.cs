using Negocios;
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
        private readonly FuncionarioNegocios _funcionarioNegocios;


        //Constructor
        public MantenimientosController(FuncionarioNegocios funcionarioNegocios)
        {
            _funcionarioNegocios = funcionarioNegocios;

        }

        // GET: Mantenimientos
        public ActionResult Index()
        {
            return View();
        }


        #region mantenimiento Funcionarios

        [Authorize(Roles = "Administración")]
        public ActionResult ManteniFuncionarios()
        {
            var lista = _funcionarioNegocios.ListarFuncionarios();
            return View(lista);
        }



        #endregion


    }//fin class
}// fin space