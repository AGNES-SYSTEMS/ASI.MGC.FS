using System;
using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;

namespace ASI.MGC.FS.Controllers
{
    public class SetupsController : Controller
    {
        readonly IUnitOfWork _unitOfWork;

        public SetupsController()
        {
            _unitOfWork = new UnitOfWork();
        }
        // GET: Setups

        public ActionResult Period()
        {
            return View();
        }
        public ActionResult JobCreation()
        {
            return View();
        }

        public ActionResult ProductMaster()
        {
            return View();
        }

        public ActionResult EmployeeMaster()
        {
            var objEmployeeMaster = _unitOfWork.Repository<EMPLOYEEMASTER>().Create();
            return View(objEmployeeMaster);
        }

        public ActionResult CustomerMaster()
        {
            return View();
        }

        public ActionResult SupplierMaster()
        {
            var objSupplierMaster = _unitOfWork.Repository<AR_AP_MASTER>().Create();
            return View(objSupplierMaster);
        }

        public ActionResult ItemsMaster()
        {
            var objItemMaster = _unitOfWork.Repository<PRODUCTMASTER>().Create();
            return View(objItemMaster);
        }

        public ActionResult UnitCreation()
        {
            var objUnitCreation = _unitOfWork.Repository<UNITMESSUREMENT>().Create();
            return View(objUnitCreation);
        }

        public ActionResult SaveCustomerMaster(FormCollection form, AR_AP_MASTER objCustomerMaster)
        {
            objCustomerMaster.STATUS_ARM = "AR";
            _unitOfWork.Repository<AR_AP_MASTER>().Insert(objCustomerMaster);
            _unitOfWork.Save();

            var objCustomerLedger = _unitOfWork.Repository<AR_AP_LEDGER>().Create();
            objCustomerLedger.DOCNUMBER_ART = objCustomerMaster.ARCODE_ARM;
            objCustomerLedger.DODATE_ART = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            objCustomerLedger.GLDATE_ART = Convert.ToDateTime(form["GLDate"]);
            objCustomerLedger.ARAPCODE_ART = objCustomerMaster.ARCODE_ARM;
            objCustomerLedger.DEBITAMOUNT_ART = Convert.ToInt32(form["OpeningBalance"]);
            objCustomerLedger.NARRATION_ART = "Opening Balance";
            objCustomerLedger.OTHERREF_ART = objCustomerMaster.ARCODE_ARM;
            objCustomerLedger.MATCHVALUE_AR = Convert.ToDecimal("0.0");
            objCustomerLedger.STATUS_ART = "OP";
            _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objCustomerLedger);
            _unitOfWork.Save();

            return RedirectToAction("CustomerMaster");
        }

        public ActionResult SaveJobCreation(JOBIDREFERENCE objJobCreation)
        {
            _unitOfWork.Repository<JOBIDREFERENCE>().Insert(objJobCreation);
            _unitOfWork.Save();
            return RedirectToAction("JobCreation");
        }

        public ActionResult SaveItemMaster(PRODUCTMASTER objItemMaster)
        {
            objItemMaster.STATUS_PM = "SP";
            _unitOfWork.Repository<PRODUCTMASTER>().Insert(objItemMaster);
            _unitOfWork.Save();
            return RedirectToAction("ItemsMaster");
        }

        public ActionResult SaveEmployeeMaster(EMPLOYEEMASTER objEmployeeMaster)
        {
            _unitOfWork.Repository<EMPLOYEEMASTER>().Insert(objEmployeeMaster);
            _unitOfWork.Save();
            return RedirectToAction("EmployeeMaster");
        }

        public ActionResult SaveSupplierMaster(FormCollection form, AR_AP_MASTER objSupplierMaster)
        {
            objSupplierMaster.STATUS_ARM = "AP";
            _unitOfWork.Repository<AR_AP_MASTER>().Insert(objSupplierMaster);
            _unitOfWork.Save();

            var objCustomerLedger = _unitOfWork.Repository<AR_AP_LEDGER>().Create();
            objCustomerLedger.DOCNUMBER_ART = objSupplierMaster.ARCODE_ARM;
            objCustomerLedger.DODATE_ART = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            objCustomerLedger.GLDATE_ART = Convert.ToDateTime(form["GLDate"]);
            objCustomerLedger.ARAPCODE_ART = objSupplierMaster.ARCODE_ARM;
            objCustomerLedger.DEBITAMOUNT_ART = Convert.ToInt32(form["OpeningBalance"]);
            objCustomerLedger.NARRATION_ART = "Opening Balance";
            objCustomerLedger.OTHERREF_ART = objSupplierMaster.ARCODE_ARM;
            objCustomerLedger.MATCHVALUE_AR = Convert.ToDecimal("0.0");
            objCustomerLedger.STATUS_ART = "OP";
            _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objCustomerLedger);
            _unitOfWork.Save();

            return RedirectToAction("SupplierMaster");
        }

        public ActionResult SaveProductMaster(PRODUCTMASTER objProductMaster)
        {
            objProductMaster.STATUS_PM = "IP";
            _unitOfWork.Repository<PRODUCTMASTER>().Insert(objProductMaster);
            _unitOfWork.Save();
            return RedirectToAction("ProductMaster");
        }

        public ActionResult SaveUnitMeasurement(UNITMESSUREMENT objUnitMessurement)
        {
            _unitOfWork.Repository<UNITMESSUREMENT>().Insert(objUnitMessurement);
            _unitOfWork.Save();
            return RedirectToAction("UnitCreation");
        }
    }
}