using System.Web.Mvc;
using ASI.MGC.FS.ExtendedAPI;

namespace ASI.MGC.FS.Controllers
{
    public class HomeController : Controller
    {
        [MesAuthorize("Admin", "Finance", "Settings", "DailyTransactions")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}