using Datos;
using Datos.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Negocios
{
    public static class Unix_DependenciasNegocios
    {
        public static void RegistrarTipos(IUnityContainer container)
        {
            //Registramos la interface
            container.RegisterType<IAccesoBD, AccesoBD>();

            // Obtenemod todas las clases públicas del namespace "Negocios" menos esta propia
            var tipos = Assembly.GetExecutingAssembly()
                 .GetTypes()
                 .Where(t =>
                     t.IsClass &&
                     t.IsPublic &&
                     t.Namespace == "Negocios" &&
                     t.Name != nameof(Unix_DependenciasNegocios)
                 );

            //Las recorremos y registramos en Unix para poder inyectarlas al controller
            foreach (var tipo in tipos)
            {
                container.RegisterType(tipo);
            }
        }
    }//fin class
}//fin space
