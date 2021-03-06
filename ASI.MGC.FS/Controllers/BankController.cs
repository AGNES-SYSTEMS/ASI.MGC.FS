﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.ExtendedAPI;
using ASI.MGC.FS.Model;
using ASI.MGC.FS.Models;
using ASI.MGC.FS.WebCommon;

namespace ASI.MGC.FS.Controllers
{
    public class BankController : Controller
    {
        readonly IUnitOfWork _unitOfWork;
        readonly TimeZoneInfo tzInfo;
        DateTime today;
        public BankController()
        {
            _unitOfWork = new UnitOfWork();
            tzInfo = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time");
            today = TimeZoneInfo.ConvertTime(DateTime.Now, tzInfo);
        }
        // GET: Bank
        //public ActionResult Index()
        //{
        //    return View();
        //}
        [MesAuthorize("DailyTransactions")]
        public ActionResult BankReceipt()
        {
            var objBankTransaction = new BANKTRANSACTION();
            @ViewBag.Today = today.ToShortDateString();
            return View(objBankTransaction);
        }
        [MesAuthorize("DailyTransactions")]
        public ActionResult BankPayment()
        {
            var objBankTransaction = new BANKTRANSACTION();
            @ViewBag.Today = today.ToShortDateString();
            return View(objBankTransaction);
        }
        public JsonResult GetBankDetailsList(string sidx, string sord, int page, int rows, string bankName = "")
        {
            var bankList = (from banks in _unitOfWork.Repository<BANKMASTER>().Query().Get()
                            select banks).Select(a => new { a.BANKCODE_BM, a.BANKNAME_BM });
            if (!string.IsNullOrEmpty(bankName))
            {
                bankList = (from banks in _unitOfWork.Repository<BANKMASTER>().Query().Get()
                            where banks.BANKNAME_BM.Contains(bankName)
                            select banks).Select(a => new { a.BANKCODE_BM, a.BANKNAME_BM });
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

        public JsonResult GetBankDetailsByType(string sidx, string sord, int page, int rows, string bankType, string searchById = "", string searchByName = "")
        {
            var bankList = (from banks in _unitOfWork.Repository<BANKMASTER>().Query().Get()
                            select banks).Select(a => new { a.BANKCODE_BM, a.BANKNAME_BM });
            if (!string.IsNullOrEmpty(bankType))
            {
                bankList = (from jobs in _unitOfWork.Repository<BANKMASTER>().Query().Get()
                            where jobs.MODE_BM.Equals(bankType)
                            select jobs).Select(a => new { a.BANKCODE_BM, a.BANKNAME_BM });
            }
            if (!string.IsNullOrEmpty(searchById))
            {
                bankList = bankList.Where(bnk => bnk.BANKCODE_BM.Contains(searchById));
            }
            if (!string.IsNullOrEmpty(searchByName))
            {
                bankList = bankList.Where(bnk => bnk.BANKNAME_BM.Contains(searchByName));
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

        [HttpPost]
        public JsonResult SaveBankReceipt(FormCollection form, BANKTRANSACTION objBankTransaction)
        {
            string brNo = "";
            string currentUser = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    string docType = form["DocType"];
                    objBankTransaction.DOCNUMBER_BT = CommonModelAccessUtility.GetDocNo(_unitOfWork, docType);
                    objBankTransaction.CREDITAMOUT_BT = 0;
                    objBankTransaction.USER_BT = currentUser;
                    objBankTransaction.STATUS_BT = "P";
                    objBankTransaction.MASTERSTATUS_BT = "M";
                    _unitOfWork.Repository<BANKTRANSACTION>().Insert(objBankTransaction);
                    _unitOfWork.Save();
                    brNo = objBankTransaction.DOCNUMBER_BT;
                    //_unitOfWork.Truncate("VOUCHERMASTER_RPT");
                    //_unitOfWork.Truncate("VOUCHERCHILD_RPT");
                    //var objVoucherMaster = _unitOfWork.Repository<VOUCHERMASTER_RPT>().Create();
                    //objVoucherMaster.GLDATE_VRPT = objBankTransaction.GLDATE_BT;
                    //objVoucherMaster.ALLCODE_VRPT = "Bank";
                    //objVoucherMaster.BANKCODE_VRT = objBankTransaction.BANKCODE_BT;
                    //objVoucherMaster.ACCDESCRIPTION_VRPT = Convert.ToString(form["BankName"]);
                    //objVoucherMaster.PARTICULARS_VRPT = objBankTransaction.NARRATION_BT;
                    //objVoucherMaster.NOTES_VRPT = objBankTransaction.NOTE_BT;
                    //objVoucherMaster.DEBITAMOUT_VRPT = objBankTransaction.DEBITAMOUT_BT;
                    //objVoucherMaster.CREDITAMOUNT_VRPT = 0;
                    //objVoucherMaster.CHQNO_VRPT = objBankTransaction.CHQNO_BT;
                    //objVoucherMaster.CHQDATE_VRPT = objBankTransaction.CHQDATE_BT;
                    //objVoucherMaster.DOCNO_VRPT = objBankTransaction.DOCNUMBER_BT;
                    //objVoucherMaster.USER_VRPT = currentUser;
                    //objVoucherMaster.VOUCHER_TYPE = "BR";
                    //objVoucherMaster.DrawerBr_VRPT = objBankTransaction.BENACCOUNT_BT;
                    //_unitOfWork.Repository<VOUCHERMASTER_RPT>().Insert(objVoucherMaster);
                    //_unitOfWork.Save();

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
                                //form["hdnAcCode"] = allocDetail.AccountCode;
                                if (Convert.ToDecimal(allocDetail.Amount) <= 0)
                                {
                                    objApLedger.DEBITAMOUNT_ART = Math.Abs(Convert.ToDecimal(allocDetail.Amount));
                                }
                                else
                                {
                                    objApLedger.CREDITAMOUNT_ART = Math.Abs(Convert.ToDecimal(allocDetail.Amount));
                                }
                                objApLedger.OTHERREF_ART = objBankTransaction.OTHERREF_BT;
                                objApLedger.NARRATION_ART = allocDetail.Narration;
                                objApLedger.MATCHVALUE_AR = 0;
                                objApLedger.USER_ART = currentUser;
                                objApLedger.STATUS_ART = "P";
                                _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objApLedger);
                                _unitOfWork.Save();

                                //var objApVoucherChild = _unitOfWork.Repository<VOUCHERCHILD_RPT>().Create();
                                //objApVoucherChild.ALLCODE_VCD = "GL";
                                //objApVoucherChild.NARRATION_VCD = objBankTransaction.NARRATION_BT;
                                //objApVoucherChild.ACCODE_VCD = allocDetail.AccountCode;
                                //objApVoucherChild.ACDESCRIPTION_VCD = allocDetail.Description;
                                //objApVoucherChild.CREDITAMOUNT_VCD = Convert.ToDecimal(allocDetail.Amount);
                                //objApVoucherChild.AMOUNTSTATUTS_VCD = "Cr";
                                //objApVoucherChild.DOCNO_VCD = objBankTransaction.DOCNUMBER_BT;
                                //_unitOfWork.Repository<VOUCHERCHILD_RPT>().Insert(objApVoucherChild);
                                //_unitOfWork.Save();
                                break;
                            case "AR":
                                var objArLedger = _unitOfWork.Repository<AR_AP_LEDGER>().Create();
                                objArLedger.DOCNUMBER_ART = objBankTransaction.DOCNUMBER_BT;
                                objArLedger.DODATE_ART = objBankTransaction.DOCDATE_BT;
                                objArLedger.GLDATE_ART = objBankTransaction.GLDATE_BT;
                                objArLedger.ARAPCODE_ART = allocDetail.AccountCode;
                                //form["hdnAcCode"] = allocDetail.AccountCode;
                                if (Convert.ToDecimal(allocDetail.Amount) <= 0)
                                {
                                    objArLedger.DEBITAMOUNT_ART = Math.Abs(Convert.ToDecimal(allocDetail.Amount));
                                }
                                else
                                {
                                    objArLedger.CREDITAMOUNT_ART = Math.Abs(Convert.ToDecimal(allocDetail.Amount));
                                }
                                objArLedger.OTHERREF_ART = objBankTransaction.OTHERREF_BT;
                                objArLedger.NARRATION_ART = allocDetail.Narration;
                                objArLedger.MATCHVALUE_AR = 0;
                                objArLedger.USER_ART = currentUser;
                                objArLedger.STATUS_ART = "P";
                                _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objArLedger);
                                _unitOfWork.Save();

                                //var objArVoucherChild = _unitOfWork.Repository<VOUCHERCHILD_RPT>().Create();
                                //objArVoucherChild.ALLCODE_VCD = "GL";
                                //objArVoucherChild.NARRATION_VCD = objBankTransaction.NARRATION_BT;
                                //objArVoucherChild.ACCODE_VCD = allocDetail.AccountCode;
                                //objArVoucherChild.ACDESCRIPTION_VCD = allocDetail.Description;
                                //objArVoucherChild.CREDITAMOUNT_VCD = Convert.ToDecimal(allocDetail.Amount);
                                //objArVoucherChild.AMOUNTSTATUTS_VCD = "Cr";
                                //objArVoucherChild.DOCNO_VCD = objBankTransaction.DOCNUMBER_BT;
                                //_unitOfWork.Repository<VOUCHERCHILD_RPT>().Insert(objArVoucherChild);
                                //_unitOfWork.Save();
                                break;
                            case "BA":
                                var objBTransaction = _unitOfWork.Repository<BANKTRANSACTION>().Create();
                                objBTransaction.DOCNUMBER_BT = objBankTransaction.DOCNUMBER_BT;
                                objBTransaction.BANKCODE_BT = allocDetail.AccountCode;
                                objBTransaction.DOCDATE_BT = objBankTransaction.DOCDATE_BT;
                                objBTransaction.GLDATE_BT = objBankTransaction.GLDATE_BT;
                                if (Convert.ToDecimal(allocDetail.Amount) <= 0)
                                {
                                    objBTransaction.DEBITAMOUT_BT = Math.Abs(Convert.ToDecimal(allocDetail.Amount));
                                }
                                else
                                {
                                    objBTransaction.CREDITAMOUT_BT = Math.Abs(Convert.ToDecimal(allocDetail.Amount));
                                }
                                objBTransaction.OTHERREF_BT = objBankTransaction.OTHERREF_BT;
                                objBTransaction.CHQDATE_BT = objBankTransaction.CHQDATE_BT;
                                objBankTransaction.CLEARANCEDATE_BT = objBankTransaction.CLEARANCEDATE_BT;
                                objBTransaction.NARRATION_BT = allocDetail.Narration;
                                objBTransaction.NOTE_BT = objBankTransaction.NOTE_BT;
                                objBankTransaction.USER_BT = currentUser;
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
                                if (Convert.ToDecimal(allocDetail.Amount) <= 0)
                                {
                                    objGlTransaction.DEBITAMOUNT_GLT = Math.Abs(Convert.ToDecimal(allocDetail.Amount));
                                }
                                else
                                {
                                    objGlTransaction.CREDITAMOUNT_GLT = Math.Abs(Convert.ToDecimal(allocDetail.Amount));
                                }
                                objGlTransaction.NARRATION_GLT = allocDetail.Narration;
                                objGlTransaction.VARUSER = currentUser;
                                objGlTransaction.GLSTATUS_GLT = "P";
                                _unitOfWork.Repository<GLTRANSACTION1>().Insert(objGlTransaction);
                                _unitOfWork.Save();

                                //var objGlVoucherChild = _unitOfWork.Repository<VOUCHERCHILD_RPT>().Create();
                                //objGlVoucherChild.ALLCODE_VCD = "GL";
                                //objGlVoucherChild.NARRATION_VCD = objBankTransaction.NARRATION_BT;
                                //objGlVoucherChild.ACCODE_VCD = allocDetail.AccountCode;
                                //objGlVoucherChild.ACDESCRIPTION_VCD = allocDetail.Description;
                                //objGlVoucherChild.CREDITAMOUNT_VCD = Convert.ToDecimal(allocDetail.Amount);
                                //objGlVoucherChild.AMOUNTSTATUTS_VCD = "Dr";
                                //objGlVoucherChild.DOCNO_VCD = objBankTransaction.DOCNUMBER_BT;
                                //_unitOfWork.Repository<VOUCHERCHILD_RPT>().Insert(objGlVoucherChild);
                                //_unitOfWork.Save();
                                break;
                        }
                    }
                    CommonModelAccessUtility.updateDocNo(_unitOfWork, form["DocType"]);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            return Json(brNo, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveBankPayment(FormCollection form, BANKTRANSACTION objBankTransaction)
        {
            string bpNo = "";
            string currentUser = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    string docType = form["DocType"];
                    objBankTransaction.DOCNUMBER_BT = CommonModelAccessUtility.GetDocNo(_unitOfWork, docType);
                    objBankTransaction.DEBITAMOUT_BT = 0;
                    objBankTransaction.USER_BT = currentUser;
                    objBankTransaction.STATUS_BT = "P";
                    objBankTransaction.MASTERSTATUS_BT = "M";
                    _unitOfWork.Repository<BANKTRANSACTION>().Insert(objBankTransaction);
                    _unitOfWork.Save();
                    //_unitOfWork.Truncate("VOUCHERMASTER_RPT");
                    //_unitOfWork.Truncate("VOUCHERCHILD_RPT");
                    bpNo = objBankTransaction.DOCNUMBER_BT;
                    //var objVoucherMaster = _unitOfWork.Repository<VOUCHERMASTER_RPT>().Create();
                    //objVoucherMaster.GLDATE_VRPT = objBankTransaction.GLDATE_BT;
                    //objVoucherMaster.ALLCODE_VRPT = "Bank";
                    //objVoucherMaster.BANKCODE_VRT = objBankTransaction.BANKCODE_BT;
                    //objVoucherMaster.ACCDESCRIPTION_VRPT = Convert.ToString(form["BankName"]);
                    //objVoucherMaster.PARTICULARS_VRPT = objBankTransaction.NARRATION_BT;
                    //objVoucherMaster.NOTES_VRPT = objBankTransaction.NOTE_BT;
                    //objVoucherMaster.CREDITAMOUNT_VRPT = objBankTransaction.CREDITAMOUT_BT;
                    //objVoucherMaster.DEBITAMOUT_VRPT = 0;
                    //objVoucherMaster.CHQNO_VRPT = objBankTransaction.CHQNO_BT;
                    //objVoucherMaster.CHQDATE_VRPT = objBankTransaction.CHQDATE_BT;
                    //objVoucherMaster.DOCNO_VRPT = objBankTransaction.DOCNUMBER_BT;
                    //objVoucherMaster.USER_VRPT = currentUser;
                    //objVoucherMaster.VOUCHER_TYPE = "BP";
                    //_unitOfWork.Repository<VOUCHERMASTER_RPT>().Insert(objVoucherMaster);
                    //_unitOfWork.Save();

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
                                //form["hdnAcCode"] = allocDetail.AccountCode;
                                if (Convert.ToDecimal(allocDetail.Amount) >= 0)
                                {
                                    objApLedger.DEBITAMOUNT_ART = Convert.ToDecimal(allocDetail.Amount);
                                }
                                else
                                {
                                    objApLedger.CREDITAMOUNT_ART = Math.Abs(Convert.ToDecimal(allocDetail.Amount));
                                }
                                objApLedger.OTHERREF_ART = objBankTransaction.OTHERREF_BT;
                                objApLedger.NARRATION_ART = allocDetail.Narration;
                                objApLedger.MATCHVALUE_AR = 0;
                                objApLedger.USER_ART = currentUser;
                                objApLedger.STATUS_ART = "P";
                                _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objApLedger);
                                _unitOfWork.Save();

                                //var objApVoucherChild = _unitOfWork.Repository<VOUCHERCHILD_RPT>().Create();
                                //objApVoucherChild.ALLCODE_VCD = "GL";
                                //objApVoucherChild.NARRATION_VCD = objBankTransaction.NARRATION_BT;
                                //objApVoucherChild.ACCODE_VCD = allocDetail.AccountCode;
                                //objApVoucherChild.ACDESCRIPTION_VCD = allocDetail.Description;
                                //objApVoucherChild.DEBITAMOUNT_VCD = Convert.ToDecimal(allocDetail.Amount);
                                //objApVoucherChild.AMOUNTSTATUTS_VCD = "Dr";
                                //objApVoucherChild.DOCNO_VCD = objBankTransaction.DOCNUMBER_BT;
                                //_unitOfWork.Repository<VOUCHERCHILD_RPT>().Insert(objApVoucherChild);
                                //_unitOfWork.Save();
                                break;
                            case "AR":
                                var objArLedger = _unitOfWork.Repository<AR_AP_LEDGER>().Create();
                                objArLedger.DOCNUMBER_ART = objBankTransaction.DOCNUMBER_BT;
                                objArLedger.DODATE_ART = objBankTransaction.DOCDATE_BT;
                                objArLedger.GLDATE_ART = objBankTransaction.GLDATE_BT;
                                objArLedger.ARAPCODE_ART = allocDetail.AccountCode;
                                //form["hdnAcCode"] = allocDetail.AccountCode;
                                if (Convert.ToDecimal(allocDetail.Amount) >= 0)
                                {
                                    objArLedger.DEBITAMOUNT_ART = Convert.ToDecimal(allocDetail.Amount);
                                }
                                else
                                {
                                    objArLedger.CREDITAMOUNT_ART = Math.Abs(Convert.ToDecimal(allocDetail.Amount));
                                }
                                objArLedger.OTHERREF_ART = objBankTransaction.OTHERREF_BT;
                                objArLedger.NARRATION_ART = allocDetail.Narration;
                                objArLedger.MATCHVALUE_AR = 0;
                                objArLedger.USER_ART = currentUser;
                                objArLedger.STATUS_ART = "P";
                                _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objArLedger);
                                _unitOfWork.Save();

                                //var objArVoucherChild = _unitOfWork.Repository<VOUCHERCHILD_RPT>().Create();
                                //objArVoucherChild.ALLCODE_VCD = "GL";
                                //objArVoucherChild.NARRATION_VCD = objBankTransaction.NARRATION_BT;
                                //objArVoucherChild.ACCODE_VCD = allocDetail.AccountCode;
                                //objArVoucherChild.ACDESCRIPTION_VCD = allocDetail.Description;
                                //objArVoucherChild.DEBITAMOUNT_VCD = Convert.ToDecimal(allocDetail.Amount);
                                //objArVoucherChild.AMOUNTSTATUTS_VCD = "Dr";
                                //objArVoucherChild.DOCNO_VCD = objBankTransaction.DOCNUMBER_BT;
                                //_unitOfWork.Repository<VOUCHERCHILD_RPT>().Insert(objArVoucherChild);
                                //_unitOfWork.Save();
                                break;
                            case "BA":
                                var objBTransaction = _unitOfWork.Repository<BANKTRANSACTION>().Create();
                                objBTransaction.DOCNUMBER_BT = objBankTransaction.DOCNUMBER_BT;
                                objBTransaction.BANKCODE_BT = allocDetail.AccountCode;
                                objBTransaction.DOCDATE_BT = objBankTransaction.DOCDATE_BT;
                                objBTransaction.GLDATE_BT = objBankTransaction.GLDATE_BT;
                                if (Convert.ToDecimal(allocDetail.Amount) >= 0)
                                {
                                    objBTransaction.DEBITAMOUT_BT = Convert.ToDecimal(allocDetail.Amount);
                                }
                                else
                                {
                                    objBTransaction.CREDITAMOUT_BT = Math.Abs(Convert.ToDecimal(allocDetail.Amount));
                                }
                                objBTransaction.OTHERREF_BT = objBankTransaction.OTHERREF_BT;
                                objBTransaction.CHQDATE_BT = objBankTransaction.CHQDATE_BT;
                                objBankTransaction.CLEARANCEDATE_BT = objBankTransaction.CLEARANCEDATE_BT;
                                objBTransaction.NARRATION_BT = allocDetail.Narration;
                                objBTransaction.NOTE_BT = objBankTransaction.NOTE_BT;
                                objBTransaction.USER_BT = currentUser;
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
                                if (Convert.ToDecimal(allocDetail.Amount) >= 0)
                                {
                                    objGlTransaction.DEBITAMOUNT_GLT = Convert.ToDecimal(allocDetail.Amount);
                                }
                                else
                                {
                                    objGlTransaction.CREDITAMOUNT_GLT = Math.Abs(Convert.ToDecimal(allocDetail.Amount));
                                }
                                objGlTransaction.NARRATION_GLT = allocDetail.Narration;
                                objGlTransaction.VARUSER = currentUser;
                                objGlTransaction.GLSTATUS_GLT = "P";
                                _unitOfWork.Repository<GLTRANSACTION1>().Insert(objGlTransaction);
                                _unitOfWork.Save();

                                //var objGlVoucherChild = _unitOfWork.Repository<VOUCHERCHILD_RPT>().Create();
                                //objGlVoucherChild.ALLCODE_VCD = "GL";
                                //objGlVoucherChild.NARRATION_VCD = objBankTransaction.NARRATION_BT;
                                //objGlVoucherChild.ACCODE_VCD = allocDetail.AccountCode;
                                //objGlVoucherChild.ACDESCRIPTION_VCD = allocDetail.Description;
                                //objGlVoucherChild.CREDITAMOUNT_VCD = Convert.ToDecimal(allocDetail.Amount);
                                //objGlVoucherChild.AMOUNTSTATUTS_VCD = "Dr";
                                //objGlVoucherChild.DOCNO_VCD = objBankTransaction.DOCNUMBER_BT;
                                //_unitOfWork.Repository<VOUCHERCHILD_RPT>().Insert(objGlVoucherChild);
                                //_unitOfWork.Save();
                                break;
                        }
                    }
                    CommonModelAccessUtility.updateDocNo(_unitOfWork, docType);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            return Json(bpNo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BankMaster()
        {
            var objBankMaster = new BANKMASTER();
            @ViewBag.CurrCode = "AED";
            @ViewBag.CurrName = "UAE DHIRHAM";
            @ViewBag.bankModeTypes = CommonModelAccessUtility.GetBankModes();
            @ViewBag.bankStatus = CommonModelAccessUtility.GetBankStatus();
            @ViewBag.Today = today.ToShortDateString();
            return View(objBankMaster);
        }

        [HttpPost]
        public JsonResult SaveBankMaster(FormCollection frm, BANKMASTER objBankmaster)
        {
            bool success;
            string currentUser = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var existingObj = _unitOfWork.Repository<BANKMASTER>().FindByID(objBankmaster.BANKCODE_BM);
                    if (existingObj != null)
                    {
                        existingObj.BANKCODE_BM = objBankmaster.BANKCODE_BM;
                        existingObj.BANKNAME_BM = objBankmaster.BANKNAME_BM;
                        existingObj.CURRENCY_BM = objBankmaster.CURRENCY_BM;
                        existingObj.MODE_BM = objBankmaster.MODE_BM;
                        existingObj.ODLIMIT_BM = objBankmaster.ODLIMIT_BM;
                        existingObj.ACCOUNTNUMBER_BM = objBankmaster.ACCOUNTNUMBER_BM;
                        existingObj.RECEIPTLOCATION_BM = objBankmaster.RECEIPTLOCATION_BM;
                        existingObj.BANKSTTUS_BM = objBankmaster.BANKSTTUS_BM;
                        _unitOfWork.Repository<BANKMASTER>().Update(existingObj);
                    }
                    else
                    {
                        _unitOfWork.Repository<BANKMASTER>().Insert(objBankmaster);
                    }
                    _unitOfWork.Save();

                    var objBankTransaction = _unitOfWork.Repository<BANKTRANSACTION>().Create();
                    objBankTransaction.BANKCODE_BT = objBankmaster.BANKCODE_BM;
                    objBankTransaction.DOCNUMBER_BT = objBankmaster.BANKCODE_BM;
                    objBankTransaction.DOCDATE_BT = today.Date;
                    objBankTransaction.GLDATE_BT = Convert.ToDateTime(frm["BankDate"]);
                    objBankTransaction.DEBITAMOUT_BT = Convert.ToDecimal(frm["OpenBalance"]);
                    objBankTransaction.CREDITAMOUT_BT = 0;
                    objBankTransaction.CHQDATE_BT = today.Date;
                    objBankTransaction.CLEARANCEDATE_BT = today.Date;
                    objBankTransaction.NARRATION_BT = "Opening Balance";
                    objBankTransaction.NOTE_BT = frm["Note"];
                    objBankTransaction.USER_BT = currentUser;
                    objBankTransaction.STATUS_BT = "OP";

                    _unitOfWork.Repository<BANKTRANSACTION>().Insert(objBankTransaction);
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

        public JsonResult GetBankCodeDetails(string bankCode = "")
        {
            Dictionary<string, string> bankData = new Dictionary<string, string>();
            try
            {
                var glCodeData = (from bankMaster in _unitOfWork.Repository<BANKMASTER>().Query().Get()
                                  where (bankMaster.BANKCODE_BM == bankCode)
                                  select new
                                  {
                                      bankMaster.BANKCODE_BM,
                                      bankMaster.BANKNAME_BM,
                                      bankMaster.ACCOUNTNUMBER_BM,
                                      bankMaster.ODLIMIT_BM,
                                      bankMaster.MODE_BM,
                                      bankMaster.BANKSTTUS_BM
                                  }).SingleOrDefault();
                var bankCodeDetails = (from bankTransaction in _unitOfWork.Repository<BANKTRANSACTION>().Query().Get()
                                       where (bankTransaction.BANKCODE_BT == bankCode && bankTransaction.STATUS_BT == "OP")
                                       select new
                                       {
                                           bankTransaction.DEBITAMOUT_BT,
                                           bankTransaction.CREDITAMOUT_BT,
                                           bankTransaction.GLDATE_BT,
                                           bankTransaction.NOTE_BT
                                       }).SingleOrDefault();

                if (glCodeData != null)
                {
                    bankData.Add("bankCode", glCodeData.BANKCODE_BM);
                    bankData.Add("bankDesc", glCodeData.BANKNAME_BM);
                    bankData.Add("accountNo", glCodeData.ACCOUNTNUMBER_BM);
                    bankData.Add("ODLimit", Convert.ToString(glCodeData.ODLIMIT_BM));
                    bankData.Add("Mode", glCodeData.MODE_BM);
                    bankData.Add("bankStatus", glCodeData.BANKSTTUS_BM);
                }
                if (bankCodeDetails != null)
                {
                    bankData.Add("debitAmount", Convert.ToString(bankCodeDetails.DEBITAMOUT_BT));
                    bankData.Add("Notes", bankCodeDetails.NOTE_BT);
                    bankData.Add("bankDate", Convert.ToDateTime(bankCodeDetails.GLDATE_BT).ToShortDateString());
                }
                else
                {
                    bankData.Add("debitAmount", "");
                    bankData.Add("Notes", "");
                    bankData.Add("bankDate", today.ToShortDateString());
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return Json(bankData, JsonRequestBehavior.AllowGet);
        }
    }
}