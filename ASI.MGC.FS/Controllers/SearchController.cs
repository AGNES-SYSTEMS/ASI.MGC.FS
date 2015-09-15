using System.Data.Common;
using System.Linq;
using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;
using ASI.MGC.FS.WebCommon;

namespace ASI.MGC.FS.Controllers
{
    public class SearchController : Controller
    {
        readonly IUnitOfWork _unitOfWork;

        public SearchController()
        {
            _unitOfWork = new UnitOfWork();
        }
        // GET: Search
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search()
        {
            ViewBag.SearchTypeList = CommonModelAccessUtility.GetSearchTypeList();
            return View();
        }

        public JsonResult GetSearchDetails(string custCode, string custName, string telephone)
        {
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }
}