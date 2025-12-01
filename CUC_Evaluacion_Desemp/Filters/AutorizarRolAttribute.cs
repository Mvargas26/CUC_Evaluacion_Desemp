using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CUC_Evaluacion_Desemp.Filters
{
    public class AutorizarRolAttribute : AuthorizeAttribute
    {
        private readonly string[] _rolesPermitidos;

        public AutorizarRolAttribute(params string[] roles)
        {
            _rolesPermitidos = roles;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //  Validamos si hay sesión activa
            if (HttpContext.Current.Session["Rol"] == null &&
                FuncionarioLogueado.retornarDatosFunc() == null)
            {
                filterContext.Result = new RedirectResult("~/Auth/Login");
                return;
            }

            // 2. Obtenemos rol desde la sesión y desde FuncionarioLogueado
            string rolSesion = HttpContext.Current.Session["Rol"]?.ToString();
            string rolFuncionario = FuncionarioLogueado.retornarDatosFunc()?.Rol;

            bool autorizado = false;

            // Validar contra roles permitidos
            foreach (var rol in _rolesPermitidos)
            {
                if (rolSesion == rol || rolFuncionario == rol)
                {
                    autorizado = true;
                    break;
                }
            }

            //  Si no tiene permiso redirigir a Acceso Denegado
            if (!autorizado)
            {
                filterContext.Result = new RedirectResult("~/Home/AccesoDenegado");
            }
        }
    }
}