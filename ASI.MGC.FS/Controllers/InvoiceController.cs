using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;
using ASI.MGC.FS.WebCommon;

namespace ASI.MGC.FS.Controllers
{
    public class InvoiceController : Controller
    {
        readonly IUnitOfWork _unitOfWork;
        public InvoiceController()
        {
            _unitOfWork = new UnitOfWork();
        }
        // GET: Invoice
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InvoicePreparation()
        {
            var invCount = (1001 + CommonModelAccessUtility.GetInvoiceCount(_unitOfWork));
            string currYear = DateTime.Now.Year.ToString();
            string invoiceCode = Convert.ToString("INV/" + Convert.ToString(invCount) + "/" + currYear);
            ViewBag.invoiceCode = invoiceCode;
            var objArApLedger = new AR_AP_LEDGER();
            return View(objArApLedger);
        }

        public ActionResult SaveInvoice(FormCollection frm, AR_AP_LEDGER objArApLedger)
        {
            string invoiceNumber = Convert.ToString(objArApLedger.DOCNUMBER_ART);
            string jsonProductDetails = frm["saleDetail"];
            var serializer = new JavaScriptSerializer();
            var lstSaleDetails = serializer.Deserialize<List<SALEDETAIL>>(jsonProductDetails);
            objArApLedger.STATUS_ART = "P";
            _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objArApLedger);
            _unitOfWork.Save();

            var objInvoiceMaster = _unitOfWork.Repository<INVMASTER>().Create();
            objInvoiceMaster.INVNO_IPM = invoiceNumber;
            objInvoiceMaster.INVDATE_IPM = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            objInvoiceMaster.CUSTNAME_IPM = Convert.ToString(frm["CustDetail"]);
            objInvoiceMaster.SHIPPING_IPM = Convert.ToInt32(frm["TotalShipCharges"]);
            objInvoiceMaster.DISCOUNT_IPM = Convert.ToInt32(frm["TotalDiscount"]);
            _unitOfWork.Repository<INVMASTER>().Insert(objInvoiceMaster);
            _unitOfWork.Save();

            foreach (var sale in lstSaleDetails)
            {
                if (!string.IsNullOrEmpty(sale.PRCODE_SD))
                {
                    var objStockLedger = _unitOfWork.Repository<STOCKLEDGER>().Create();
                    objStockLedger.DOC_DATE_SL = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                    objStockLedger.LEDGER_DATE_SL = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                    objStockLedger.VOUCHERNO_SL = objArApLedger.DOCNUMBER_ART;
                    objStockLedger.OTHERREF_SL = objArApLedger.NARRATION_ART;
                    objStockLedger.PRODID_SL = sale.PRCODE_SD;
                    objStockLedger.ISSUE_QTY_SL = sale.QTY_SD;
                    objStockLedger.ISSUE_RATE_SL = sale.RATE_SD;
                    objStockLedger.UNIT_SL = sale.UNIT_SD;
                    objStockLedger.STATUS_SL = "P";
                    _unitOfWork.Repository<STOCKLEDGER>().Insert(objStockLedger);
                    _unitOfWork.Save();
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
                objInvoiceDetail.AMOUNT_INVNO = (objInvoiceDetail.QTY_INVD * objInvoiceDetail.RATE_INVD);
                objInvoiceDetail.INVNO_INVD = invoiceNumber;
                objInvoiceDetail.JOBNO_INVD = sale.JOBNO_SD;
                objInvoiceDetail.UNIT_INVD = sale.UNIT_SD;
                _unitOfWork.Repository<INVDETAIL>().Insert(objInvoiceDetail);
                _unitOfWork.Save();
            }

            return View("InvoicePreparation");
        }

        public ActionResult InvoiceRevesal()
        {
            return View();
        }

        public ActionResult InvoicePendings()
        {
            return View();
        }

        public ActionResult SaveInvoiceReversal()
        {
            throw new NotImplementedException();
        }

        public ActionResult PendingInvoices()
        {
            return View();
        }
    }
}