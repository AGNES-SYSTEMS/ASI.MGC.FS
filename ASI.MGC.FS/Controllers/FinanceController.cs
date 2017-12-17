using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;
using ASI.MGC.FS.Models;
using ASI.MGC.FS.WebCommon;
using ASI.MGC.FS.Domain.Repositories;

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
            var objGlMaster = new GLMASTER();
            ViewBag.accountsType = CommonModelAccessUtility.GetAccountsType();
            ViewBag.glType = CommonModelAccessUtility.GetGlType();
            ViewBag.balanceType = CommonModelAccessUtility.GetBalanceType();
            return View(objGlMaster);
        }

        [HttpPost]
        public JsonResult SaveGlCreation(FormCollection frm, GLMASTER objGlMaster)
        {
            bool success;
            string currentUser = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    _unitOfWork.Repository<GLMASTER>().Insert(objGlMaster);
                    _unitOfWork.Save();

                    var objGlTransaction = _unitOfWork.Repository<GLTRANSACTION1>().Create();
                    objGlTransaction.DOCDATE_GLT = DateTime.Now;
                    objGlTransaction.GLDATE_GLT = Convert.ToDateTime(frm["GLDate"]);
                    objGlTransaction.DOCNUMBER_GLT = objGlMaster.GLCODE_LM;
                    objGlTransaction.GLACCODE_GLT = objGlMaster.GLCODE_LM;
                    objGlTransaction.OTHERREF_GLT = objGlMaster.GLCODE_LM;
                    objGlTransaction.VARUSER = currentUser;
                    if (Convert.ToInt32(frm["BalanceType"]) == 1)
                    {
                        objGlTransaction.CREDITAMOUNT_GLT = Convert.ToDecimal(frm["OpenBalance"]);
                        objGlTransaction.DEBITAMOUNT_GLT = 0;
                    }
                    else
                    {
                        objGlTransaction.CREDITAMOUNT_GLT = 0;
                        objGlTransaction.DEBITAMOUNT_GLT = Convert.ToDecimal(frm["OpenBalance"]);
                    }
                    objGlTransaction.NARRATION_GLT = "Opening Balance";
                    objGlTransaction.GLSTATUS_GLT = "OP";
                    _unitOfWork.Repository<GLTRANSACTION1>().Insert(objGlTransaction);
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
            throw new NotImplementedException();
        }

        public ActionResult SaveAccountsReceivable()
        {
            throw new NotImplementedException();
        }

        public ActionResult PdcReceipt()
        {
            var objBankTransaction = new BANKTRANSACTION();
            return View(objBankTransaction);
        }

        public ActionResult JvCreation()
        {
            var objBankTransaction = new BANKTRANSACTION();
            return View(objBankTransaction);
        }

        public ActionResult ArapMatching(string AcCode = null, string DocNo = null)
        {
            var objAllocDetails = new ARMATCHING();
            if (!string.IsNullOrEmpty(AcCode))
            {
                var custDetails = (from customers in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                                where customers.ARCODE_ARM.Equals(AcCode)
                                select customers).Select(a => new { a.ARCODE_ARM, a.DESCRIPTION_ARM }).FirstOrDefault();
                ViewBag.AcCode = custDetails.ARCODE_ARM;
                ViewBag.AcDesc = custDetails.DESCRIPTION_ARM;
            }
            if (!string.IsNullOrEmpty(DocNo))
            {
                ViewBag.DocNo = DocNo;
            }
            return View(objAllocDetails);
        }

        public ActionResult ArUnmatching()
        {
            ViewBag.DocType = CommonModelAccessUtility.GetDocTypes(_unitOfWork);
            var objAllocDetails = new ARMATCHING();
            return View(objAllocDetails);
        }

        public ActionResult DocumentReversal()
        {
            @ViewBag.DocType = CommonModelAccessUtility.GetDocTypes(_unitOfWork);
            return View();
        }
        public ActionResult DayEndOperation()
        {
            var objDayEnd = new DayEndOperationModel();
            objDayEnd = CommonModelAccessUtility.InitializeDayEndObj(_unitOfWork);
            return View(objDayEnd);
        }
        [HttpPost]
        public JsonResult SavePdcReceipt(FormCollection form, BANKTRANSACTION objBankTransaction)
        {
            string pdcNo = "";
            string currentUser = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);
            bool success;
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    objBankTransaction.USER_BT = currentUser;
                    objBankTransaction.STATUS_BT = "P";
                    objBankTransaction.MASTERSTATUS_BT = "M";
                    _unitOfWork.Repository<BANKTRANSACTION>().Insert(objBankTransaction);
                    _unitOfWork.Save();
                    _unitOfWork.Truncate("VOUCHERMASTER_RPT");
                    _unitOfWork.Truncate("VOUCHERCHILD_RPT");
                    pdcNo = objBankTransaction.DOCNUMBER_BT;
                    var objVoucherMaster = _unitOfWork.Repository<VOUCHERMASTER_RPT>().Create();
                    objVoucherMaster.GLDATE_VRPT = objBankTransaction.GLDATE_BT;
                    objVoucherMaster.ALLCODE_VRPT = "Bank";
                    objVoucherMaster.BANKCODE_VRT = objBankTransaction.BANKCODE_BT;
                    objVoucherMaster.ACCDESCRIPTION_VRPT = Convert.ToString(form["BankName"]);
                    objVoucherMaster.PARTICULARS_VRPT = objBankTransaction.NARRATION_BT;
                    objVoucherMaster.NOTES_VRPT = objBankTransaction.NOTE_BT;
                    objVoucherMaster.DEBITAMOUT_VRPT = objBankTransaction.DEBITAMOUT_BT;
                    objVoucherMaster.CREDITAMOUNT_VRPT = 0;
                    objVoucherMaster.CHQNO_VRPT = objBankTransaction.CHQNO_BT;
                    objVoucherMaster.CHQDATE_VRPT = objBankTransaction.CHQDATE_BT;
                    objVoucherMaster.DOCNO_VRPT = objBankTransaction.DOCNUMBER_BT;
                    objVoucherMaster.USER_VRPT = currentUser;
                    objVoucherMaster.VOUCHER_TYPE = "BR";
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
                                objApLedger.CREDITAMOUNT_ART = Convert.ToDecimal(allocDetail.Amount);
                                objApLedger.OTHERREF_ART = objBankTransaction.OTHERREF_BT;
                                objApLedger.NARRATION_ART = allocDetail.Narration;
                                objApLedger.MATCHVALUE_AR = 0;
                                objApLedger.USER_ART = currentUser;
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
                                objArLedger.CREDITAMOUNT_ART = Convert.ToDecimal(allocDetail.Amount);
                                objArLedger.OTHERREF_ART = objBankTransaction.OTHERREF_BT;
                                objArLedger.NARRATION_ART = allocDetail.Narration;
                                objArLedger.MATCHVALUE_AR = 0;
                                objArLedger.USER_ART = currentUser;
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
                                objGlTransaction.CREDITAMOUNT_GLT = Convert.ToDecimal(allocDetail.Amount);
                                objGlTransaction.NARRATION_GLT = allocDetail.Narration;
                                objGlTransaction.VARUSER = currentUser;
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
                    success = true;
                    transaction.Commit();
                }
                catch (Exception)
                {
                    success = false;
                    transaction.Rollback();
                }
            }
            return Json(new { success, pdcNo }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveDayEndOperation(DayEndOperationModel objDayEndOpr)
        {
            string currentUser = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);
            bool success = false;
            ENTRYMASTER objEntry = new ENTRYMASTER();
            string msg = string.Empty;
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var objGLTransaction = new GLTRANSACTION1();
                    objGLTransaction.DOCNUMBER_GLT = objDayEndOpr.DocumentNo;
                    objGLTransaction.DOCDATE_GLT = Convert.ToDateTime(objDayEndOpr.Date);
                    objGLTransaction.GLDATE_GLT = Convert.ToDateTime(objDayEndOpr.DayTo);
                    objGLTransaction.NARRATION_GLT = objDayEndOpr.DayFrom + " TO " + objDayEndOpr.DayTo;
                    objGLTransaction.VARUSER = currentUser;
                    objGLTransaction.GLSTATUS_GLT = "P";

                    objEntry = GetEntryMasterACcodeDetails("CASHSALE");
                    var var_CashAcCode = objEntry != null ? objEntry.ACCODE_EM : "";
                    objEntry = GetEntryMasterACcodeDetails("SALESDIS");
                    var var_DiscAccode = objEntry != null ? objEntry.ACCODE_EM : "";
                    if (string.IsNullOrEmpty(var_DiscAccode))
                    {
                        msg = "Can't SalesAcCode Discount Code";
                        goto err;
                    }
                    objEntry = GetEntryMasterACcodeDetails("SALE1");
                    var var_SalesAcode = objEntry != null ? objEntry.ACCODE_EM : "";
                    if (string.IsNullOrEmpty(var_SalesAcode))
                    {
                        msg = "Can't SalesAcCode Discount Code";
                        goto err;
                    }
                    objEntry = GetEntryMasterACcodeDetails("SALE2");
                    var var_JobTotalAcCode = objEntry != null ? objEntry.ACCODE_EM : "";
                    if (string.IsNullOrEmpty(var_JobTotalAcCode))
                    {
                        msg = "Can't JobTotalAccode Discount Code";
                    }
                    objEntry = GetEntryMasterACcodeDetails("SALESHIPIN");
                    var var_ShippingAcCode = objEntry != null ? objEntry.ACCODE_EM : "";
                    if (string.IsNullOrEmpty(var_ShippingAcCode))
                    {
                        msg = "Can't ShippingChargeACCOde Discount Code";
                        goto err;
                    }

                    if (Convert.ToDecimal(objDayEndOpr.JobTotal) > 0)
                    {
                        var objGLJobTrans = objGLTransaction;
                        objGLJobTrans.DEBITAMOUNT_GLT = 0;
                        objGLJobTrans.CREDITAMOUNT_GLT = Convert.ToDecimal(objDayEndOpr.JobTotal);
                        objGLJobTrans.GLACCODE_GLT = var_JobTotalAcCode;
                        _unitOfWork.Repository<GLTRANSACTION1>().Insert(objGLJobTrans);
                        _unitOfWork.Save();
                    }
                    if (Convert.ToDecimal(objDayEndOpr.SalesTotal) > 0)
                    {
                        var objGLSaleTrans = objGLTransaction;
                        objGLSaleTrans.DEBITAMOUNT_GLT = 0;
                        objGLSaleTrans.CREDITAMOUNT_GLT = Convert.ToDecimal(objDayEndOpr.SalesTotal);
                        objGLSaleTrans.GLACCODE_GLT = var_SalesAcode;
                        _unitOfWork.Repository<GLTRANSACTION1>().Insert(objGLSaleTrans);
                        _unitOfWork.Save();
                    }
                    if (Convert.ToDecimal(objDayEndOpr.DiscountTotal) > 0)
                    {
                        var objGLDiscTrans = objGLTransaction;
                        objGLDiscTrans.DEBITAMOUNT_GLT = Convert.ToDecimal(objDayEndOpr.DiscountTotal);
                        objGLDiscTrans.CREDITAMOUNT_GLT = 0;
                        objGLDiscTrans.GLACCODE_GLT = var_DiscAccode;
                        _unitOfWork.Repository<GLTRANSACTION1>().Insert(objGLDiscTrans);
                        _unitOfWork.Save();
                    }
                    if (Convert.ToDecimal(objDayEndOpr.ShippingTotal) > 0)
                    {
                        var objGLShipTrans = objGLTransaction;
                        objGLShipTrans.DEBITAMOUNT_GLT = 0;
                        objGLShipTrans.CREDITAMOUNT_GLT = Convert.ToDecimal(objDayEndOpr.ShippingTotal);
                        objGLShipTrans.GLACCODE_GLT = var_ShippingAcCode;
                        _unitOfWork.Repository<GLTRANSACTION1>().Insert(objGLShipTrans);
                        _unitOfWork.Save();
                    }
                    DayEndUpdate(Convert.ToDateTime(objDayEndOpr.DayFrom), Convert.ToDateTime(objDayEndOpr.DayTo), objDayEndOpr.DocumentNo);
                    success = true;
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    success = false;
                    transaction.Rollback();
                }
            }
        err:

            var jsonData = new
            {
                success = success,
                msg = msg
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveDocumentReversal(FormCollection frm)
        {
            bool success = false;
            string currentUser = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);
            var invCode = frm["InvNumber"];
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    if (!string.IsNullOrEmpty(invCode))
                    {
                        // Checking Bank Transactions entries and adding their reverse
                        var btEnteries = (from bankTransactions in _unitOfWork.Repository<BANKTRANSACTION>().Query().Get()
                                          where (bankTransactions.DOCNUMBER_BT == invCode)
                                          select bankTransactions);
                        foreach (var entry in btEnteries.ToList())
                        {
                            entry.STATUS_BT = "R";
                            entry.USER_BT = currentUser;
                            _unitOfWork.Repository<BANKTRANSACTION>().Update(entry);
                            _unitOfWork.Save();

                            // adding reverse entry
                            success = ReverseBankTransactions(entry);
                        }

                        // Checking AR_AP Ledger entries and adding their reverse
                        var arApEnteries = (from arApLedger in _unitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                                            where (arApLedger.DOCNUMBER_ART == invCode)
                                            select arApLedger);
                        foreach (var entry in arApEnteries.ToList())
                        {
                            entry.USER_ART = currentUser;
                            entry.STATUS_ART = "R";
                            _unitOfWork.Repository<AR_AP_LEDGER>().Update(entry);
                            _unitOfWork.Save();

                            // adding reverse entry
                            success = ReverseAr_Ap_Enteries(entry);
                        }

                        // Checking GL Transactions entries and adding their reverse
                        var glEnteries = (from glTransactions in _unitOfWork.Repository<GLTRANSACTION1>().Query().Get()
                                          where (glTransactions.DOCNUMBER_GLT == invCode)
                                          select glTransactions);
                        foreach (var entry in glEnteries.ToList())
                        {
                            entry.VARUSER = currentUser;
                            entry.GLSTATUS_GLT = "R";
                            _unitOfWork.Repository<GLTRANSACTION1>().Update(entry);
                            _unitOfWork.Save();

                            // adding reverse entry
                            success = ReverseGlTransactions(entry);
                        }
                    }
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
        public JsonResult SaveArApMatching(ARMATCHING objAllocDetails, FormCollection frm)
        {
            bool success = false;
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    string currentUser = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);
                    if (!string.IsNullOrEmpty(objAllocDetails.DOCCNUMBER_ARM))
                    {
                        var matchNo = (from allocDetails in _unitOfWork.Repository<ARMATCHING>().Query().Get()
                                       select allocDetails).Max(p => p.MATCHNO_ARM) + 1;
                        string jsonAllocDetails = frm["allocDetails"];
                        var serializer = new JavaScriptSerializer();
                        var lstAllocDetails = serializer.Deserialize<List<ARMATCHING>>(jsonAllocDetails);
                        objAllocDetails.USER_ARM = currentUser;
                        objAllocDetails.MATCHNO_ARM = matchNo;
                        _unitOfWork.Repository<ARMATCHING>().Insert(objAllocDetails);
                        _unitOfWork.Save();
                        Proc_ARAPTransaction_MatchValue_Update(Convert.ToString(objAllocDetails.DOCCNUMBER_ARM), Convert.ToDecimal(objAllocDetails.AMOUNT_ARM));
                        foreach (var record in lstAllocDetails)
                        {
                            record.USER_ARM = currentUser;
                            record.MATCHNO_ARM = matchNo;
                            _unitOfWork.Repository<ARMATCHING>().Insert(record);
                            _unitOfWork.Save();
                            Proc_ARAPTransaction_MatchValue_Update(Convert.ToString(record.DOCCNUMBER_ARM), Convert.ToDecimal(record.AMOUNT_ARM));
                        }
                        success = true;
                    }
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
        public JsonResult SaveArUnMatching(ARMATCHING objAllocDetails)
        {
            bool success = false;
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    if (!string.IsNullOrEmpty(objAllocDetails.DOCCNUMBER_ARM))
                    {
                        var invMatchDetails = (from arMatching in _unitOfWork.Repository<ARMATCHING>().Query().Get()
                                               where arMatching.DOCCNUMBER_ARM.Equals(objAllocDetails.DOCCNUMBER_ARM)
                                               select arMatching);
                        var matchNo = invMatchDetails.Max(o => o.MATCHNO_ARM);
                        var arMatchAllocDetails = (from arMatching in _unitOfWork.Repository<ARMATCHING>().Query().Get()
                                                   where arMatching.MATCHNO_ARM == matchNo
                                                   select arMatching).ToList();
                        foreach (var matchVal in arMatchAllocDetails)
                        {
                            Proc_ARAPTransaction_MatchValue_Update(Convert.ToString(matchVal.DOCCNUMBER_ARM), Convert.ToDecimal(matchVal.AMOUNT_ARM), false);
                            //var objMatch = (from arMatching in _unitOfWork.Repository<ARMATCHING>().Query().Get()
                            //                where arMatching.MATCHNO_ARM == matchVal.AMOUNT_ARM && arMatching.DOCCNUMBER_ARM.Equals(matchVal.DOCCNUMBER_ARM)
                            //                select arMatching).SingleOrDefault();
                            _unitOfWork.Repository<ARMATCHING>().Delete(matchVal);
                            _unitOfWork.Save();
                        }
                        success = true;
                    }
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
        public ActionResult AccountsPayableList()
        {
            return View();
        }
        public ActionResult AccountsReceivableList()
        {
            return View();
        }
        public ActionResult GeneralLedgerList()
        {
            return View();
        }
        [HttpPost]
        public JsonResult SaveJvCreation(BANKTRANSACTION objBankTransaction, FormCollection frm)
        {
            string docType = frm["DocType"];
            objBankTransaction.DOCNUMBER_BT = CommonModelAccessUtility.GetDocNo(_unitOfWork, docType);
            string jvNo = objBankTransaction.DOCNUMBER_BT;
            string currentUser = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);
            bool success;
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    string jsonAllocDetails = frm["allocDetails"];
                    var serializer = new JavaScriptSerializer();
                    var lstAllocDetails = serializer.Deserialize<List<CustomJvAllocationDetails>>(jsonAllocDetails);
                    _unitOfWork.Truncate("JVREPORT");
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
                                objApLedger.CREDITAMOUNT_ART = Convert.ToDecimal(allocDetail.Credit);
                                objApLedger.DEBITAMOUNT_ART = Convert.ToDecimal(allocDetail.Debit);
                                objApLedger.OTHERREF_ART = objBankTransaction.OTHERREF_BT;
                                objApLedger.NARRATION_ART = allocDetail.Narration;
                                objApLedger.MATCHVALUE_AR = 0;
                                objApLedger.USER_ART = currentUser;
                                objApLedger.STATUS_ART = "P";
                                _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objApLedger);
                                _unitOfWork.Save();
                                break;
                            case "AR":
                                var objArLedger = _unitOfWork.Repository<AR_AP_LEDGER>().Create();
                                objArLedger.DOCNUMBER_ART = objBankTransaction.DOCNUMBER_BT;
                                objArLedger.DODATE_ART = objBankTransaction.DOCDATE_BT;
                                objArLedger.GLDATE_ART = objBankTransaction.GLDATE_BT;
                                objArLedger.ARAPCODE_ART = allocDetail.AccountCode;
                                objArLedger.CREDITAMOUNT_ART = Convert.ToDecimal(allocDetail.Credit);
                                objArLedger.DEBITAMOUNT_ART = Convert.ToDecimal(allocDetail.Debit);
                                objArLedger.OTHERREF_ART = objBankTransaction.OTHERREF_BT;
                                objArLedger.NARRATION_ART = allocDetail.Narration;
                                objArLedger.MATCHVALUE_AR = 0;
                                objArLedger.USER_ART = currentUser;
                                objArLedger.STATUS_ART = "P";
                                _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objArLedger);
                                _unitOfWork.Save();
                                break;
                            case "BA":
                                var objAccBTransaction = _unitOfWork.Repository<BANKTRANSACTION>().Create();
                                objAccBTransaction.DOCNUMBER_BT = objBankTransaction.DOCNUMBER_BT;
                                objAccBTransaction.BANKCODE_BT = allocDetail.AccountCode;
                                objAccBTransaction.DOCDATE_BT = objBankTransaction.DOCDATE_BT;
                                objAccBTransaction.GLDATE_BT = objBankTransaction.GLDATE_BT;
                                objAccBTransaction.DEBITAMOUT_BT = Convert.ToDecimal(allocDetail.Debit);
                                objAccBTransaction.CREDITAMOUT_BT = Convert.ToDecimal(allocDetail.Credit);
                                objAccBTransaction.OTHERREF_BT = objBankTransaction.OTHERREF_BT;
                                objAccBTransaction.NARRATION_BT = allocDetail.Narration;
                                objAccBTransaction.STATUS_BT = "P";
                                objAccBTransaction.USER_BT = currentUser;
                                _unitOfWork.Repository<BANKTRANSACTION>().Insert(objAccBTransaction);
                                _unitOfWork.Save();
                                break;
                            case "GL":
                                var objGlTransaction = _unitOfWork.Repository<GLTRANSACTION1>().Create();
                                objGlTransaction.DOCNUMBER_GLT = objBankTransaction.DOCNUMBER_BT;
                                objGlTransaction.DOCDATE_GLT = objBankTransaction.DOCDATE_BT;
                                objGlTransaction.GLDATE_GLT = objBankTransaction.GLDATE_BT;
                                objGlTransaction.GLACCODE_GLT = allocDetail.AccountCode;
                                objGlTransaction.OTHERREF_GLT = objBankTransaction.OTHERREF_BT;
                                objGlTransaction.CREDITAMOUNT_GLT = Convert.ToDecimal(allocDetail.Credit);
                                objGlTransaction.DEBITAMOUNT_GLT = Convert.ToDecimal(allocDetail.Debit);
                                objGlTransaction.NARRATION_GLT = allocDetail.Narration;
                                objGlTransaction.VARUSER = currentUser;
                                objGlTransaction.GLSTATUS_GLT = "P";
                                _unitOfWork.Repository<GLTRANSACTION1>().Insert(objGlTransaction);
                                _unitOfWork.Save();
                                break;
                        }
                    }
                    CommonModelAccessUtility.updateDocNo(_unitOfWork, docType);
                    success = true;
                    transaction.Commit();
                }
                catch (Exception)
                {
                    success = false;
                    transaction.Rollback();
                }
            }
            return Json(new { success, jvNo = jvNo }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGlCodes(string sidx, string sord, int page, int rows, string searchValue = "", string searchById = "", string searchByName = "")
        {
            var glCodeList = (from glDetails in _unitOfWork.Repository<GLMASTER>().Query().Get()
                              select glDetails).Select(a => new { a.GLCODE_LM, a.GLDESCRIPTION_LM });
            if (!string.IsNullOrEmpty(searchValue))
            {
                glCodeList = glCodeList.Where(gl => gl.GLDESCRIPTION_LM.Contains(searchValue));
            }
            if (!string.IsNullOrEmpty(searchById))
            {
                glCodeList = glCodeList.Where(gl => gl.GLCODE_LM.Contains(searchById));
            }
            if (!string.IsNullOrEmpty(searchByName))
            {
                glCodeList = glCodeList.Where(gl => gl.GLDESCRIPTION_LM.Contains(searchByName));
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = glCodeList.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / pageSize);
            if (sord.ToUpper() == "ASC")
            {
                glCodeList = glCodeList.OrderBy(a => a.GLCODE_LM);
                glCodeList = glCodeList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                glCodeList = glCodeList.OrderByDescending(a => a.GLCODE_LM);
                glCodeList = glCodeList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = glCodeList

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetGlCodeDetails(string glCode = "")
        {
            Dictionary<string, string> glData = new Dictionary<string, string>();
            try
            {
                var glCodeData = (from glMaster in _unitOfWork.Repository<GLMASTER>().Query().Get()
                                  where (glMaster.GLCODE_LM == glCode)
                                  select new
                                  {
                                      glMaster.GLCODE_LM,
                                      glMaster.GLDESCRIPTION_LM,
                                      glMaster.SUBCODE_LM,
                                      glMaster.MAINCODE_LM,
                                      glMaster.NOTES_LM,
                                      glMaster.GTYPE_LM
                                  }).SingleOrDefault();
                var glCodeDetails = (from glTransaction in _unitOfWork.Repository<GLTRANSACTION1>().Query().Get()
                                     where (glTransaction.GLACCODE_GLT == glCode && glTransaction.GLSTATUS_GLT == "OP")
                                     select new
                                     {
                                         glTransaction.DEBITAMOUNT_GLT,
                                         glTransaction.CREDITAMOUNT_GLT,
                                         glTransaction.GLDATE_GLT
                                     }).SingleOrDefault();

                if (glCodeData != null)
                {
                    glData.Add("glCode", glCodeData.GLCODE_LM);
                    glData.Add("glDesc", glCodeData.GLDESCRIPTION_LM);
                    glData.Add("glSubCode", glCodeData.SUBCODE_LM);
                    glData.Add("glMainCode", glCodeData.MAINCODE_LM);
                    glData.Add("glNotes", glCodeData.NOTES_LM);
                    glData.Add("glType", glCodeData.GTYPE_LM);
                }
                if (glCodeDetails != null)
                {
                    glData.Add("debitAmount", Convert.ToString(glCodeDetails.DEBITAMOUNT_GLT));
                    glData.Add("creditAmount", Convert.ToString(glCodeDetails.CREDITAMOUNT_GLT));
                    glData.Add("glDate", Convert.ToDateTime(glCodeDetails.GLDATE_GLT).ToShortDateString());
                }
                else
                {
                    glData.Add("debitAmount", "");
                    glData.Add("creditAmount", "");
                    glData.Add("glDate", DateTime.Now.ToShortDateString());
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return Json(glData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetInvDetails(string invCode = "")
        {
            try
            {
                var invCodeData = (from bankTransactions in _unitOfWork.Repository<BANKTRANSACTION>().Query().Get()
                                   where (bankTransactions.DOCNUMBER_BT == invCode)
                                   select new
                                   {
                                       bankTransactions.DOCDATE_BT,
                                       bankTransactions.OTHERREF_BT,
                                       bankTransactions.BANKCODE_BT,
                                       bankTransactions.CREDITAMOUT_BT,
                                       bankTransactions.STATUS_BT,
                                       bankTransactions.DEBITAMOUT_BT
                                   }).FirstOrDefault();
                return Json(invCodeData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                // ignored
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        private bool ReverseBankTransactions(BANKTRANSACTION entry)
        {
            bool success;
            string currentUser = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);
            try
            {
                var reverseEntry = _unitOfWork.Repository<BANKTRANSACTION>().Create();
                reverseEntry.DOCNUMBER_BT = "Rev" + entry.DOCNUMBER_BT;
                reverseEntry.BANKCODE_BT = entry.BANKCODE_BT;
                reverseEntry.DOCDATE_BT = entry.DOCDATE_BT;
                reverseEntry.GLDATE_BT = entry.GLDATE_BT;
                reverseEntry.OTHERREF_BT = entry.OTHERREF_BT;
                reverseEntry.CHQNO_BT = entry.CHQNO_BT;
                reverseEntry.CHQDATE_BT = entry.CHQDATE_BT;
                reverseEntry.CLEARANCEDATE_BT = entry.CLEARANCEDATE_BT;
                reverseEntry.BENACNO_BT = entry.BENACNO_BT;
                reverseEntry.BENACCOUNT_BT = entry.BENACCOUNT_BT;
                reverseEntry.NARRATION_BT = entry.NARRATION_BT;
                reverseEntry.NOTE_BT = entry.NOTE_BT;
                reverseEntry.MASTERSTATUS_BT = entry.MASTERSTATUS_BT;
                reverseEntry.CREDITAMOUT_BT = entry.DEBITAMOUT_BT;
                reverseEntry.DEBITAMOUT_BT = entry.CREDITAMOUT_BT;
                reverseEntry.USER_BT = currentUser;
                reverseEntry.STATUS_BT = "R";

                _unitOfWork.Repository<BANKTRANSACTION>().Insert(reverseEntry);
                _unitOfWork.Save();
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }

            return success;
        }
        private bool ReverseAr_Ap_Enteries(AR_AP_LEDGER entry)
        {
            bool success;
            string currentUser = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);
            try
            {
                var reverseEntry = _unitOfWork.Repository<AR_AP_LEDGER>().Create();
                reverseEntry.DOCNUMBER_ART = "Rev" + entry.DOCNUMBER_ART;
                reverseEntry.DODATE_ART = entry.DODATE_ART;
                reverseEntry.GLDATE_ART = entry.GLDATE_ART;
                reverseEntry.ARAPCODE_ART = entry.ARAPCODE_ART;
                reverseEntry.OTHERREF_ART = entry.OTHERREF_ART;
                reverseEntry.NARRATION_ART = entry.NARRATION_ART;
                reverseEntry.MATCHVALUE_AR = entry.MATCHVALUE_AR;
                reverseEntry.CREDITAMOUNT_ART = entry.DEBITAMOUNT_ART;
                reverseEntry.DEBITAMOUNT_ART = entry.CREDITAMOUNT_ART;
                reverseEntry.USER_ART = currentUser;
                reverseEntry.STATUS_ART = "R";
                _unitOfWork.Repository<AR_AP_LEDGER>().Insert(reverseEntry);
                _unitOfWork.Save();
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            return success;
        }
        private bool ReverseGlTransactions(GLTRANSACTION1 entry)
        {
            bool success;
            string currentUser = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);
            try
            {
                var reverseEntry = _unitOfWork.Repository<GLTRANSACTION1>().Create();
                reverseEntry.DOCNUMBER_GLT = "Rev" + entry.DOCNUMBER_GLT;
                reverseEntry.DOCDATE_GLT = entry.DOCDATE_GLT;
                reverseEntry.GLDATE_GLT = entry.GLDATE_GLT;
                reverseEntry.GLACCODE_GLT = entry.GLACCODE_GLT;
                reverseEntry.OTHERREF_GLT = entry.OTHERREF_GLT;
                reverseEntry.NARRATION_GLT = entry.NARRATION_GLT;
                reverseEntry.CREDITAMOUNT_GLT = entry.DEBITAMOUNT_GLT;
                reverseEntry.DEBITAMOUNT_GLT = entry.CREDITAMOUNT_GLT;
                reverseEntry.VARUSER = currentUser;
                reverseEntry.GLSTATUS_GLT = "R";
                _unitOfWork.Repository<GLTRANSACTION1>().Insert(reverseEntry);
                _unitOfWork.Save();
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            return success;
        }
        private void Proc_ARAPTransaction_MatchValue_Update(string docNo, decimal amount, bool isMatching = true)
        {
            try
            {
                var arApLedgerEntry = (from ledgerEntry in _unitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                                       where (ledgerEntry.DOCNUMBER_ART == docNo)
                                       select ledgerEntry).SingleOrDefault();
                if (isMatching)
                {
                    arApLedgerEntry.MATCHVALUE_AR = arApLedgerEntry.MATCHVALUE_AR + amount;
                }
                else
                {
                    arApLedgerEntry.MATCHVALUE_AR = arApLedgerEntry.MATCHVALUE_AR - amount;
                }
                _unitOfWork.Repository<AR_AP_LEDGER>().Update(arApLedgerEntry);
                _unitOfWork.Save();
            }
            catch (Exception e)
            {
            }
        }
        public ActionResult JvPrinting()
        {
            @ViewBag.DocType = CommonModelAccessUtility.GetDocTypes(_unitOfWork);
            return View();
        }
        public ActionResult VoucherPrinting()
        {
            @ViewBag.VoucherTypes = CommonModelAccessUtility.GetVoucherTypes();
            @ViewBag.DocType = CommonModelAccessUtility.GetDocTypes(_unitOfWork);
            return View();
        }
        public JsonResult GetAllocationDetailsDocNo(string sidx, string sord, int page, int rows, string partyId, string docId)
        {
            var repo = _unitOfWork.ExtRepositoryFor<ReportRepository>();
            var allocDocList = repo.sp_GetAllocationDetailsDocList(page, rows, partyId, docId);
            return Json(allocDocList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPartyAllocationDocumentDetails(string partyCode, string docId)
        {
            var arApLedgerList = (from ledgers in _unitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                                  where (ledgers.ARAPCODE_ART.Equals(partyCode) && ledgers.DOCNUMBER_ART.Equals(docId))
                                  select ledgers).FirstOrDefault();
            return Json(arApLedgerList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetArMatchAllocationDetails(string sidx, string sord, int page, int rows, string partyId, bool isCredit)
        {
            if (!String.IsNullOrEmpty(partyId))
            {
                var repo = _unitOfWork.ExtRepositoryFor<ReportRepository>();
                if (isCredit)
                {
                    var allocDocList = repo.sp_GetPartyAllocationCreditDocuments(partyId);
                    return Json(allocDocList, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var allocDocList = repo.sp_GetPartyAllocationDocuments(partyId);
                    return Json(allocDocList, JsonRequestBehavior.AllowGet);
                }

            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetArUnMatchDetails(string sidx, string sord, int page, int rows, string invNo)
        {
            if (!String.IsNullOrEmpty(invNo))
            {
                var invMatchDetails = (from arMatching in _unitOfWork.Repository<ARMATCHING>().Query().Get()
                                       where arMatching.DOCCNUMBER_ARM.Equals(invNo)
                                       select arMatching);
                var matchNo = invMatchDetails.Max(o => o.MATCHNO_ARM);
                var arMatchAllocDetails = (from arMatching in _unitOfWork.Repository<ARMATCHING>().Query().Get()
                                           where arMatching.MATCHNO_ARM == matchNo
                                           select arMatching);
                return Json(arMatchAllocDetails, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDayEndProcessData(DateTime startDate, DateTime endDate)
        {
            decimal varDayEnd_JobTotal = 0;
            decimal varDayEnd_SalesTotal = 0;
            decimal varDayEnd_DiscountTotal = 0;
            decimal varDayEnd_ShippingTotal = 0;
            var unPostedSales = (from sales in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                 where sales.STATUS_SD == "N" && (sales.SALEDATE_SD >= startDate && sales.SALEDATE_SD <= endDate)
                                 select sales).Count();
            if (unPostedSales == 0)
            {
                var jobData = (from sales in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                               join jobs in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get() on sales.JOBID_SD equals jobs.JOBID_JR
                               where (sales.SALEDATE_SD >= startDate && sales.SALEDATE_SD <= endDate)
                               select sales).ToList();
                foreach (var job in jobData)
                {
                    var amount = Convert.ToInt32(job.QTY_SD) * Convert.ToDecimal(job.RATE_SD);
                    varDayEnd_JobTotal += amount;
                }

                var saleData = (from sales in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                join prds in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get() on sales.PRCODE_SD equals prds.PROD_CODE_PM
                                where (sales.SALEDATE_SD >= startDate && sales.SALEDATE_SD <= endDate)
                                select sales).ToList();
                foreach (var sale in saleData)
                {
                    var amount = Convert.ToInt32(sale.QTY_SD) * Convert.ToDecimal(sale.RATE_SD);
                    varDayEnd_SalesTotal += amount;
                }

                varDayEnd_DiscountTotal = Convert.ToDecimal((from sales in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                                             where sales.SALEDATE_SD >= startDate && sales.SALEDATE_SD <= endDate
                                                             select sales).Sum(o => o.DISCOUNT_SD));
                varDayEnd_ShippingTotal = Convert.ToDecimal((from sales in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                                             where sales.SALEDATE_SD >= startDate && sales.SALEDATE_SD <= endDate
                                                             select sales).Sum(o => o.SHIPPINGCHARGES_SD));
            }
            var jsonData = new
            {
                unPostedSales = unPostedSales,
                jobTotal = varDayEnd_JobTotal,
                salesTotal = varDayEnd_SalesTotal,
                discountTotal = varDayEnd_DiscountTotal,
                shippingTotal = varDayEnd_ShippingTotal
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        public ENTRYMASTER GetEntryMasterACcodeDetails(string acode)
        {
            var objEntry = (from entry in _unitOfWork.Repository<ENTRYMASTER>().Query().Get()
                            where entry.ENTRYCODE_EM.Equals(acode)
                            select entry).FirstOrDefault();
            return objEntry;
        }
        public void DayEndUpdate(DateTime startDate, DateTime endDate, string djCode)
        {
            var saleData = (from sales in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                            where sales.SALEDATE_SD >= startDate && sales.SALEDATE_SD <= endDate
                            select sales).ToList();
            foreach (var sale in saleData)
            {
                sale.DAYENDDOC_NO = djCode;
                _unitOfWork.Repository<SALEDETAIL>().Insert(sale);
                _unitOfWork.Save();
            }
        }
    }
}