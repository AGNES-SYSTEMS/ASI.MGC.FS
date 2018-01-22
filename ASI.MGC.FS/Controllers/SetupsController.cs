using System;
using System.Linq;
using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;
using System.Net.NetworkInformation;
using ASI.MGC.FS.WebCommon;

namespace ASI.MGC.FS.Controllers
{
    public class SetupsController : Controller
    {
        readonly IUnitOfWork _unitOfWork;
        readonly TimeZoneInfo tzInfo;
        DateTime today;
        public SetupsController()
        {
            _unitOfWork = new UnitOfWork();
            tzInfo = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time");
            today = TimeZoneInfo.ConvertTime(DateTime.Now, tzInfo);
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
            using (var transaction = _unitOfWork.BeginTransaction())
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
                    transaction.Commit();
                }
                catch (Exception)
                {
                    TempData["error"] = "Error: Sorry, Something went wrong!";
                    transaction.Rollback();
                }
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
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var openingBalance = Convert.ToInt32(form["OpeningBalance"]);
                    var objCustomerLedger = _unitOfWork.Repository<AR_AP_LEDGER>().Create();
                    objCustomerLedger.DOCNUMBER_ART = objCustomerMaster.ARCODE_ARM;
                    objCustomerLedger.DODATE_ART = today.Date;
                    objCustomerLedger.GLDATE_ART = Convert.ToDateTime(form["GLDate"]);
                    objCustomerLedger.ARAPCODE_ART = objCustomerMaster.ARCODE_ARM;
                    if (openingBalance >= 0)
                    {
                        objCustomerLedger.DEBITAMOUNT_ART = openingBalance;
                    }
                    else
                    {
                        objCustomerLedger.CREDITAMOUNT_ART = Math.Abs(openingBalance);
                    }
                    objCustomerLedger.NARRATION_ART = "Opening Balance";
                    objCustomerLedger.OTHERREF_ART = objCustomerMaster.ARCODE_ARM;
                    objCustomerLedger.MATCHVALUE_AR = Convert.ToDecimal("0.0");
                    objCustomerLedger.STATUS_ART = "OP";
                    objCustomerLedger.USER_ART = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);

                    var objCustomer = (from custList in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                                       where custList.ARCODE_ARM.Equals(objCustomerMaster.ARCODE_ARM)
                                       select custList).SingleOrDefault();
                    if (objCustomer != null)
                    {
                        objCustomer.ADDRESS1_ARM = objCustomerMaster.ADDRESS1_ARM;
                        objCustomer.ADDRESS2_ARM = objCustomerMaster.ADDRESS2_ARM;
                        objCustomer.ADDRESS3_ARM = objCustomerMaster.ADDRESS3_ARM;
                        objCustomer.CONDACTPERSON_ARM = objCustomerMaster.CONDACTPERSON_ARM;
                        objCustomer.CREDITDAYS_ARM = objCustomerMaster.CREDITDAYS_ARM;
                        objCustomer.DESCRIPTION_ARM = objCustomerMaster.DESCRIPTION_ARM;
                        objCustomer.EMAIL_ARM = objCustomerMaster.EMAIL_ARM;
                        objCustomer.FAX_ARM = objCustomerMaster.FAX_ARM;
                        objCustomer.LIMITAMOUNT_ARM = objCustomerMaster.LIMITAMOUNT_ARM;
                        objCustomer.Notes_ARM = objCustomerMaster.Notes_ARM;
                        objCustomer.POBOX_ARM = objCustomerMaster.POBOX_ARM;
                        objCustomer.RECEIVABLETYPE_ARM = objCustomerMaster.RECEIVABLETYPE_ARM;
                        objCustomer.SALESEXE_ARM = objCustomerMaster.SALESEXE_ARM;
                        objCustomer.STATUS_ARM = objCustomerMaster.STATUS_ARM;
                        objCustomer.TELEPHONE_ARM = objCustomerMaster.TELEPHONE_ARM;

                        objCustomer.VATNO_ARM = objCustomerMaster.VATNO_ARM;
                        _unitOfWork.Repository<AR_AP_MASTER>().Update(objCustomer);
                        _unitOfWork.Save();

                        var existingLedger = (from custLedger in _unitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                                              where custLedger.DOCNUMBER_ART.Equals(objCustomerMaster.ARCODE_ARM)
                                              select custLedger).ToList();
                        foreach (var record in existingLedger)
                        {
                            _unitOfWork.Repository<AR_AP_LEDGER>().Delete(record);
                            _unitOfWork.Save();
                        }

                        if (openingBalance > 0)
                        {
                            _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objCustomerLedger);
                            _unitOfWork.Save();
                        }
                    }
                    else
                    {
                        objCustomerMaster.TYPE_ARM = "AR";
                        _unitOfWork.Repository<AR_AP_MASTER>().Insert(objCustomerMaster);
                        _unitOfWork.Save();

                        if (openingBalance > 0)
                        {
                            _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objCustomerLedger);
                            _unitOfWork.Save();
                        }
                    }


                    success = true;
                    transaction.Commit();
                }
                catch (Exception)
                {
                    success = false;
                    transaction.Rollback();
                }
            }
            return Json(success, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("CustomerMaster");
        }
        [HttpPost]
        public JsonResult SaveJobCreation(JOBIDREFERENCE objJobCreation)
        {
            bool success;
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    _unitOfWork.Repository<JOBIDREFERENCE>().Insert(objJobCreation);
                    _unitOfWork.Save();
                    success = true;
                    transaction.Commit();
                }
                catch (Exception)
                {
                    success = false;
                    transaction.Rollback();
                }
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveItemMaster(PRODUCTMASTER objItemMaster)
        {
            bool success;
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    objItemMaster.STATUS_PM = "SP";
                    _unitOfWork.Repository<PRODUCTMASTER>().Insert(objItemMaster);
                    _unitOfWork.Save();
                    success = true;
                    transaction.Commit();
                }
                catch (Exception)
                {
                    success = false;
                    transaction.Rollback();
                }
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveEmployeeMaster(EMPLOYEEMASTER objEmployeeMaster)
        {
            bool success;
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {

                    _unitOfWork.Repository<EMPLOYEEMASTER>().Insert(objEmployeeMaster);
                    _unitOfWork.Save();
                    success = true;
                    transaction.Commit();

                }
                catch (Exception)
                {
                    transaction.Rollback();
                    success = false;
                }
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveSupplierMaster(FormCollection form, AR_AP_MASTER objSupplierMaster)
        {
            bool success;
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var openingBalance = Convert.ToInt32(form["OpeningBalance"]);
                    var objCustomerLedger = _unitOfWork.Repository<AR_AP_LEDGER>().Create();
                    objCustomerLedger.DOCNUMBER_ART = objSupplierMaster.ARCODE_ARM;
                    objCustomerLedger.DODATE_ART = today.Date;
                    objCustomerLedger.GLDATE_ART = Convert.ToDateTime(form["GLDate"]);
                    objCustomerLedger.ARAPCODE_ART = objSupplierMaster.ARCODE_ARM;
                    if (openingBalance >= 0)
                    {
                        objCustomerLedger.CREDITAMOUNT_ART = openingBalance;
                    }
                    else
                    {
                        objCustomerLedger.DEBITAMOUNT_ART = Math.Abs(openingBalance);
                    }
                    objCustomerLedger.NARRATION_ART = "Opening Balance";
                    objCustomerLedger.OTHERREF_ART = objSupplierMaster.ARCODE_ARM;
                    objCustomerLedger.MATCHVALUE_AR = Convert.ToDecimal("0.0");
                    objCustomerLedger.STATUS_ART = "OP";

                    var objSupplier = (from supplierList in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                                       where supplierList.ARCODE_ARM.Equals(objSupplierMaster.ARCODE_ARM)
                                       select supplierList).SingleOrDefault();
                    if (objSupplier != null)
                    {
                        objSupplier.ADDRESS1_ARM = objSupplierMaster.ADDRESS1_ARM;
                        objSupplier.ADDRESS2_ARM = objSupplierMaster.ADDRESS2_ARM;
                        objSupplier.ADDRESS3_ARM = objSupplierMaster.ADDRESS3_ARM;
                        objSupplier.CONDACTPERSON_ARM = objSupplierMaster.CONDACTPERSON_ARM;
                        objSupplier.CREDITDAYS_ARM = objSupplierMaster.CREDITDAYS_ARM;
                        objSupplier.DESCRIPTION_ARM = objSupplierMaster.DESCRIPTION_ARM;
                        objSupplier.EMAIL_ARM = objSupplierMaster.EMAIL_ARM;
                        objSupplier.FAX_ARM = objSupplierMaster.FAX_ARM;
                        objSupplier.LIMITAMOUNT_ARM = objSupplierMaster.LIMITAMOUNT_ARM;
                        objSupplier.Notes_ARM = objSupplierMaster.Notes_ARM;
                        objSupplier.POBOX_ARM = objSupplierMaster.POBOX_ARM;
                        objSupplier.RECEIVABLETYPE_ARM = objSupplierMaster.RECEIVABLETYPE_ARM;
                        objSupplier.SALESEXE_ARM = objSupplierMaster.SALESEXE_ARM;
                        objSupplier.STATUS_ARM = objSupplierMaster.STATUS_ARM;
                        objSupplier.TELEPHONE_ARM = objSupplierMaster.TELEPHONE_ARM;

                        objSupplier.VATNO_ARM = objSupplierMaster.VATNO_ARM;
                        _unitOfWork.Repository<AR_AP_MASTER>().Update(objSupplier);
                        _unitOfWork.Save();

                        var existingLedger = (from custLedger in _unitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                                              where custLedger.DOCNUMBER_ART.Equals(objSupplierMaster.ARCODE_ARM)
                                              select custLedger).ToList();
                        foreach (var record in existingLedger)
                        {
                            _unitOfWork.Repository<AR_AP_LEDGER>().Delete(record);
                            _unitOfWork.Save();
                        }

                        if (openingBalance > 0)
                        {
                            _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objCustomerLedger);
                            _unitOfWork.Save();
                        }
                    }
                    else
                    {
                        objSupplierMaster.TYPE_ARM = "AP";
                        _unitOfWork.Repository<AR_AP_MASTER>().Create();
                        _unitOfWork.Repository<AR_AP_MASTER>().Insert(objSupplierMaster);
                        _unitOfWork.Save();

                        if (openingBalance > 0)
                        {
                            _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objCustomerLedger);
                            _unitOfWork.Save();
                        }
                    }
                    success = true;
                    transaction.Commit();
                }
                catch (Exception)
                {
                    success = false;
                    transaction.Rollback();
                }
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveProductMaster(PRODUCTMASTER objProductMaster)
        {
            bool success;
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    objProductMaster.STATUS_PM = "IP";
                    _unitOfWork.Repository<PRODUCTMASTER>().Insert(objProductMaster);
                    _unitOfWork.Save();
                    success = true;
                    transaction.Commit();
                }
                catch (Exception)
                {
                    success = false;
                    transaction.Rollback();
                }
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveUnitMeasurement(UNITMESSUREMENT objUnitMessurement)
        {
            bool success;
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    _unitOfWork.Repository<UNITMESSUREMENT>().Insert(objUnitMessurement);
                    _unitOfWork.Save();
                    success = true;
                    transaction.Commit();
                }
                catch (Exception)
                {
                    success = false;
                    transaction.Rollback();
                }
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveMesMachine(MESMachine mesMachine)
        {
            bool success = false;
            bool clientExist = false;
            using (var transaction = _unitOfWork.BeginTransaction())
            {
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
                    transaction.Commit();
                }
                catch (Exception)
                {
                    success = false;
                    transaction.Rollback();
                }
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