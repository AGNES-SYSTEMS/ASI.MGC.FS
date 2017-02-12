using System;
using System.Linq;
using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;
using System.Net.NetworkInformation;

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
            var objJobCreation = new JOBIDREFERENCE();
            return View(objJobCreation);
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
            var objCustomerMaster = new AR_AP_MASTER();
            return View(objCustomerMaster);
        }

        public ActionResult SupplierMaster()
        {
            var objSupplierMaster = new AR_AP_MASTER();
            return View(objSupplierMaster);
        }

        public ActionResult ItemsMaster()
        {
            var objItemMaster = new PRODUCTMASTER();
            return View(objItemMaster);
        }

        public ActionResult UnitCreation()
        {
            var objUnitCreation = _unitOfWork.Repository<UNITMESSUREMENT>().Create();
            return View(objUnitCreation);
        }
        public ActionResult MesMachines()
        {
            ViewBag.currentMachineMac = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                                         where nic.OperationalStatus == OperationalStatus.Up
                                         select nic.GetPhysicalAddress().ToString()).FirstOrDefault();
            return View();
        }
        [HttpPost]
        public JsonResult SaveCustomerMaster(FormCollection form, AR_AP_MASTER objCustomerMaster)
        {
            bool success;
            try
            {
                objCustomerMaster.TYPE_ARM = "AR";
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
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            return Json(success, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("CustomerMaster");
        }
        [HttpPost]
        public JsonResult SaveJobCreation(JOBIDREFERENCE objJobCreation)
        {
            bool success;
            try
            {
                _unitOfWork.Repository<JOBIDREFERENCE>().Insert(objJobCreation);
                _unitOfWork.Save();
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }

            return Json(success, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveItemMaster(PRODUCTMASTER objItemMaster)
        {
            bool success;
            try
            {
                objItemMaster.STATUS_PM = "SP";
                _unitOfWork.Repository<PRODUCTMASTER>().Insert(objItemMaster);
                _unitOfWork.Save();
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveEmployeeMaster(EMPLOYEEMASTER objEmployeeMaster)
        {
            bool success;
            try
            {
                _unitOfWork.Repository<EMPLOYEEMASTER>().Insert(objEmployeeMaster);
                _unitOfWork.Save();
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveSupplierMaster(FormCollection form, AR_AP_MASTER objSupplierMaster)
        {
            bool success;
            try
            {
                objSupplierMaster.TYPE_ARM = "AP";
                _unitOfWork.Repository<AR_AP_MASTER>().Create();
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
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveProductMaster(PRODUCTMASTER objProductMaster)
        {
            bool success;
            try
            {
                objProductMaster.STATUS_PM = "IP";
                _unitOfWork.Repository<PRODUCTMASTER>().Insert(objProductMaster);
                _unitOfWork.Save();
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveUnitMeasurement(UNITMESSUREMENT objUnitMessurement)
        {
            bool success;
            try
            {
                _unitOfWork.Repository<UNITMESSUREMENT>().Insert(objUnitMessurement);
                _unitOfWork.Save();
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveMesMachine(MESMachine mesMachine)
        {
            bool success = false;
            bool clientExist = false;
            try
            {
                var clientMachine = (from machine in _unitOfWork.Repository<MESMachine>().Query().Get()
                                     where machine.MacAddress.Contains(mesMachine.MacAddress)
                                     select machine).SingleOrDefault();
                if (clientMachine == null)
                {
                    _unitOfWork.Repository<MESMachine>().Insert(mesMachine);
                    _unitOfWork.Save();
                    success = true;
                }
                else
                {
                    clientExist = true;
                }
            }
            catch (Exception)
            {
                success = false;
            }

            return Json(new { success = success, clientExists = clientExist }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMesMachinesList(string sidx, string sord, int page, int rows, string machineSearch, string macSearch)
        {
            var machineList = (from machines in _unitOfWork.Repository<MESMachine>().Query().Get()
                               select machines).Select(a => new { a.ID, a.MachineName, a.MacAddress, a.IsActive });
            if (!string.IsNullOrEmpty(machineSearch))
            {
                machineList = machineList.Where(a => a.MachineName.Contains(machineSearch)).Select(a => new { a.ID, a.MachineName, a.MacAddress, a.IsActive });
            }
            if (!string.IsNullOrEmpty(macSearch))
            {
                machineList = machineList.Where(a => a.MacAddress.Contains(macSearch)).Select(a => new { a.ID, a.MachineName, a.MacAddress, a.IsActive });
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = machineList.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
            if (sord.ToUpper() == "DESC")
            {
                machineList = machineList.OrderByDescending(a => a.MachineName);
                machineList = machineList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                machineList = machineList.OrderBy(a => a.MachineName);
                machineList = machineList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = machineList

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}