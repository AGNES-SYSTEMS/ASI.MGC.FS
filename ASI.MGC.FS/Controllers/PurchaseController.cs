using System;
using ASI.MGC.FS.Domain;
using System.Web.Mvc;
using ASI.MGC.FS.Model;

namespace ASI.MGC.FS.Controllers
{
    public class PurchaseController : Controller
    {
        readonly IUnitOfWork _unitOfWork;

        public PurchaseController()
        {
            _unitOfWork = new UnitOfWork();
        }
        // GET: Purchase
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PurchaseEntry()
        {
            return View();
        }

        public ActionResult SavePurchaseEntry(FormCollection frm)
        {
            var objArApLedger = _unitOfWork.Repository<JOBMASTER>().Create();
            objArApLedger.JOBNO_JM = Convert.ToString(frm["APCode"]);
            _unitOfWork.Repository<JOBMASTER>().Insert(objArApLedger);
            _unitOfWork.Save();
            return View("PurchaseEntry");
        }

        public ActionResult PurchaseReturn()
        {
            return View();
        }
    }
}