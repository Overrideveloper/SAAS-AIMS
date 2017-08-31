using System.Web;
using System.Web.Mvc;

namespace SAAS_AIMS
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new RollbarExceptionFilter());
        }
    }
}
