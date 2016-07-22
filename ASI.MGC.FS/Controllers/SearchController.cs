using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Domain.Repositories;
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

        public JsonResult GetSearchDetails(string custCode, string custName, string telephone, int searchType)
        {
            var repo = _unitOfWork.ExtRepositoryFor<ReportRepository>();
            if (searchType == 0)
            {
                var arMrvSearchDetails = repo.sp_GetARMrvDetails(custCode, custName, telephone);
                return Json(arMrvSearchDetails, JsonRequestBehavior.AllowGet);
            }
            if (searchType == 1)
            {
                var cashMrvSearchDetails = repo.sp_GetCashMrvDetails(custCode, custName, telephone);
                return Json(cashMrvSearchDetails, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }
}