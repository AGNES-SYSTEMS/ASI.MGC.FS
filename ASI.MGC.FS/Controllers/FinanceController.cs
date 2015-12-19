using System.Web.Mvc;
using ASI.MGC.FS.Domain;

namespace ASI.MGC.FS.Controllers
{
    public class FinanceController : Controller
    {
        readonly IUnitOfWork _unitOfWork;
        public FinanceController()
        {
            _unitOfWork = new UnitOfWork();
        }
        // GET: Finance
        public ActionResult GlCreation()
        {
            return View();
        }

        public ActionResult SaveGlCreation()
        {
            throw new System.NotImplementedException();
        }

        public ActionResult AccountsReceivable()
        {
            return View();
        }

        public ActionResult AccountsPayable()
        {
            return View();
        }

        public ActionResult SaveAccountsPayable()
        {
            throw new System.NotImplementedException();
        }

        public ActionResult SaveAccountsReceivable()
        {
            throw new System.NotImplementedException();
        }

        public ActionResult PdcReceipt()
        {
            return View();
        }

        public ActionResult JvCreation()
        {
            return View();
        }

        public ActionResult ArapMatching()
        {
            return View();
        }

        public ActionResult ArUnmatching()
        {
            return View();
        }

        public ActionResult DocumentReversal()
        {
            return View();
        }

        public ActionResult DayEndOperation()
        {
            return View();
        }

        public ActionResult SavePdcReceipt()
        {
            throw new System.NotImplementedException();
        }

        public ActionResult SaveDayEndOperation()
        {
            throw new System.NotImplementedException();
        }

        public ActionResult SaveDocumentReversal()
        {
            throw new System.NotImplementedException();
        }

        public ActionResult SaveArApMatching()
        {
            throw new System.NotImplementedException();
        }

        public ActionResult SaveArUnMatching()
        {
            throw new System.NotImplementedException();
        }

        public ActionResult SaveJvCreation()
        {
            throw new System.NotImplementedException();
        }
    }
}