using CUC_Evaluacion_Desemp.Filters;
using System.Web;
using System.Web.Mvc;

namespace CUC_Evaluacion_Desemp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            // Filtro global de sesión para que funcione la segurdad por URL
            filters.Add(new ValidarSesionAttribute());

            //Filtro global para uqe no guarde el cache de las vistas protegidas
            filters.Add(new NoCacheAttribute());
        }
    }
}
