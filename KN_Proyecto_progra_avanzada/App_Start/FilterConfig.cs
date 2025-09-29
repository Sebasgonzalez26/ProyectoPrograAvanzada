using System.Web;
using System.Web.Mvc;

namespace KN_Proyecto_progra_avanzada
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
