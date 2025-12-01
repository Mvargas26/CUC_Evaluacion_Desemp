using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CUC_Evaluacion_Desemp.Filters
{
    public class ValidarSesionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var contexto = HttpContext.Current;

            // Si no hay sesión de usuario
            if (contexto.Session["Cedula"] == null)
            {
                string controlador = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                string accion = filterContext.ActionDescriptor.ActionName;

                // Permitimos acceso únicamente al login y verificar código
                if (!(controlador == "Auth" &&
                     (accion == "Login" || accion == "VerificarCodigo" || accion == "RecuperarPassword")))
                {
                    filterContext.Result = new RedirectResult("~/Auth/Login");
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }//fn class
}//fin space