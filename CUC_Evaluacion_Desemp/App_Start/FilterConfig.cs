using System.Web;
using System.Web.Mvc;

namespace CUC_Evaluacion_Desemp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
