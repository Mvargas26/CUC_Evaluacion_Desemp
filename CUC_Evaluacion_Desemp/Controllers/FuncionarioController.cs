using Entidades;
using Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CUC_Evaluacion_Desemp.Controllers
{
    public class FuncionarioController : Controller
    {

        private readonly FuncionarioNegocios _negocios;

        //Constructor
        public FuncionarioController(FuncionarioNegocios negocios)
        {
            _negocios = negocios;
        }

        public ActionResult Index()
        {
            var lista = _negocios.ListarFuncionarios();
            return View(lista);
        }


    }
}