using Entidades.AuthModels;
using Negocios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CUC_Evaluacion_Desemp.Controllers
{
    public class AuthController : Controller
    {
        // GET: Auth
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            TempData["Cedula"] = model.Cedula;
            return RedirectToAction("VerificarCodigo", "Auth");

        }

        [HttpGet]
        public ActionResult RecuperarPassword()
        {
            //return View(new RecuperarPasswordViewModel());
            return View();
        }

        [HttpGet]
        public ActionResult VerificarCodigo()
        {
            // Verifica que la cédula esté en TempData
            var cedula = TempData["Cedula"]?.ToString();
            if (cedula == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        public ActionResult VerificarCodigo(string cedula, string codigoSeguridad) 
        {
            //if (string.IsNullOrEmpty(cedula))
            //{
            //    cedula = TempData["Cedula"]?.ToString();
            //}
            //else
            //{
            //    TempData["Cedula"] = cedula;
            //}

            //if (string.IsNullOrEmpty(cedula))
            //{
            //    return RedirectToAction("Login");
            //}

            //var funcionario = _funcionarioNegocios.ConsultarFuncionarioID(cedula);

            //if (funcionario != null && funcionario.CodigoSeguridad == codigoSeguridad)
            //{
            //    var origen = TempData["Origen"]?.ToString();
               var origen = "pasa";


            //    TempData["Cedula"] = cedula; // mantener cédula en TempData

            if (origen == "Recuperar")
                {
                    return RedirectToAction("ReestablecerPassword", "Auth");
                }
                else
                {
                    TempData["MensajeExito"] = "Login exitoso";
                    return RedirectToAction("Index", "Home");
                }
            //}
            //else
            //{
            //    ModelState.AddModelError("", "Código incorrecto.");
            //    TempData["Cedula"] = cedula;
            //    return View();
            //}
        }

        
    }//fin controller
}//fin space