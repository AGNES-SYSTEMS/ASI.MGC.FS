using System;
using System.Linq;
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

        #region Financial Year
        public JsonResult GetAllFinancialYears(string sidx, string sord, int page, int rows)
        {
            var allFinancialYears = (from financialYears in _unitOfWork.Repository<FINYEARMASTER>().Query().Get()
                                     select financialYears).Select(a => new { a.PERRIEDFROM_FM, a.PERRIEDRTO_FM, a.CURRENTPERIOD_FM });

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = allFinancialYears.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
            if (sord.ToUpper() == "DESC")
            {
                allFinancialYears = allFinancialYears.OrderByDescending(a => a.PERRIEDFROM_FM);
                allFinancialYears = allFinancialYears.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                allFinancialYears = allFinancialYears.OrderBy(a => a.PERRIEDFROM_FM);
                allFinancialYears = allFinancialYears.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = allFinancialYears

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FinancialYearPeriod()
        {
            var currentFyear = GetCurrentFinancialYear();
            ViewBag.CurrentFYFrom = currentFyear != null ? Convert.ToDateTime(currentFyear.PERRIEDFROM_FM).ToShortDateString() : DateTime.Now.ToShortDateString();
            ViewBag.CurrentFYTo = currentFyear != null ? Convert.ToDateTime(currentFyear.PERRIEDRTO_FM).ToShortDateString() : DateTime.Now.ToShortDateString();
            return View();
        }
        public ActionResult SaveNewFinancialYear(FINYEARMASTER financialYear)
        {
            try
            {
                var currentFinancialYear = GetCurrentFinancialYear();
                currentFinancialYear.CURRENTPERIOD_FM = false;
                financialYear.ARAPSTATUS_FM = true;
                financialYear.BANKSTATUS_FM = true;
                financialYear.GLSTATUS_FM = true;
                financialYear.CURRENTPERIOD_FM = true;
                _unitOfWork.Repository<FINYEARMASTER>().Update(currentFinancialYear);
                _unitOfWork.Save();
                _unitOfWork.Repository<FINYEARMASTER>().Insert(financialYear);
                _unitOfWork.Save();
                TempData["success"] = "New Financial Year is added successfully!";
            }
            catch (Exception)
            {
                TempData["error"] = "Error: Sorry, Something went wrong!";
            }
            return RedirectToAction("FinancialYearPeriod");
        }

        public FINYEARMASTER GetCurrentFinancialYear()
        {
            var currFinancialYear = (from financialYears in _unitOfWork.Repository<FINYEARMASTER>().Query().Get()
                                     where financialYears.CURRENTPERIOD_FM == true
                                     select financialYears).FirstOrDefault();
            return currFinancialYear;
        }

        #endregion

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
            try
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
            }
            catch (Exception)
            {
                // ignored
            }
            return RedirectToAction("CustomerMaster");
        }

        public ActionResult SaveJobCreation(JOBIDREFERENCE objJobCreation)
        {
            try
            {
                _unitOfWork.Repository<JOBIDREFERENCE>().Insert(objJobCreation);
                _unitOfWork.Save();
            }
            catch (Exception)
            {
                // ignored
            }

            return RedirectToAction("JobCreation");
        }

        public ActionResult SaveItemMaster(PRODUCTMASTER objItemMaster)
        {
            try
            {
                objItemMaster.STATUS_PM = "SP";
                _unitOfWork.Repository<PRODUCTMASTER>().Insert(objItemMaster);
                _unitOfWork.Save();
            }
            catch (Exception)
            {
                // ignored
            }
            return RedirectToAction("ItemsMaster");
        }

        public ActionResult SaveEmployeeMaster(EMPLOYEEMASTER objEmployeeMaster)
        {
            try
            {
                _unitOfWork.Repository<EMPLOYEEMASTER>().Insert(objEmployeeMaster);
                _unitOfWork.Save();
            }
            catch (Exception)
            {
                // ignored
            }
            return RedirectToAction("EmployeeMaster");
        }

        public ActionResult SaveSupplierMaster(FormCollection form, AR_AP_MASTER objSupplierMaster)
        {
            try
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
            }
            catch (Exception)
            {
                // ignored
            }
            return RedirectToAction("SupplierMaster");
        }

        public ActionResult SaveProductMaster(PRODUCTMASTER objProductMaster)
        {
            try
            {
                objProductMaster.STATUS_PM = "IP";
                _unitOfWork.Repository<PRODUCTMASTER>().Insert(objProductMaster);
                _unitOfWork.Save();
            }
            catch (Exception)
            {
                // ignored
            }
            return RedirectToAction("ProductMaster");
        }

        public ActionResult SaveUnitMeasurement(UNITMESSUREMENT objUnitMessurement)
        {
            try
            {
                _unitOfWork.Repository<UNITMESSUREMENT>().Insert(objUnitMessurement);
                _unitOfWork.Save();
            }
            catch (Exception)
            {
                // ignored
            }
            return RedirectToAction("UnitCreation");
        }
    }
}