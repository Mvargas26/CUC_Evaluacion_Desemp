using Datos;
using Datos.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Negocios
{
    public static class DependenciasNegocios
    {
        public static void RegistrarTipos(IUnityContainer container)
        {
            container.RegisterType<IAccesoBD, AccesoBD>();
            container.RegisterType<FuncionarioNegocios>();
        }
    }
}
