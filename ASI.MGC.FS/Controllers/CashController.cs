using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;
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
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CashPayments()
        {
            return View();
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
            return View();
        }

        public ActionResult SaveCashMemo(FormCollection frm, BANKTRANSACTION objBankTransaction)
        {
            var mrvNumber = Convert.ToString(frm["MRVNo"]);
            var cashMemoNumber = objBankTransaction.DOCNUMBER_BT;
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
                    var objPrDetail = _unitOfWork.Repository<PRODUCTMASTER>().FindByID(sale.PRCODE_SD).DESCRIPTION_PM;
                    objInvoiceDetail.DESCRIPTION_INVD = objPrDetail;
                }
                else
                {
                    objInvoiceDetail.CODE_INVD = sale.JOBID_SD;
                    var objJobDetail = _unitOfWork.Repository<JOBIDREFERENCE>().FindByID(sale.JOBID_SD).JOBDESCRIPTION_JR;
                    objInvoiceDetail.DESCRIPTION_INVD = objJobDetail;
                }
                objInvoiceDetail.QTY_INVD = sale.QTY_SD;
                objInvoiceDetail.RATE_INVD = sale.RATE_SD;
                objInvoiceDetail.AMOUNT_INVNO = (objInvoiceDetail.QTY_INVD*objInvoiceDetail.RATE_INVD);
                objInvoiceDetail.JOBNO_INVD = sale.JOBNO_SD;
                objInvoiceDetail.UNIT_INVD = sale.UNIT_SD;
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


            return View("CashMemo");
        }
    }
}