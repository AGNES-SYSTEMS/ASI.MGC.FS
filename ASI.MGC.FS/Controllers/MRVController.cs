using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;

namespace ASI.MGC.FS.Controllers
{
    public class MRVController : Controller
    {
        IUnitOfWork _unitOfWork;

        public MRVController()
        {
            _unitOfWork = new UnitOfWork();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MRVCreation()
        {
            return View();
        }

        public JsonResult getCustomerName(string term)
        {
            IList<string> lstCustomersName = (from customerList in _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                                          where customerList.CUSTOMERNAME_MRV.StartsWith(term)
                                          select customerList).Distinct().Select(x => x.CUSTOMERNAME_MRV).ToList();
            return Json(lstCustomersName, JsonRequestBehavior.AllowGet);
        }
    }
}