using System.Web;
using System.Web.Mvc;
using ASI.MGC.FS.ExtendedAPI;

namespace ASI.MGC.FS
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new MesAuthorize("Admin", "Finance", "Settings", "DailyTransactions"));
        }
    }
}
