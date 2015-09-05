using System.Web.Mvc;

namespace ASI.MGC.FS.Controllers
{
    public class QuotationController : Controller
    {
        // GET: Quotation
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult QuotationEntry()
        {
            return View();
        }
    }
}