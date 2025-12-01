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

    }//fn class
}