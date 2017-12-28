using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Domain.Repositories;
using ASI.MGC.FS.WebCommon;
using System;

namespace ASI.MGC.FS.Controllers
{
    public class SearchController : Controller
    {
        readonly IUnitOfWork _unitOfWork;
        readonly TimeZoneInfo timeZoneInfo;
        public SearchController()
        {
            _unitOfWork = new UnitOfWork();
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time");
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

        public JsonResult GetSearchDetails(string custCode, string custName, string telephone, int searchType, string mrvNo = null, string jobNo = null)
        {
            var repo = _unitOfWork.ExtRepositoryFor<ReportRepository>();
            if (searchType == 0)
            {
                var arMrvSearchDetails = repo.sp_GetARMrvDetails(custCode, custName, telephone, mrvNo, jobNo);
                return Json(arMrvSearchDetails, JsonRequestBehavior.AllowGet);
            }
            if (searchType == 1)
            {
                var cashMrvSearchDetails = repo.sp_GetCashMrvDetails(custCode, custName, telephone, mrvNo, jobNo);
                return Json(cashMrvSearchDetails, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }
}