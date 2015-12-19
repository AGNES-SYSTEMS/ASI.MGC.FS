using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;
using ASI.MGC.FS.Models;

namespace ASI.MGC.FS.Controllers
{
    public class BankController : Controller
    {
        readonly IUnitOfWork _unitOfWork;
        public BankController()
        {
            _unitOfWork = new UnitOfWork();
        }
        // GET: Bank
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BankReceipt()
        {
            var objBankTransaction = new BANKTRANSACTION();
            return View(objBankTransaction);
        }

        public ActionResult BankPayment()
        {
            var objBankTransaction = new BANKTRANSACTION();
            return View(objBankTransaction);
        }
        public JsonResult GetBankDetailsList(string sidx, string sord, int page, int rows, string bankCode, string bankName)
        {
            var bankList = (from banks in _unitOfWork.Repository<BANKMASTER>().Query().Get()
                            select banks).Select(a => new { a.BANKCODE_BM, a.BANKNAME_BM });
            if (!string.IsNullOrEmpty(bankCode))
            {
                bankList = (from jobs in _unitOfWork.Repository<BANKMASTER>().Query().Get()
                            where jobs.BANKCODE_BM.Contains(bankCode)
                            select jobs).Select(a => new { a.BANKCODE_BM, a.BANKNAME_BM });
            }
            if (!string.IsNullOrEmpty(bankName))
            {
                bankList = (from jobs in _unitOfWork.Repository<BANKMASTER>().Query().Get()
                            where jobs.BANKCODE_BM.Contains(bankName)
                            select jobs).Select(a => new { a.BANKCODE_BM, a.BANKNAME_BM });
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = bankList.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
            if (sord.ToUpper() == "DESC")
            {
                bankList = bankList.OrderByDescending(a => a.BANKCODE_BM);
                bankList = bankList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                bankList = bankList.OrderBy(a => a.BANKCODE_BM);
                bankList = bankList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = bankList

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBankDetailsByType(string sidx, string sord, int page, int rows, string bankType)
        {
            var bankList = (from banks in _unitOfWork.Repository<BANKMASTER>().Query().Get()
                            select banks).Select(a => new { a.BANKCODE_BM, a.BANKNAME_BM });
            if (!string.IsNullOrEmpty(bankType))
            {
                bankList = (from jobs in _unitOfWork.Repository<BANKMASTER>().Query().Get()
                            where jobs.MODE_BM.Equals(bankType)
                            select jobs).Select(a => new { a.BANKCODE_BM, a.BANKNAME_BM });
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = bankList.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
            if (sord.ToUpper() == "DESC")
            {
                bankList = bankList.OrderByDescending(a => a.BANKCODE_BM);
                bankList = bankList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                bankList = bankList.OrderBy(a => a.BANKCODE_BM);
                bankList = bankList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = bankList

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveBankReceipt(FormCollection form, BANKTRANSACTION objBankTransaction)
        {
            objBankTransaction.STATUS_BT = "P";
            objBankTransaction.MASTERSTATUS_BT = "M";
            _unitOfWork.Repository<BANKTRANSACTION>().Insert(objBankTransaction);
            _unitOfWork.Save();

            var objVoucherMaster = _unitOfWork.Repository<VOUCHERMASTER_RPT>().Create();
            objVoucherMaster.GLDATE_VRPT = objBankTransaction.GLDATE_BT;
            objVoucherMaster.ALLCODE_VRPT = "Bank";
            objVoucherMaster.BANKCODE_VRT = objBankTransaction.BANKCODE_BT;
            objVoucherMaster.ACCDESCRIPTION_VRPT = Convert.ToString(form["BankName"]);
            objVoucherMaster.PARTICULARS_VRPT = objBankTransaction.NARRATION_BT;
            objVoucherMaster.NOTES_VRPT = objBankTransaction.NOTE_BT;
            objVoucherMaster.DEBITAMOUT_VRPT = objVoucherMaster.DEBITAMOUT_VRPT;
            objVoucherMaster.CHQNO_VRPT = objBankTransaction.CHQNO_BT;
            objVoucherMaster.CHQDATE_VRPT = objBankTransaction.CHQDATE_BT;
            objVoucherMaster.DOCNO_VRPT = objBankTransaction.DOCNUMBER_BT;
            _unitOfWork.Repository<VOUCHERMASTER_RPT>().Insert(objVoucherMaster);
            _unitOfWork.Save();

            string jsonAllocDetails = form["allocDetails"];
            var serializer = new JavaScriptSerializer();
            var lstAllocDetails = serializer.Deserialize<List<CustomAllocationDetails>>(jsonAllocDetails);
            foreach (var allocDetail in lstAllocDetails)
            {
                switch (allocDetail.AlCode)
                {
                    case "AP":
                        var objApLedger = _unitOfWork.Repository<AR_AP_LEDGER>().Create();
                        objApLedger.DOCNUMBER_ART = objBankTransaction.DOCNUMBER_BT;
                        objApLedger.DODATE_ART = objBankTransaction.DOCDATE_BT;
                        objApLedger.GLDATE_ART = objBankTransaction.GLDATE_BT;
                        objApLedger.ARAPCODE_ART = allocDetail.AccountCode;
                        objApLedger.CREDITAMOUNT_ART = Convert.ToInt32(allocDetail.Amount);
                        objApLedger.OTHERREF_ART = objBankTransaction.OTHERREF_BT;
                        objApLedger.NARRATION_ART = allocDetail.Narration;
                        objApLedger.MATCHVALUE_AR = 0;
                        objApLedger.STATUS_ART = "P";
                        _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objApLedger);
                        _unitOfWork.Save();

                        var objApVoucherChild = _unitOfWork.Repository<VOUCHERCHILD_RPT>().Create();
                        objApVoucherChild.ALLCODE_VCD = "GL";
                        objApVoucherChild.NARRATION_VCD = objBankTransaction.NARRATION_BT;
                        objApVoucherChild.ACCODE_VCD = allocDetail.AccountCode;
                        objApVoucherChild.ACDESCRIPTION_VCD = allocDetail.Description;
                        objApVoucherChild.CREDITAMOUNT_VCD = Convert.ToDecimal(allocDetail.Amount);
                        objApVoucherChild.AMOUNTSTATUTS_VCD = "Cr";
                        objApVoucherChild.DOCNO_VCD = objBankTransaction.DOCNUMBER_BT;
                        _unitOfWork.Repository<VOUCHERCHILD_RPT>().Insert(objApVoucherChild);
                        _unitOfWork.Save();
                        break;
                    case "AR":
                        var objArLedger = _unitOfWork.Repository<AR_AP_LEDGER>().Create();
                        objArLedger.DOCNUMBER_ART = objBankTransaction.DOCNUMBER_BT;
                        objArLedger.DODATE_ART = objBankTransaction.DOCDATE_BT;
                        objArLedger.GLDATE_ART = objBankTransaction.GLDATE_BT;
                        objArLedger.ARAPCODE_ART = allocDetail.AccountCode;
                        objArLedger.CREDITAMOUNT_ART = Convert.ToInt32(allocDetail.Amount);
                        objArLedger.OTHERREF_ART = objBankTransaction.OTHERREF_BT;
                        objArLedger.NARRATION_ART = allocDetail.Narration;
                        objArLedger.MATCHVALUE_AR = 0;
                        objArLedger.STATUS_ART = "P";
                        _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objArLedger);
                        _unitOfWork.Save();

                        var objArVoucherChild = _unitOfWork.Repository<VOUCHERCHILD_RPT>().Create();
                        objArVoucherChild.ALLCODE_VCD = "GL";
                        objArVoucherChild.NARRATION_VCD = objBankTransaction.NARRATION_BT;
                        objArVoucherChild.ACCODE_VCD = allocDetail.AccountCode;
                        objArVoucherChild.ACDESCRIPTION_VCD = allocDetail.Description;
                        objArVoucherChild.CREDITAMOUNT_VCD = Convert.ToDecimal(allocDetail.Amount);
                        objArVoucherChild.AMOUNTSTATUTS_VCD = "Cr";
                        objArVoucherChild.DOCNO_VCD = objBankTransaction.DOCNUMBER_BT;
                        _unitOfWork.Repository<VOUCHERCHILD_RPT>().Insert(objArVoucherChild);
                        _unitOfWork.Save();
                        break;
                    case "BA":
                        var objBTransaction = _unitOfWork.Repository<BANKTRANSACTION>().Create();
                        objBTransaction.DOCNUMBER_BT = objBankTransaction.DOCNUMBER_BT;
                        objBTransaction.BANKCODE_BT = allocDetail.AccountCode;
                        objBTransaction.DOCDATE_BT = objBankTransaction.DOCDATE_BT;
                        objBTransaction.GLDATE_BT = objBankTransaction.GLDATE_BT;
                        objBTransaction.DEBITAMOUT_BT = Convert.ToDecimal(allocDetail.Amount);
                        objBTransaction.OTHERREF_BT = objBankTransaction.OTHERREF_BT;
                        objBTransaction.CHQDATE_BT = objBankTransaction.CHQDATE_BT;
                        objBankTransaction.CLEARANCEDATE_BT = objBankTransaction.CLEARANCEDATE_BT;
                        objBTransaction.NARRATION_BT = allocDetail.Narration;
                        objBTransaction.NOTE_BT = objBankTransaction.NOTE_BT;
                        objBTransaction.STATUS_BT = "P";
                        _unitOfWork.Repository<BANKTRANSACTION>().Insert(objBTransaction);
                        _unitOfWork.Save();
                        break;
                    case "GL":
                        var objGlTransaction = _unitOfWork.Repository<GLTRANSACTION1>().Create();
                        objGlTransaction.DOCNUMBER_GLT = objBankTransaction.DOCNUMBER_BT;
                        objGlTransaction.DOCDATE_GLT = objBankTransaction.DOCDATE_BT;
                        objGlTransaction.GLDATE_GLT = objBankTransaction.GLDATE_BT;
                        objGlTransaction.GLACCODE_GLT = allocDetail.AccountCode;
                        objGlTransaction.OTHERREF_GLT = objBankTransaction.OTHERREF_BT;
                        objGlTransaction.CREDITAMOUNT_GLT = Convert.ToDecimal(allocDetail.Amount);
                        objGlTransaction.NARRATION_GLT = allocDetail.Narration;
                        objGlTransaction.GLSTATUS_GLT = "P";
                        _unitOfWork.Repository<GLTRANSACTION1>().Insert(objGlTransaction);
                        _unitOfWork.Save();

                        var objGlVoucherChild = _unitOfWork.Repository<VOUCHERCHILD_RPT>().Create();
                        objGlVoucherChild.ALLCODE_VCD = "GL";
                        objGlVoucherChild.NARRATION_VCD = objBankTransaction.NARRATION_BT;
                        objGlVoucherChild.ACCODE_VCD = allocDetail.AccountCode;
                        objGlVoucherChild.ACDESCRIPTION_VCD = allocDetail.Description;
                        objGlVoucherChild.CREDITAMOUNT_VCD = Convert.ToDecimal(allocDetail.Amount);
                        objGlVoucherChild.AMOUNTSTATUTS_VCD = "Dr";
                        objGlVoucherChild.DOCNO_VCD = objBankTransaction.DOCNUMBER_BT;
                        _unitOfWork.Repository<VOUCHERCHILD_RPT>().Insert(objGlVoucherChild);
                        _unitOfWork.Save();
                        break;
                }
            }
            return RedirectToAction("BankReceipt");
        }

        public ActionResult SaveBankPayment(FormCollection form, BANKTRANSACTION objBankTransaction)
        {
            objBankTransaction.STATUS_BT = "P";
            objBankTransaction.MASTERSTATUS_BT = "M";
            _unitOfWork.Repository<BANKTRANSACTION>().Insert(objBankTransaction);
            _unitOfWork.Save();

            var objVoucherMaster = _unitOfWork.Repository<VOUCHERMASTER_RPT>().Create();
            objVoucherMaster.GLDATE_VRPT = objBankTransaction.GLDATE_BT;
            objVoucherMaster.ALLCODE_VRPT = "Bank";
            objVoucherMaster.BANKCODE_VRT = objBankTransaction.BANKCODE_BT;
            objVoucherMaster.ACCDESCRIPTION_VRPT = Convert.ToString(form["BankName"]);
            objVoucherMaster.PARTICULARS_VRPT = objBankTransaction.NARRATION_BT;
            objVoucherMaster.NOTES_VRPT = objBankTransaction.NOTE_BT;
            objVoucherMaster.DEBITAMOUT_VRPT = objVoucherMaster.CREDITAMOUNT_VRPT;
            objVoucherMaster.CHQNO_VRPT = objBankTransaction.CHQNO_BT;
            objVoucherMaster.CHQDATE_VRPT = objBankTransaction.CHQDATE_BT;
            objVoucherMaster.DOCNO_VRPT = objBankTransaction.DOCNUMBER_BT;
            _unitOfWork.Repository<VOUCHERMASTER_RPT>().Insert(objVoucherMaster);
            _unitOfWork.Save();

            string jsonAllocDetails = form["allocDetails"];
            var serializer = new JavaScriptSerializer();
            var lstAllocDetails = serializer.Deserialize<List<CustomAllocationDetails>>(jsonAllocDetails);
            foreach (var allocDetail in lstAllocDetails)
            {
                switch (allocDetail.AlCode)
                {
                    case "AP":
                        var objApLedger = _unitOfWork.Repository<AR_AP_LEDGER>().Create();
                        objApLedger.DOCNUMBER_ART = objBankTransaction.DOCNUMBER_BT;
                        objApLedger.DODATE_ART = objBankTransaction.DOCDATE_BT;
                        objApLedger.GLDATE_ART = objBankTransaction.GLDATE_BT;
                        objApLedger.ARAPCODE_ART = allocDetail.AccountCode;
                        objApLedger.DEBITAMOUNT_ART = Convert.ToInt32(allocDetail.Amount);
                        objApLedger.OTHERREF_ART = objBankTransaction.OTHERREF_BT;
                        objApLedger.NARRATION_ART = allocDetail.Narration;
                        objApLedger.MATCHVALUE_AR = 0;
                        objApLedger.STATUS_ART = "P";
                        _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objApLedger);
                        _unitOfWork.Save();

                        var objApVoucherChild = _unitOfWork.Repository<VOUCHERCHILD_RPT>().Create();
                        objApVoucherChild.ALLCODE_VCD = "GL";
                        objApVoucherChild.NARRATION_VCD = objBankTransaction.NARRATION_BT;
                        objApVoucherChild.ACCODE_VCD = allocDetail.AccountCode;
                        objApVoucherChild.ACDESCRIPTION_VCD = allocDetail.Description;
                        objApVoucherChild.DEBITAMOUNT_VCD = Convert.ToDecimal(allocDetail.Amount);
                        objApVoucherChild.AMOUNTSTATUTS_VCD = "Dr";
                        objApVoucherChild.DOCNO_VCD = objBankTransaction.DOCNUMBER_BT;
                        _unitOfWork.Repository<VOUCHERCHILD_RPT>().Insert(objApVoucherChild);
                        _unitOfWork.Save();
                        break;
                    case "AR":
                        var objArLedger = _unitOfWork.Repository<AR_AP_LEDGER>().Create();
                        objArLedger.DOCNUMBER_ART = objBankTransaction.DOCNUMBER_BT;
                        objArLedger.DODATE_ART = objBankTransaction.DOCDATE_BT;
                        objArLedger.GLDATE_ART = objBankTransaction.GLDATE_BT;
                        objArLedger.ARAPCODE_ART = allocDetail.AccountCode;
                        objArLedger.CREDITAMOUNT_ART = Convert.ToInt32(allocDetail.Amount);
                        objArLedger.OTHERREF_ART = objBankTransaction.OTHERREF_BT;
                        objArLedger.NARRATION_ART = allocDetail.Narration;
                        objArLedger.MATCHVALUE_AR = 0;
                        objArLedger.STATUS_ART = "P";
                        _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objArLedger);
                        _unitOfWork.Save();

                        var objArVoucherChild = _unitOfWork.Repository<VOUCHERCHILD_RPT>().Create();
                        objArVoucherChild.ALLCODE_VCD = "GL";
                        objArVoucherChild.NARRATION_VCD = objBankTransaction.NARRATION_BT;
                        objArVoucherChild.ACCODE_VCD = allocDetail.AccountCode;
                        objArVoucherChild.ACDESCRIPTION_VCD = allocDetail.Description;
                        objArVoucherChild.DEBITAMOUNT_VCD = Convert.ToDecimal(allocDetail.Amount);
                        objArVoucherChild.AMOUNTSTATUTS_VCD = "Dr";
                        objArVoucherChild.DOCNO_VCD = objBankTransaction.DOCNUMBER_BT;
                        _unitOfWork.Repository<VOUCHERCHILD_RPT>().Insert(objArVoucherChild);
                        _unitOfWork.Save();
                        break;
                    case "BA":
                        var objBTransaction = _unitOfWork.Repository<BANKTRANSACTION>().Create();
                        objBTransaction.DOCNUMBER_BT = objBankTransaction.DOCNUMBER_BT;
                        objBTransaction.BANKCODE_BT = allocDetail.AccountCode;
                        objBTransaction.DOCDATE_BT = objBankTransaction.DOCDATE_BT;
                        objBTransaction.GLDATE_BT = objBankTransaction.GLDATE_BT;
                        objBTransaction.DEBITAMOUT_BT = Convert.ToDecimal(allocDetail.Amount);
                        objBTransaction.OTHERREF_BT = objBankTransaction.OTHERREF_BT;
                        objBTransaction.CHQDATE_BT = objBankTransaction.CHQDATE_BT;
                        objBankTransaction.CLEARANCEDATE_BT = objBankTransaction.CLEARANCEDATE_BT;
                        objBTransaction.NARRATION_BT = allocDetail.Narration;
                        objBTransaction.NOTE_BT = objBankTransaction.NOTE_BT;
                        objBTransaction.STATUS_BT = "P";
                        _unitOfWork.Repository<BANKTRANSACTION>().Insert(objBTransaction);
                        _unitOfWork.Save();
                        break;
                    case "GL":
                        var objGlTransaction = _unitOfWork.Repository<GLTRANSACTION1>().Create();
                        objGlTransaction.DOCNUMBER_GLT = objBankTransaction.DOCNUMBER_BT;
                        objGlTransaction.DOCDATE_GLT = objBankTransaction.DOCDATE_BT;
                        objGlTransaction.GLDATE_GLT = objBankTransaction.GLDATE_BT;
                        objGlTransaction.GLACCODE_GLT = allocDetail.AccountCode;
                        objGlTransaction.OTHERREF_GLT = objBankTransaction.OTHERREF_BT;
                        objGlTransaction.DEBITAMOUNT_GLT = Convert.ToDecimal(allocDetail.Amount);
                        objGlTransaction.NARRATION_GLT = allocDetail.Narration;
                        objGlTransaction.GLSTATUS_GLT = "P";
                        _unitOfWork.Repository<GLTRANSACTION1>().Insert(objGlTransaction);
                        _unitOfWork.Save();

                        var objGlVoucherChild = _unitOfWork.Repository<VOUCHERCHILD_RPT>().Create();
                        objGlVoucherChild.ALLCODE_VCD = "GL";
                        objGlVoucherChild.NARRATION_VCD = objBankTransaction.NARRATION_BT;
                        objGlVoucherChild.ACCODE_VCD = allocDetail.AccountCode;
                        objGlVoucherChild.ACDESCRIPTION_VCD = allocDetail.Description;
                        objGlVoucherChild.CREDITAMOUNT_VCD = Convert.ToDecimal(allocDetail.Amount);
                        objGlVoucherChild.AMOUNTSTATUTS_VCD = "Dr";
                        objGlVoucherChild.DOCNO_VCD = objBankTransaction.DOCNUMBER_BT;
                        _unitOfWork.Repository<VOUCHERCHILD_RPT>().Insert(objGlVoucherChild);
                        _unitOfWork.Save();
                        break;
                }
            }
            return RedirectToAction("BankPayment");
        }

        public ActionResult BankMaster()
        {
            @ViewBag.CurrCode = "AED";
            @ViewBag.CurrName = "UAE DHIRHAM";
            return View();
        }

        public ActionResult SaveBankMaster()
        {
            throw new NotImplementedException();
        }
    }
}