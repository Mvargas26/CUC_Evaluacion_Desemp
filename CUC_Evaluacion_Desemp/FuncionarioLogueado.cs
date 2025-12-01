using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CUC_Evaluacion_Desemp
{
    public static class FuncionarioLogueado
    {
        private static FuncionarioModel _funcionarioActual;

        public static void capturarDatosFunc(FuncionarioModel funcionario)
        {
            _funcionarioActual = funcionario;
        }

        public static FuncionarioModel retornarDatosFunc()
        {
            return _funcionarioActual;
        }

        public static void CerrarSesion()
        {
            _funcionarioActual = null;
        }

        #region Verificar el Rol
            public static bool TieneRol(string rolRequerido)
            {
                try
                {
                    var func = retornarDatosFunc();

                    // Validación por objeto
                    if (func != null && !string.IsNullOrEmpty(func.Rol) && func.Rol == rolRequerido)
                        return true;

                    // Validación por Session
                    if (HttpContext.Current.Session["Rol"] != null &&
                        HttpContext.Current.Session["Rol"].ToString() == rolRequerido)
                        return true;

                    return false;
                }
                catch
                {
                    return false;
                }
            }

            public static bool EsAdministrador()
            {
                return TieneRol("Administración");
            }

            public static bool EsJefatura()
            {
                return TieneRol("Jefatura");
            }

            public static bool EsSubalterno()
            {
                return TieneRol("Sub-Alterno");
            }

            public static bool EsRecursosHumanos()
            {
                return TieneRol("Recursos Humanos");
            }
        #endregion

    }//fn class
}