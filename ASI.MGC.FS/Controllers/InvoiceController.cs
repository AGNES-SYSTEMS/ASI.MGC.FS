using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Domain.Repositories;
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
            ViewBag.dlnNumber = CommonModelAccessUtility.GetDlnNumber(_unitOfWork);
            var objArApLedger = new AR_AP_LEDGER();
            return View(objArApLedger);
        }

        public JsonResult SaveInvoice(FormCollection frm, AR_AP_LEDGER objArApLedger)
        {
            string invNumber = "";
            string currentUser = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);
            try
            {
                string invoiceNumber = Convert.ToString(objArApLedger.DOCNUMBER_ART);
                string jsonProductDetails = frm["saleDetail"];
                var serializer = new JavaScriptSerializer();
                var lstSaleDetails = serializer.Deserialize<List<SALEDETAIL>>(jsonProductDetails);
                objArApLedger.STATUS_ART = "P";
                objArApLedger.USER_ART = currentUser;
                _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objArApLedger);
                _unitOfWork.Save();
                var objInvoiceMaster = _unitOfWork.Repository<INVMASTER>().Create();
                objInvoiceMaster.INVNO_IPM = invoiceNumber;
                objInvoiceMaster.INVDATE_IPM = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                objInvoiceMaster.CUSTNAME_IPM = Convert.ToString(frm["CustDetail"]);
                objInvoiceMaster.SHIPPING_IPM = Convert.ToInt32(frm["TotalShipCharges"]);
                objInvoiceMaster.DISCOUNT_IPM = Convert.ToInt32(frm["TotalDiscount"]);
                objInvoiceMaster.INVTYPE_IPM = "INV";
                _unitOfWork.Repository<INVMASTER>().Insert(objInvoiceMaster);
                _unitOfWork.Save();
                invNumber = objInvoiceMaster.INVNO_IPM;

                UpdateSalesStatus(objArApLedger);

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
                    objInvoiceDetail.AMOUNT_INVNO = (objInvoiceDetail.QTY_INVD*objInvoiceDetail.RATE_INVD);
                    objInvoiceDetail.INVNO_INVD = invoiceNumber;
                    objInvoiceDetail.JOBNO_INVD = sale.JOBNO_SD;
                    objInvoiceDetail.UNIT_INVD = sale.UNIT_SD;
                    _unitOfWork.Repository<INVDETAIL>().Insert(objInvoiceDetail);
                    _unitOfWork.Save();

                    var objJobMaster = _unitOfWork.Repository<JOBMASTER>().FindByID(sale.JOBNO_SD);
                    objJobMaster.DELEVERNOTENO_JM = Convert.ToString(frm["DLNNo"]);
                    objJobMaster.JOBSTATUS_JM = "P";
                    _unitOfWork.Repository<JOBMASTER>().Update(objJobMaster);
                    _unitOfWork.Save();
                }
            }
            catch (Exception)
            {
                // ignored
            }
           return Json(invNumber, JsonRequestBehavior.AllowGet);
        }

        private void UpdateSalesStatus(AR_AP_LEDGER objArApLedger)
        {
            string currentUser = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);
            var sales = (from saleList in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                         where saleList.MRVNO_SD.Equals(objArApLedger.NARRATION_ART)
                         select saleList);
            foreach (var sale in sales.ToList())
            {
                sale.USERID_SD = currentUser;
                sale.STATUS_SD = "P";
                sale.INVNO_SD = objArApLedger.DOCNUMBER_ART;
                _unitOfWork.Repository<SALEDETAIL>().Update(sale);
                _unitOfWork.Save();
            }
        }

        public ActionResult InvoiceReversal()
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
        public JsonResult GetInvoicePrepMrvList(string sidx, string sord, int page, int rows, string mrvCode = null, string jobNo = null, string custName = null)
        {
            var repo = _unitOfWork.ExtRepositoryFor<ReportRepository>();
            var mrvList = repo.sp_GetInvoicePreparationMrvList(page, rows, mrvCode, custName, jobNo);
            return Json(mrvList, JsonRequestBehavior.AllowGet);
        }
    }
}