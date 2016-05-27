using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;
using ASI.MGC.FS.Models;
using ASI.MGC.FS.WebCommon;

namespace ASI.MGC.FS.Controllers
{
    public class CashController : Controller
    {
        readonly IUnitOfWork _unitOfWork;
        public CashController()
        {
            _unitOfWork = new UnitOfWork();
        }
        // GET: Cash
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult CashPayments()
        {
            var objBankTransaction = new BANKTRANSACTION();
            return View(objBankTransaction);
        }

        public ActionResult CashMemo()
        {
            var cmCount = (1001 + CommonModelAccessUtility.GetCashSaleCount(_unitOfWork));
            string currYear = DateTime.Now.Year.ToString();
            string cashMemoCode = Convert.ToString("RCT/" + Convert.ToString(cmCount) + "/" + currYear);
            ViewBag.cashMemoCode = cashMemoCode;
            var objBankTransaction = new BANKTRANSACTION();
            return View(objBankTransaction);
        }

        public ActionResult CashReceipt()
        {
            var objBankTransaction = new BANKTRANSACTION();
            return View(objBankTransaction);
        }

        [HttpPost]
        public JsonResult SaveCashMemo(FormCollection frm, BANKTRANSACTION objBankTransaction)
        {
            bool success;
            string cashMemoNumber = "";
            try
            {
                var mrvNumber = Convert.ToString(frm["MRVNo"]);
                cashMemoNumber = objBankTransaction.DOCNUMBER_BT;
                string jsonProductDetails = frm["saleDetails"];
                var serializer = new JavaScriptSerializer();
                var lstSaleDetails = serializer.Deserialize<List<SALEDETAIL>>(jsonProductDetails);
                objBankTransaction.CHQDATE_BT = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                objBankTransaction.CLEARANCEDATE_BT = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                objBankTransaction.STATUS_BT = "P";
                _unitOfWork.Repository<BANKTRANSACTION>().Insert(objBankTransaction);
                _unitOfWork.Save();

                var objInvoiceMaster = _unitOfWork.Repository<INVMASTER>().Create();
                objInvoiceMaster.INVNO_IPM = cashMemoNumber;
                objInvoiceMaster.INVDATE_IPM = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                objInvoiceMaster.CUSTNAME_IPM = Convert.ToString(frm["CustDetail"]);
                objInvoiceMaster.SHIPPING_IPM = Convert.ToInt32(frm["TotalShipCharges"]);
                objInvoiceMaster.DISCOUNT_IPM = Convert.ToInt32(frm["TotalDiscount"]);
                objInvoiceMaster.INVTYPE_IPM = "CM";
                _unitOfWork.Repository<INVMASTER>().Insert(objInvoiceMaster);
                _unitOfWork.Save();

                foreach (SALEDETAIL sale in lstSaleDetails)
                {
                    if (!string.IsNullOrEmpty(sale.PRCODE_SD))
                    {
                        var objStockLedger = _unitOfWork.Repository<STOCKLEDGER>().Create();
                        objStockLedger.DOC_DATE_SL = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                        objStockLedger.LEDGER_DATE_SL = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                        objStockLedger.VOUCHERNO_SL = cashMemoNumber;
                        objStockLedger.OTHERREF_SL = mrvNumber;
                        objStockLedger.PRODID_SL = sale.PRCODE_SD;
                        objStockLedger.ISSUE_QTY_SL = sale.QTY_SD;
                        objStockLedger.ISSUE_RATE_SL = sale.RATE_SD;
                        objStockLedger.UNIT_SL = sale.UNIT_SD;
                        objStockLedger.STATUS_SL = "P";

                    }

                    var objInvoiceDetail = _unitOfWork.Repository<INVDETAIL>().Create();
                    if (!string.IsNullOrEmpty(sale.PRCODE_SD))
                    {
                        objInvoiceDetail.CODE_INVD = sale.PRCODE_SD;
                        var objPrDetail =
                            _unitOfWork.Repository<PRODUCTMASTER>().FindByID(sale.PRCODE_SD).DESCRIPTION_PM;
                        objInvoiceDetail.DESCRIPTION_INVD = objPrDetail;
                    }
                    else
                    {
                        objInvoiceDetail.CODE_INVD = sale.JOBID_SD;
                        var objJobDetail =
                            _unitOfWork.Repository<JOBIDREFERENCE>().FindByID(sale.JOBID_SD).JOBDESCRIPTION_JR;
                        objInvoiceDetail.DESCRIPTION_INVD = objJobDetail;
                    }
                    objInvoiceDetail.QTY_INVD = sale.QTY_SD;
                    objInvoiceDetail.RATE_INVD = sale.RATE_SD;
                    objInvoiceDetail.AMOUNT_INVNO = (objInvoiceDetail.QTY_INVD * objInvoiceDetail.RATE_INVD);
                    objInvoiceDetail.JOBNO_INVD = sale.JOBNO_SD;
                    objInvoiceDetail.UNIT_INVD = sale.UNIT_SD;
                    objInvoiceDetail.INVNO_INVD = cashMemoNumber;
                    _unitOfWork.Repository<INVDETAIL>().Insert(objInvoiceDetail);
                    _unitOfWork.Save();

                    var objSaleDetails = _unitOfWork.Repository<SALEDETAIL>().FindByID(sale.SLNO_SD);
                    objSaleDetails.CASHRVNO_SD = Convert.ToString(cashMemoNumber);
                    objSaleDetails.STATUS_SD = "P";
                    _unitOfWork.Repository<SALEDETAIL>().Update(objSaleDetails);
                    _unitOfWork.Save();

                    var objJobMaster = _unitOfWork.Repository<JOBMASTER>().FindByID(sale.JOBNO_SD);
                    objJobMaster.DELEVERNOTENO_JM = Convert.ToString(frm["DLNNo"]);
                    objJobMaster.JOBSTATUS_JM = "P";
                    _unitOfWork.Repository<JOBMASTER>().Update(objJobMaster);
                    _unitOfWork.Save();
                }
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            return Json(new { cashMemoNumber, success }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveCashReceipt(FormCollection form, BANKTRANSACTION objBankTransaction)
        {
            string crNo = "";
            bool success;
            try
            {
                crNo = objBankTransaction.DOCNUMBER_BT;
                objBankTransaction.STATUS_BT = "P";
                objBankTransaction.MASTERSTATUS_BT = "M";
                _unitOfWork.Repository<BANKTRANSACTION>().Insert(objBankTransaction);
                _unitOfWork.Save();
                _unitOfWork.Truncate("VOUCHERMASTER_RPT");
                _unitOfWork.Truncate("VOUCHERCHILD_RPT");
                var objVoucherMaster = _unitOfWork.Repository<VOUCHERMASTER_RPT>().Create();
                objVoucherMaster.GLDATE_VRPT = objBankTransaction.GLDATE_BT;
                objVoucherMaster.ALLCODE_VRPT = "Bank";
                objVoucherMaster.BANKCODE_VRT = objBankTransaction.BANKCODE_BT;
                objVoucherMaster.ACCDESCRIPTION_VRPT = Convert.ToString(form["BankName"]);
                objVoucherMaster.PARTICULARS_VRPT = objBankTransaction.NARRATION_BT;
                objVoucherMaster.NOTES_VRPT = objBankTransaction.NOTE_BT;
                objVoucherMaster.DEBITAMOUT_VRPT = objBankTransaction.CREDITAMOUT_BT;
                objVoucherMaster.CREDITAMOUNT_VRPT = 0;
                objVoucherMaster.CHQNO_VRPT = objBankTransaction.CHQNO_BT;
                objVoucherMaster.CHQDATE_VRPT = objBankTransaction.CHQDATE_BT;
                objVoucherMaster.DOCNO_VRPT = objBankTransaction.DOCNUMBER_BT;
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
                            objGlVoucherChild.DEBITAMOUNT_VCD = 0;
                            objGlVoucherChild.AMOUNTSTATUTS_VCD = "Dr";
                            objGlVoucherChild.DOCNO_VCD = objBankTransaction.DOCNUMBER_BT;
                            _unitOfWork.Repository<VOUCHERCHILD_RPT>().Insert(objGlVoucherChild);
                            _unitOfWork.Save();
                            break;
                    }
                }
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            return Json(new { success, crNo }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveCashPayment(FormCollection form, BANKTRANSACTION objBankTransaction)
        {
            string cpNo = "";
            bool success;
            try
            {
                objBankTransaction.STATUS_BT = "P";
                objBankTransaction.MASTERSTATUS_BT = "M";
                _unitOfWork.Repository<BANKTRANSACTION>().Insert(objBankTransaction);
                _unitOfWork.Save();
                cpNo = objBankTransaction.DOCNUMBER_BT;
                _unitOfWork.Truncate("VOUCHERMASTER_RPT");
                _unitOfWork.Truncate("VOUCHERCHILD_RPT");
                var objVoucherMaster = _unitOfWork.Repository<VOUCHERMASTER_RPT>().Create();
                objVoucherMaster.GLDATE_VRPT = objBankTransaction.GLDATE_BT;
                objVoucherMaster.ALLCODE_VRPT = "Bank";
                objVoucherMaster.BANKCODE_VRT = objBankTransaction.BANKCODE_BT;
                objVoucherMaster.ACCDESCRIPTION_VRPT = Convert.ToString(form["BankName"]);
                objVoucherMaster.PARTICULARS_VRPT = objBankTransaction.NARRATION_BT;
                objVoucherMaster.NOTES_VRPT = objBankTransaction.NOTE_BT;
                objVoucherMaster.DEBITAMOUT_VRPT = 0;
                objVoucherMaster.CREDITAMOUNT_VRPT = objBankTransaction.CREDITAMOUT_BT;
                objVoucherMaster.CHQNO_VRPT = objBankTransaction.CHQNO_BT;
                objVoucherMaster.CHQDATE_VRPT = objBankTransaction.CHQDATE_BT;
                objVoucherMaster.DOCNO_VRPT = objBankTransaction.DOCNUMBER_BT;
                objVoucherMaster.VOUCHER_TYPE = "CP";
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
                            objArLedger.DEBITAMOUNT_ART = Convert.ToInt32(allocDetail.Amount);
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
                            objBTransaction.CREDITAMOUT_BT = Convert.ToDecimal(allocDetail.Amount);
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
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            return Json(new {success,cpNo}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CashMemoReversal()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SaveCashMemoReversal()
        {
            throw new NotImplementedException();
        }
    }
}