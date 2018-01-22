using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Domain.Repositories;
using ASI.MGC.FS.ExtendedAPI;
using ASI.MGC.FS.Model;
using ASI.MGC.FS.Models;
using ASI.MGC.FS.WebCommon;
using System.Linq;

namespace ASI.MGC.FS.Controllers
{
    public class CashController : Controller
    {
        readonly IUnitOfWork _unitOfWork;
        readonly TimeZoneInfo tzInfo;
        DateTime today;
        public CashController()
        {
            _unitOfWork = new UnitOfWork();
            tzInfo = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time");
            today = TimeZoneInfo.ConvertTime(DateTime.Now, tzInfo);
        }
        // GET: Cash
        //public ActionResult Index()
        //{
        //    return View();
        //}
        [MesAuthorize("DailyTransactions")]
        public ActionResult CashPayments()
        {
            var objBankTransaction = new BANKTRANSACTION();
            ViewBag.Today = today.ToShortDateString();
            return View(objBankTransaction);
        }
        [MesAuthorize("DailyTransactions")]
        public ActionResult CashMemo()
        {
            var cashMemoCode = CommonModelAccessUtility.GetCashSaleCount(_unitOfWork);
            ViewBag.cashMemoCode = cashMemoCode;
            ViewBag.Today = today.ToShortDateString();
            ViewBag.dlnNumber = CommonModelAccessUtility.GetDeleNumberCount(_unitOfWork);
            var objBankTransaction = new BANKTRANSACTION();
            return View(objBankTransaction);
        }
        [MesAuthorize("DailyTransactions")]
        public ActionResult CashReceipt()
        {
            var objBankTransaction = new BANKTRANSACTION();
            ViewBag.Today = today.ToShortDateString();
            return View(objBankTransaction);
        }

        [HttpPost]
        public JsonResult SaveCashMemo(FormCollection frm, BANKTRANSACTION objBankTransaction)
        {
            bool success;
            string currentUser = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);
            List<string> reportParams = new List<string>();
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    _unitOfWork.Truncate("DELEVERYNOTE_RPT");
                    var mrvNumber = Convert.ToString(frm["MRVNo"]);
                    var cashMemoNumber = CommonModelAccessUtility.GetCashSaleCount(_unitOfWork);
                    var dlnNumber = CommonModelAccessUtility.GetDeleNumberCount(_unitOfWork);
                    var invoiceMrvDetails = CommonModelAccessUtility.GetMrvDetails(mrvNumber, _unitOfWork);
                    reportParams.Add(cashMemoNumber);
                    reportParams.Add(dlnNumber);
                    string jsonProductDetails = frm["saleDetails"];
                    var serializer = new JavaScriptSerializer();
                    var lstSaleDetails = serializer.Deserialize<List<SALEDETAIL>>(jsonProductDetails);
                    objBankTransaction.CHQDATE_BT = Convert.ToDateTime(today.ToShortDateString());
                    objBankTransaction.CLEARANCEDATE_BT = Convert.ToDateTime(today.ToShortDateString());
                    objBankTransaction.USER_BT = currentUser;
                    objBankTransaction.STATUS_BT = "P";
                    objBankTransaction.CREDITAMOUT_BT = 0;
                    objBankTransaction.VAT_BT = Convert.ToDecimal(frm["TotalVAT"]);
                    _unitOfWork.Repository<BANKTRANSACTION>().Insert(objBankTransaction);
                    _unitOfWork.Save();
                    if (!string.IsNullOrEmpty(frm["TotalVAT"]) && Convert.ToDecimal(frm["TotalVAT"]) > 0)
                    {
                        var objVATChrg = _unitOfWork.Repository<GLTRANSACTION1>().Create();
                        objVATChrg.DOCNUMBER_GLT = cashMemoNumber;
                        objVATChrg.DOCDATE_GLT = Convert.ToDateTime(frm["DocDate"]);
                        objVATChrg.GLDATE_GLT = today;
                        objVATChrg.GLACCODE_GLT = "2510";
                        objVATChrg.CREDITAMOUNT_GLT = Convert.ToDecimal(frm["TotalVAT"]);
                        objVATChrg.DEBITAMOUNT_GLT = 0;
                        objVATChrg.OTHERREF_GLT = objBankTransaction.NOTE_BT;
                        objVATChrg.NARRATION_GLT = frm["CustDetail"];
                        objVATChrg.GLSTATUS_GLT = "P";
                        objVATChrg.VARUSER = currentUser;
                        _unitOfWork.Repository<GLTRANSACTION1>().Insert(objVATChrg);
                        _unitOfWork.Save();
                    }
                    var objInvoiceMaster = _unitOfWork.Repository<INVMASTER>().Create();
                    objInvoiceMaster.INVNO_IPM = cashMemoNumber;
                    objInvoiceMaster.INVDATE_IPM = Convert.ToDateTime(today.ToShortDateString());
                    objInvoiceMaster.CUSTNAME_IPM = Convert.ToString(frm["CustDetail"]);
                    objInvoiceMaster.SHIPPING_IPM = Convert.ToDecimal(frm["TotalShipCharges"]);
                    objInvoiceMaster.DISCOUNT_IPM = Convert.ToDecimal(frm["TotalDiscount"]);
                    if (invoiceMrvDetails != null)
                    {
                        objInvoiceMaster.CUSTADDRESS_IPM = invoiceMrvDetails.ADDRESS1_MRV;
                        if (!string.IsNullOrEmpty(Convert.ToString(frm["CustVATNo"])) && string.IsNullOrEmpty(invoiceMrvDetails.CUSTOMERVATNO_MRV))
                        {
                            invoiceMrvDetails.CUSTOMERVATNO_MRV = Convert.ToString(frm["CustVATNo"]);
                            objInvoiceMaster.CUSTVATNO_IPM = "TRN: " + Convert.ToString(frm["CustVATNo"]);
                            _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Update(invoiceMrvDetails);
                            _unitOfWork.Save();
                        }
                    }
                    objInvoiceMaster.CUST_CODE_IPM = "CASH";
                    objInvoiceMaster.VAT_IPM = Convert.ToDecimal(frm["TotalVAT"]);
                    objInvoiceMaster.INVTYPE_IPM = "CM";
                    _unitOfWork.Repository<INVMASTER>().Insert(objInvoiceMaster);
                    _unitOfWork.Save();

                    foreach (SALEDETAIL sale in lstSaleDetails)
                    {
                        if (!string.IsNullOrEmpty(sale.PRCODE_SD))
                        {
                            UpdateSalesStatus(sale.JOBNO_SD, sale.PRCODE_SD, 2, cashMemoNumber, "P");
                            var objStockLedger = _unitOfWork.Repository<STOCKLEDGER>().Create();
                            objStockLedger.DOC_DATE_SL = Convert.ToDateTime(today.ToShortDateString());
                            objStockLedger.LEDGER_DATE_SL = Convert.ToDateTime(today.ToShortDateString());
                            objStockLedger.VOUCHERNO_SL = cashMemoNumber;
                            objStockLedger.OTHERREF_SL = mrvNumber;
                            objStockLedger.PRODID_SL = sale.PRCODE_SD;
                            objStockLedger.ISSUE_QTY_SL = sale.QTY_SD;
                            objStockLedger.ISSUE_RATE_SL = sale.RATE_SD;
                            objStockLedger.UNIT_SL = sale.UNIT_SD;
                            objStockLedger.STATUS_SL = "P";

                        }

                        var objInvoiceDetail = _unitOfWork.Repository<INVDETAIL>().Create();
                        var objDeleveryNote = _unitOfWork.Repository<DELEVERYNOTE_RPT>().Create();
                        objDeleveryNote.DLNR_DLNRPT = dlnNumber;
                        objDeleveryNote.QTY_DLNRPT = sale.QTY_SD;
                        objDeleveryNote.JOBNO_DLNRPT = sale.JOBNO_SD;
                        objDeleveryNote.DLNTYPE_DLNRPT = "CM";
                        if (!string.IsNullOrEmpty(sale.PRCODE_SD))
                        {
                            objInvoiceDetail.CODE_INVD = sale.PRCODE_SD;
                            objDeleveryNote.ID_DLNRPT = sale.PRCODE_SD;
                            var objPrDetail =
                                _unitOfWork.Repository<PRODUCTMASTER>().FindByID(sale.PRCODE_SD).DESCRIPTION_PM;
                            objInvoiceDetail.DESCRIPTION_INVD = objPrDetail;
                            objDeleveryNote.DESCRIPTION_DLNRPT = objPrDetail;
                        }
                        else
                        {
                            UpdateSalesStatus(sale.JOBNO_SD, sale.JOBID_SD, 1, cashMemoNumber, "P");
                            objInvoiceDetail.CODE_INVD = sale.JOBID_SD;
                            objDeleveryNote.ID_DLNRPT = sale.JOBID_SD;
                            var objJobDetail =
                                _unitOfWork.Repository<JOBIDREFERENCE>().FindByID(sale.JOBID_SD).JOBDESCRIPTION_JR;
                            objInvoiceDetail.DESCRIPTION_INVD = objJobDetail;
                            objDeleveryNote.DESCRIPTION_DLNRPT = objJobDetail;
                        }
                        objInvoiceDetail.QTY_INVD = sale.QTY_SD;
                        objInvoiceDetail.RATE_INVD = sale.RATE_SD;
                        objInvoiceDetail.AMOUNT_INVNO = (objInvoiceDetail.QTY_INVD * objInvoiceDetail.RATE_INVD);
                        objInvoiceDetail.JOBNO_INVD = sale.JOBNO_SD;
                        objInvoiceDetail.UNIT_INVD = sale.UNIT_SD;
                        objInvoiceDetail.INVNO_INVD = cashMemoNumber;
                        _unitOfWork.Repository<INVDETAIL>().Insert(objInvoiceDetail);
                        _unitOfWork.Save();

                        //var objSaleDetails = _unitOfWork.Repository<SALEDETAIL>().FindByID(sale.SLNO_SD);
                        //objSaleDetails.CASHRVNO_SD = Convert.ToString(cashMemoNumber);
                        //objSaleDetails.USERID_SD = currentUser;
                        //objSaleDetails.STATUS_SD = "P";
                        //_unitOfWork.Repository<SALEDETAIL>().Update(objSaleDetails);
                        //_unitOfWork.Save();

                        var objJobMaster = _unitOfWork.Repository<JOBMASTER>().FindByID(sale.JOBNO_SD);
                        objDeleveryNote.SERVICEPROID_DLNRPT = objJobMaster.PRODID_JIM;
                        objJobMaster.DELEVERNOTENO_JM = dlnNumber;
                        objJobMaster.JOBSTATUS_JM = "P";
                        _unitOfWork.Repository<JOBMASTER>().Update(objJobMaster);
                        _unitOfWork.Save();
                        _unitOfWork.Repository<DELEVERYNOTE_RPT>().Insert(objDeleveryNote);
                        _unitOfWork.Save();
                    }
                    success = true;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    success = false;
                    transaction.Rollback();
                }
            }
            return Json(new { reportParams, success }, JsonRequestBehavior.AllowGet);
        }
        private void UpdateSalesStatus(string jobNo, string Pr_Job_Code, int choice, string cashMemo_no, string status)
        {
            string currentUser = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);
            if (choice == 1)
            {
                var sale = (from saleList in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                            where saleList.JOBNO_SD.Equals(jobNo) && saleList.JOBID_SD.Equals(Pr_Job_Code)
                            select saleList).FirstOrDefault();
                sale.USERID_SD = currentUser;
                sale.STATUS_SD = status;
                sale.CASHRVNO_SD = cashMemo_no;
                _unitOfWork.Repository<SALEDETAIL>().Update(sale);
                _unitOfWork.Save();
            }
            else if (choice == 2)
            {
                var sale = (from saleList in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                            where saleList.JOBNO_SD.Equals(jobNo) && saleList.PRCODE_SD.Equals(Pr_Job_Code)
                            select saleList).FirstOrDefault();
                sale.USERID_SD = currentUser;
                sale.STATUS_SD = status;
                sale.CASHRVNO_SD = cashMemo_no;
                _unitOfWork.Repository<SALEDETAIL>().Update(sale);
                _unitOfWork.Save();
            }
        }
        [HttpPost]
        public ActionResult SaveCashReceipt(FormCollection form, BANKTRANSACTION objBankTransaction)
        {
            string crNo = "";
            string currentUser = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);
            bool success;
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    string docType = form["DocType"];
                    objBankTransaction.DOCNUMBER_BT = CommonModelAccessUtility.GetDocNo(_unitOfWork, docType);
                    crNo = objBankTransaction.DOCNUMBER_BT;
                    objBankTransaction.USER_BT = currentUser;
                    objBankTransaction.CREDITAMOUT_BT = 0;
                    objBankTransaction.STATUS_BT = "P";
                    objBankTransaction.MASTERSTATUS_BT = "M";
                    _unitOfWork.Repository<BANKTRANSACTION>().Insert(objBankTransaction);
                    _unitOfWork.Save();
                    //_unitOfWork.Truncate("VOUCHERMASTER_RPT");
                    //_unitOfWork.Truncate("VOUCHERCHILD_RPT");
                    //var objVoucherMaster = _unitOfWork.Repository<VOUCHERMASTER_RPT>().Create();
                    //objVoucherMaster.GLDATE_VRPT = objBankTransaction.GLDATE_BT;
                    //objVoucherMaster.ALLCODE_VRPT = "Bank";
                    //objVoucherMaster.BANKCODE_VRT = objBankTransaction.BANKCODE_BT;
                    //objVoucherMaster.ACCDESCRIPTION_VRPT = Convert.ToString(form["BankName"]);
                    //objVoucherMaster.PARTICULARS_VRPT = objBankTransaction.NARRATION_BT;
                    //objVoucherMaster.NOTES_VRPT = objBankTransaction.NOTE_BT;
                    //objVoucherMaster.DEBITAMOUT_VRPT = objBankTransaction.CREDITAMOUT_BT;
                    //objVoucherMaster.CREDITAMOUNT_VRPT = 0;
                    //objVoucherMaster.CHQNO_VRPT = objBankTransaction.CHQNO_BT;
                    //objVoucherMaster.CHQDATE_VRPT = objBankTransaction.CHQDATE_BT;
                    //objVoucherMaster.DOCNO_VRPT = objBankTransaction.DOCNUMBER_BT;
                    //objVoucherMaster.USER_VRPT = currentUser;
                    //objVoucherMaster.VOUCHER_TYPE = "CR";
                    //_unitOfWork.Repository<VOUCHERMASTER_RPT>().Insert(objVoucherMaster);
                    //_unitOfWork.Save();

                    if (Convert.ToBoolean(form["hdnIncludeVAT"]))
                    {
                        var objVATChrg = _unitOfWork.Repository<GLTRANSACTION1>().Create();
                        objVATChrg.DOCNUMBER_GLT = objBankTransaction.DOCNUMBER_BT;
                        objVATChrg.DOCDATE_GLT = objBankTransaction.DOCDATE_BT;
                        objVATChrg.GLDATE_GLT = objBankTransaction.GLDATE_BT;
                        objVATChrg.GLACCODE_GLT = "2510";
                        objVATChrg.CREDITAMOUNT_GLT = 0;
                        objVATChrg.DEBITAMOUNT_GLT = Convert.ToDecimal(form["TotalVAT"]);
                        objVATChrg.OTHERREF_GLT = objBankTransaction.OTHERREF_BT;
                        objVATChrg.NARRATION_GLT = objBankTransaction.NARRATION_BT;
                        objVATChrg.GLSTATUS_GLT = "P";
                        objVATChrg.VARUSER = currentUser;
                        _unitOfWork.Repository<GLTRANSACTION1>().Insert(objVATChrg);
                        _unitOfWork.Save();
                    }

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
                                objBTransaction.STATUS_BT = "P";
                                objBTransaction.USER_BT = currentUser;
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
                                //objGlVoucherChild.DEBITAMOUNT_VCD = 0;
                                //objGlVoucherChild.AMOUNTSTATUTS_VCD = "Dr";
                                //objGlVoucherChild.DOCNO_VCD = objBankTransaction.DOCNUMBER_BT;
                                //_unitOfWork.Repository<VOUCHERCHILD_RPT>().Insert(objGlVoucherChild);
                                //_unitOfWork.Save();
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
            return Json(new { success, crNo }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveCashPayment(FormCollection form, BANKTRANSACTION objBankTransaction)
        {
            string cpNo = "";
            string currentUser = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);
            bool success;
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    string docType = form["DocType"];
                    objBankTransaction.DOCNUMBER_BT = CommonModelAccessUtility.GetDocNo(_unitOfWork, docType);
                    objBankTransaction.USER_BT = currentUser;
                    objBankTransaction.STATUS_BT = "P";
                    objBankTransaction.MASTERSTATUS_BT = "M";
                    _unitOfWork.Repository<BANKTRANSACTION>().Insert(objBankTransaction);
                    _unitOfWork.Save();
                    cpNo = objBankTransaction.DOCNUMBER_BT;
                    //_unitOfWork.Truncate("VOUCHERMASTER_RPT");
                    //_unitOfWork.Truncate("VOUCHERCHILD_RPT");
                    //var objVoucherMaster = _unitOfWork.Repository<VOUCHERMASTER_RPT>().Create();
                    //objVoucherMaster.GLDATE_VRPT = objBankTransaction.GLDATE_BT;
                    //objVoucherMaster.ALLCODE_VRPT = "Bank";
                    //objVoucherMaster.BANKCODE_VRT = objBankTransaction.BANKCODE_BT;
                    //objVoucherMaster.ACCDESCRIPTION_VRPT = Convert.ToString(form["BankName"]);
                    //objVoucherMaster.PARTICULARS_VRPT = objBankTransaction.NARRATION_BT;
                    //objVoucherMaster.NOTES_VRPT = objBankTransaction.NOTE_BT;
                    //objVoucherMaster.DEBITAMOUT_VRPT = 0;
                    //objVoucherMaster.CREDITAMOUNT_VRPT = objBankTransaction.CREDITAMOUT_BT;
                    //objVoucherMaster.CHQNO_VRPT = objBankTransaction.CHQNO_BT;
                    //objVoucherMaster.CHQDATE_VRPT = objBankTransaction.CHQDATE_BT;
                    //objVoucherMaster.DOCNO_VRPT = objBankTransaction.DOCNUMBER_BT;
                    //objVoucherMaster.USER_VRPT = currentUser;
                    //objVoucherMaster.VOUCHER_TYPE = "CP";
                    //_unitOfWork.Repository<VOUCHERMASTER_RPT>().Insert(objVoucherMaster);
                    //_unitOfWork.Save();

                    if (Convert.ToBoolean(form["hdnIncludeVAT"]))
                    {
                        var objVATChrg = _unitOfWork.Repository<GLTRANSACTION1>().Create();
                        objVATChrg.DOCNUMBER_GLT = objBankTransaction.DOCNUMBER_BT;
                        objVATChrg.DOCDATE_GLT = objBankTransaction.DOCDATE_BT;
                        objVATChrg.GLDATE_GLT = objBankTransaction.GLDATE_BT;
                        objVATChrg.GLACCODE_GLT = "3510";
                        objVATChrg.CREDITAMOUNT_GLT = 0;
                        objVATChrg.DEBITAMOUNT_GLT = Convert.ToDecimal(form["TotalVAT"]);
                        objVATChrg.OTHERREF_GLT = objBankTransaction.OTHERREF_BT;
                        objVATChrg.NARRATION_GLT = objBankTransaction.NARRATION_BT;
                        objVATChrg.GLSTATUS_GLT = "P";
                        objVATChrg.VARUSER = currentUser;
                        _unitOfWork.Repository<GLTRANSACTION1>().Insert(objVATChrg);
                        _unitOfWork.Save();
                    }

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
                    success = true;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    success = false;
                    transaction.Rollback();
                }
            }
            return Json(new { success, cpNo }, JsonRequestBehavior.AllowGet);
        }
        [MesAuthorize("DailyTransactions")]
        public ActionResult CashMemoReversal()
        {
            ViewBag.Today = today.ToShortDateString();
            return View();
        }

        [HttpPost]
        public JsonResult SaveCashMemoReversal(BANKTRANSACTION objBankTransaction)
        {
            bool success = false;
            ReportRepository repo = _unitOfWork.ExtRepositoryFor<ReportRepository>();
            string cashReceiptNo = objBankTransaction.DOCNUMBER_BT;
            string revRCTNo = "Rev" + objBankTransaction.DOCNUMBER_BT;
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    // Reversing Bank Transactions
                    var bankTransactionData = (from bankTransaction in _unitOfWork.Repository<BANKTRANSACTION>().Query().Get()
                                               where bankTransaction.DOCNUMBER_BT.Equals(cashReceiptNo)
                                               select bankTransaction).ToList();
                    foreach (var entry in bankTransactionData)
                    {
                        BANKTRANSACTION objBnk = _unitOfWork.Repository<BANKTRANSACTION>().Create();
                        if (Convert.ToDecimal(entry.DEBITAMOUT_BT) > 0)
                        {
                            objBnk.DEBITAMOUT_BT = 0;
                            objBnk.CREDITAMOUT_BT = entry.DEBITAMOUT_BT;
                            objBnk.DOCDATE_BT = DateTime.Now;
                            objBnk.GLDATE_BT = entry.GLDATE_BT;
                            objBnk.DOCNUMBER_BT = revRCTNo;
                            objBnk.BANKCODE_BT = entry.BANKCODE_BT;
                            objBnk.OTHERREF_BT = cashReceiptNo;
                            objBnk.STATUS_BT = "R";
                        }
                        if (Convert.ToDecimal(entry.CREDITAMOUT_BT) > 0)
                        {
                            objBnk.CREDITAMOUT_BT = 0;
                            objBnk.DEBITAMOUT_BT = entry.CREDITAMOUT_BT;
                            objBnk.DOCDATE_BT = DateTime.Now;
                            objBnk.GLDATE_BT = entry.GLDATE_BT;
                            objBnk.DOCNUMBER_BT = revRCTNo;
                            objBnk.BANKCODE_BT = entry.BANKCODE_BT;
                            objBnk.OTHERREF_BT = cashReceiptNo;
                            objBnk.STATUS_BT = "R";
                        }
                        _unitOfWork.Repository<BANKTRANSACTION>().Insert(objBnk);
                        _unitOfWork.Save();

                        entry.STATUS_BT = "R";
                        _unitOfWork.Repository<BANKTRANSACTION>().Update(entry);
                        _unitOfWork.Save();
                    }
                    // Reversing ArApLedger Transactions
                    var ArApLedgerData = (from ArApLedger in _unitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                                          where ArApLedger.DOCNUMBER_ART.Equals(cashReceiptNo)
                                          select ArApLedger).ToList();
                    foreach (var entry in ArApLedgerData)
                    {
                        AR_AP_LEDGER objArAp = _unitOfWork.Repository<AR_AP_LEDGER>().Create();
                        if (Convert.ToDecimal(entry.DEBITAMOUNT_ART) > 0)
                        {
                            objArAp.DOCNUMBER_ART = revRCTNo;
                            objArAp.ARAPCODE_ART = entry.ARAPCODE_ART;
                            objArAp.DODATE_ART = DateTime.Now;
                            objArAp.GLDATE_ART = entry.GLDATE_ART;
                            objArAp.DEBITAMOUNT_ART = 0;
                            objArAp.CREDITAMOUNT_ART = entry.DEBITAMOUNT_ART;
                            objArAp.OTHERREF_ART = cashReceiptNo;
                            objArAp.STATUS_ART = "R";
                        }
                        if (Convert.ToDecimal(entry.CREDITAMOUNT_ART) > 0)
                        {
                            objArAp.DOCNUMBER_ART = revRCTNo;
                            objArAp.ARAPCODE_ART = entry.ARAPCODE_ART;
                            objArAp.DODATE_ART = DateTime.Now;
                            objArAp.GLDATE_ART = entry.GLDATE_ART;
                            objArAp.CREDITAMOUNT_ART = 0;
                            objArAp.DEBITAMOUNT_ART = entry.CREDITAMOUNT_ART;
                            objArAp.OTHERREF_ART = cashReceiptNo;
                            objArAp.STATUS_ART = "R";
                        }
                        _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objArAp);
                        _unitOfWork.Save();

                        entry.STATUS_ART = "R";
                        _unitOfWork.Repository<AR_AP_LEDGER>().Update(entry);
                        _unitOfWork.Save();
                    }
                    // Reversing GL Transactions
                    var glTransactionData = (from glTransaction in _unitOfWork.Repository<GLTRANSACTION1>().Query().Get()
                                             where glTransaction.DOCNUMBER_GLT.Equals(cashReceiptNo)
                                             select glTransaction).ToList();
                    foreach (var entry in glTransactionData)
                    {
                        GLTRANSACTION1 objGlt = _unitOfWork.Repository<GLTRANSACTION1>().Create();
                        if (Convert.ToDecimal(entry.DEBITAMOUNT_GLT) > 0)
                        {
                            objGlt.DOCNUMBER_GLT = revRCTNo;
                            objGlt.GLACCODE_GLT = entry.GLACCODE_GLT;
                            objGlt.DOCDATE_GLT = DateTime.Now;
                            objGlt.GLDATE_GLT = entry.GLDATE_GLT;
                            objGlt.DEBITAMOUNT_GLT = 0;
                            objGlt.CREDITAMOUNT_GLT = entry.DEBITAMOUNT_GLT;
                            objGlt.OTHERREF_GLT = cashReceiptNo;
                            objGlt.GLSTATUS_GLT = "R";
                        }
                        if (Convert.ToDecimal(entry.CREDITAMOUNT_GLT) > 0)
                        {
                            objGlt.DOCNUMBER_GLT = revRCTNo;
                            objGlt.GLACCODE_GLT = entry.GLACCODE_GLT;
                            objGlt.DOCDATE_GLT = DateTime.Now;
                            objGlt.GLDATE_GLT = entry.GLDATE_GLT;
                            objGlt.DEBITAMOUNT_GLT = entry.CREDITAMOUNT_GLT;
                            objGlt.CREDITAMOUNT_GLT = 0;
                            objGlt.OTHERREF_GLT = cashReceiptNo;
                            objGlt.GLSTATUS_GLT = "R";
                        }
                        _unitOfWork.Repository<GLTRANSACTION1>().Insert(objGlt);
                        _unitOfWork.Save();

                        entry.GLSTATUS_GLT = "R";
                        _unitOfWork.Repository<GLTRANSACTION1>().Update(entry);
                        _unitOfWork.Save();
                    }

                    //// Clearing Stock Ledger
                    //var stockLedgerData = (from stockLedger in _unitOfWork.Repository<STOCKLEDGER>().Query().Get()
                    //                       where stockLedger.VOUCHERNO_SL.Equals(cashReceiptNo)
                    //                       select stockLedger).ToList();
                    //foreach (var entry in stockLedgerData)
                    //{
                    //    _unitOfWork.Repository<STOCKLEDGER>().Delete(entry);
                    //    _unitOfWork.Save();
                    //}
                    ////Reversing Job & Sale Data
                    //var saleDetailData = (from saleDetail in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                    //                      where saleDetail.CASHRVNO_SD.Equals(cashReceiptNo)
                    //                      select saleDetail).ToList();
                    //foreach (var entry in saleDetailData)
                    //{
                    //    entry.STATUS_SD = "N";
                    //    entry.CASHRVNO_SD = "";
                    //    var JobData = (from jobMaster in _unitOfWork.Repository<JOBMASTER>().Query().Get()
                    //                   where jobMaster.JOBNO_JM.Equals(entry.JOBNO_SD)
                    //                   select jobMaster).SingleOrDefault();
                    //    if (JobData != null)
                    //    {
                    //        JobData.DELEVERNOTENO_JM = "";
                    //        JobData.JOBSTATUS_JM = "N";
                    //        _unitOfWork.Repository<JOBMASTER>().Update(JobData);
                    //        _unitOfWork.Save();
                    //    }
                    //    _unitOfWork.Repository<SALEDETAIL>().Update(entry);
                    //    _unitOfWork.Save();
                    //}
                    success = true;
                    transaction.Commit();
                    repo.Sp_CashMemoReversal(cashReceiptNo);
                }
                catch (Exception)
                {
                    success = false;
                    transaction.Rollback();
                }
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPostedCashMemo(string sidx, string sord, int page, int rows, string cmNo = null)
        {
            var postedCashMemoList = (from saleDetails in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                      where saleDetails.STATUS_SD.Equals("P") && !string.IsNullOrEmpty(saleDetails.CASHRVNO_SD) && string.IsNullOrEmpty(saleDetails.DAYENDDOC_NO)
                                      select new { saleDetails.CASHRVNO_SD }).Distinct();
            if (!string.IsNullOrEmpty(cmNo))
            {
                postedCashMemoList = postedCashMemoList.Where(o => o.CASHRVNO_SD.Contains(cmNo)).Distinct();
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = postedCashMemoList.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
            if (sord.ToUpper() == "DESC")
            {
                postedCashMemoList = postedCashMemoList.OrderByDescending(a => a.CASHRVNO_SD);
                postedCashMemoList = postedCashMemoList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                postedCashMemoList = postedCashMemoList.OrderBy(a => a.CASHRVNO_SD);
                postedCashMemoList = postedCashMemoList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = postedCashMemoList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult getPostedCMDetails(string cmNo)
        {
            var bankData = (from bankTransaction in _unitOfWork.Repository<BANKTRANSACTION>().Query().Get()
                            where bankTransaction.DOCNUMBER_BT.Equals(cmNo)
                            select new { bankTransaction.DOCNUMBER_BT, bankTransaction.DOCDATE_BT, bankTransaction.OTHERREF_BT, bankTransaction.BANKCODE_BT, bankTransaction.DEBITAMOUT_BT, bankTransaction.STATUS_BT }).FirstOrDefault();
            return Json(bankData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCashMemoMrvList(string sidx, string sord, int page, int rows, string mrvCode, string jobNo, string custName)
        {
            var repo = _unitOfWork.ExtRepositoryFor<ReportRepository>();
            var mrvList = repo.sp_GetCashMemoMrvList(page, rows, mrvCode, custName, jobNo);
            return Json(mrvList, JsonRequestBehavior.AllowGet);
        }
    }
}