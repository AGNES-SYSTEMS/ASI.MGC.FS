using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Domain.Repositories;
using ASI.MGC.FS.ExtendedAPI;
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
        [MesAuthorize("DailyTransactions")]
        public ActionResult InvoicePreparation()
        {
            string invoiceCode = CommonModelAccessUtility.GetInvoiceCount(_unitOfWork);
            ViewBag.invoiceCode = invoiceCode;
            ViewBag.dlnNumber = CommonModelAccessUtility.GetDeleNumberCount(_unitOfWork);
            var objArApLedger = new AR_AP_LEDGER();
            return View(objArApLedger);
        }
        [HttpPost]
        public JsonResult SaveInvoice(FormCollection frm, AR_AP_LEDGER objArApLedger)
        {
            List<string> reportParams = new List<string>();
            string currentUser = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    _unitOfWork.Truncate("DELEVERYNOTE_RPT");
                    var mrvNumber = Convert.ToString(objArApLedger.NARRATION_ART);
                    var invNumber = CommonModelAccessUtility.GetInvoiceCount(_unitOfWork);
                    objArApLedger.DOCNUMBER_ART = invNumber;
                    reportParams.Add(invNumber);
                    var dlnNumber = CommonModelAccessUtility.GetDeleNumberCount(_unitOfWork);
                    reportParams.Add(dlnNumber);
                    string jsonProductDetails = frm["saleDetail"];
                    var serializer = new JavaScriptSerializer();
                    var lstSaleDetails = serializer.Deserialize<List<SALEDETAIL>>(jsonProductDetails);
                    objArApLedger.STATUS_ART = "P";
                    objArApLedger.USER_ART = currentUser;
                    _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objArApLedger);
                    _unitOfWork.Save();
                    var objInvoiceMaster = _unitOfWork.Repository<INVMASTER>().Create();
                    objInvoiceMaster.INVNO_IPM = invNumber;
                    objInvoiceMaster.INVDATE_IPM = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                    objInvoiceMaster.CUSTNAME_IPM = Convert.ToString(frm["CustDetail"]);
                    objInvoiceMaster.SHIPPING_IPM = Convert.ToDecimal(frm["TotalShipCharges"]);
                    objInvoiceMaster.DISCOUNT_IPM = Convert.ToDecimal(frm["TotalDiscount"]);
                    objInvoiceMaster.INVTYPE_IPM = "INV";
                    objInvoiceMaster.LPONO_IPM = _unitOfWork.Repository<MATERIALRECEIPTMASTER>().FindByID(mrvNumber).NOTES_MRV;
                    _unitOfWork.Repository<INVMASTER>().Insert(objInvoiceMaster);
                    _unitOfWork.Save();

                    foreach (var sale in lstSaleDetails)
                    {
                        if (!string.IsNullOrEmpty(sale.PRCODE_SD))
                        {
                            UpdateSalesStatus(sale.JOBNO_SD, sale.PRCODE_SD, 2, invNumber, "P");
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
                        var objDeleveryNote = _unitOfWork.Repository<DELEVERYNOTE_RPT>().Create();
                        objDeleveryNote.DLNR_DLNRPT = Convert.ToString(frm["DLNNo"]);
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
                            UpdateSalesStatus(sale.JOBNO_SD, sale.JOBID_SD, 1, invNumber, "P");
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
                        objInvoiceDetail.INVNO_INVD = invNumber;
                        objInvoiceDetail.JOBNO_INVD = sale.JOBNO_SD;
                        objInvoiceDetail.UNIT_INVD = sale.UNIT_SD;
                        _unitOfWork.Repository<INVDETAIL>().Insert(objInvoiceDetail);
                        _unitOfWork.Save();

                        var objJobMaster = _unitOfWork.Repository<JOBMASTER>().FindByID(sale.JOBNO_SD);
                        objDeleveryNote.SERVICEPROID_DLNRPT = objJobMaster.PRODID_JIM;
                        objJobMaster.DELEVERNOTENO_JM = Convert.ToString(frm["DLNNo"]);
                        objJobMaster.JOBSTATUS_JM = "P";
                        _unitOfWork.Repository<JOBMASTER>().Update(objJobMaster);
                        _unitOfWork.Save();
                        _unitOfWork.Repository<DELEVERYNOTE_RPT>().Insert(objDeleveryNote);
                        _unitOfWork.Save();
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            return Json(reportParams, JsonRequestBehavior.AllowGet);
        }

        private void UpdateSalesStatus(string jobNo, string Pr_Job_Code, int choice, string inv_no, string status)
        {
            string currentUser = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);
            if (choice == 1)
            {
                var sale = (from saleList in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                            where saleList.JOBNO_SD.Equals(jobNo) && saleList.JOBID_SD.Equals(Pr_Job_Code)
                            select saleList).FirstOrDefault();
                sale.USERID_SD = currentUser;
                sale.STATUS_SD = status;
                sale.INVNO_SD = inv_no;
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
                sale.INVNO_SD = inv_no;
                _unitOfWork.Repository<SALEDETAIL>().Update(sale);
                _unitOfWork.Save();
            }
        }

        [MesAuthorize("DailyTransactions")]
        public ActionResult InvoiceReversal()
        {
            return View();
        }
        [MesAuthorize("DailyTransactions")]
        public ActionResult InvoicePendings()
        {
            return View();
        }
        [HttpPost]
        public JsonResult SaveInvoiceReversal(SALEDETAIL objSaleDetail)
        {
            bool success = false;
            string invNo = objSaleDetail.INVNO_SD;
            string revInvNo = "Rev" + objSaleDetail.INVNO_SD;
            bool isSaleClose = false;
            var salesByInvoice = (from saleDetail in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                  where saleDetail.INVNO_SD.Equals(invNo) && string.IsNullOrEmpty(saleDetail.DAYENDDOC_NO)
                                  select saleDetail).FirstOrDefault();
            if (salesByInvoice != null)
            {
                var dayEndData = (from glDetail in _unitOfWork.Repository<GLTRANSACTION1>().Query().Get()
                                  where glDetail.DOCNUMBER_GLT.Equals(salesByInvoice.DAYENDDOC_NO)
                                  select glDetail).FirstOrDefault();
                if (dayEndData != null)
                {
                    if (dayEndData.GLDATE_GLT >= objSaleDetail.SALEDATE_SD)
                    {
                        isSaleClose = true;
                    }
                    else
                    {
                        isSaleClose = false;
                    }
                }
            }
            if (!isSaleClose)
            {
                using (var transaction = _unitOfWork.BeginTransaction())
                {
                    try
                    {
                        // Reversing Bank Transactions
                        var bankTransactionData = (from bankTransaction in _unitOfWork.Repository<BANKTRANSACTION>().Query().Get()
                                                   where bankTransaction.DOCNUMBER_BT.Equals(invNo)
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
                                objBnk.DOCNUMBER_BT = revInvNo;
                                objBnk.BANKCODE_BT = entry.BANKCODE_BT;
                                objBnk.OTHERREF_BT = invNo;
                                objBnk.STATUS_BT = "R";
                            }
                            if (Convert.ToDecimal(entry.CREDITAMOUT_BT) > 0)
                            {
                                objBnk.CREDITAMOUT_BT = 0;
                                objBnk.DEBITAMOUT_BT = entry.CREDITAMOUT_BT;
                                objBnk.DOCDATE_BT = DateTime.Now;
                                objBnk.GLDATE_BT = entry.GLDATE_BT;
                                objBnk.DOCNUMBER_BT = revInvNo;
                                objBnk.BANKCODE_BT = entry.BANKCODE_BT;
                                objBnk.OTHERREF_BT = invNo;
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
                                              where ArApLedger.DOCNUMBER_ART.Equals(invNo)
                                              select ArApLedger).ToList();
                        foreach (var entry in ArApLedgerData)
                        {
                            AR_AP_LEDGER objArAp = _unitOfWork.Repository<AR_AP_LEDGER>().Create();
                            if (Convert.ToDecimal(entry.DEBITAMOUNT_ART) > 0)
                            {
                                objArAp.DOCNUMBER_ART = revInvNo;
                                objArAp.ARAPCODE_ART = entry.ARAPCODE_ART;
                                objArAp.DODATE_ART = DateTime.Now;
                                objArAp.GLDATE_ART = entry.GLDATE_ART;
                                objArAp.DEBITAMOUNT_ART = 0;
                                objArAp.CREDITAMOUNT_ART = entry.DEBITAMOUNT_ART;
                                objArAp.OTHERREF_ART = invNo;
                                objArAp.STATUS_ART = "R";
                            }
                            if (Convert.ToDecimal(entry.CREDITAMOUNT_ART) > 0)
                            {
                                objArAp.DOCNUMBER_ART = revInvNo;
                                objArAp.ARAPCODE_ART = entry.ARAPCODE_ART;
                                objArAp.DODATE_ART = DateTime.Now;
                                objArAp.GLDATE_ART = entry.GLDATE_ART;
                                objArAp.CREDITAMOUNT_ART = 0;
                                objArAp.DEBITAMOUNT_ART = entry.CREDITAMOUNT_ART;
                                objArAp.OTHERREF_ART = invNo;
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
                                                 where glTransaction.DOCNUMBER_GLT.Equals(invNo)
                                                 select glTransaction).ToList();
                        foreach (var entry in glTransactionData)
                        {
                            GLTRANSACTION1 objGlt = _unitOfWork.Repository<GLTRANSACTION1>().Create();
                            if (Convert.ToDecimal(entry.DEBITAMOUNT_GLT) > 0)
                            {
                                objGlt.DOCNUMBER_GLT = revInvNo;
                                objGlt.GLACCODE_GLT = entry.GLACCODE_GLT;
                                objGlt.DOCDATE_GLT = DateTime.Now;
                                objGlt.GLDATE_GLT = entry.GLDATE_GLT;
                                objGlt.DEBITAMOUNT_GLT = 0;
                                objGlt.CREDITAMOUNT_GLT = entry.DEBITAMOUNT_GLT;
                                objGlt.OTHERREF_GLT = invNo;
                                objGlt.GLSTATUS_GLT = "R";
                            }
                            if (Convert.ToDecimal(entry.CREDITAMOUNT_GLT) > 0)
                            {
                                objGlt.DOCNUMBER_GLT = revInvNo;
                                objGlt.GLACCODE_GLT = entry.GLACCODE_GLT;
                                objGlt.DOCDATE_GLT = DateTime.Now;
                                objGlt.GLDATE_GLT = entry.GLDATE_GLT;
                                objGlt.DEBITAMOUNT_GLT = entry.CREDITAMOUNT_GLT;
                                objGlt.CREDITAMOUNT_GLT = 0;
                                objGlt.OTHERREF_GLT = invNo;
                                objGlt.GLSTATUS_GLT = "R";
                            }
                            _unitOfWork.Repository<GLTRANSACTION1>().Insert(objGlt);
                            _unitOfWork.Save();

                            entry.GLSTATUS_GLT = "R";
                            _unitOfWork.Repository<GLTRANSACTION1>().Update(entry);
                            _unitOfWork.Save();
                        }
                        // Clearing Stock Ledger
                        var stockLedgerData = (from stockLedger in _unitOfWork.Repository<STOCKLEDGER>().Query().Get()
                                               where stockLedger.VOUCHERNO_SL.Equals(invNo)
                                               select stockLedger).ToList();
                        foreach (var entry in stockLedgerData)
                        {
                            _unitOfWork.Repository<STOCKLEDGER>().Delete(entry);
                            _unitOfWork.Save();
                        }
                        //Reversing Job & Sale Data
                        var saleDetailData = (from saleDetail in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                              where saleDetail.INVNO_SD.Equals(invNo)
                                              select saleDetail).ToList();
                        foreach (var entry in saleDetailData)
                        {
                            entry.STATUS_SD = "N";
                            entry.INVNO_SD = "";
                            var JobData = (from jobMaster in _unitOfWork.Repository<JOBMASTER>().Query().Get()
                                           where jobMaster.JOBNO_JM.Equals(entry.JOBNO_SD)
                                           select jobMaster).SingleOrDefault();
                            if (JobData != null)
                            {
                                JobData.DELEVERNOTENO_JM = "";
                                JobData.JOBSTATUS_JM = "N";
                                _unitOfWork.Repository<JOBMASTER>().Update(JobData);
                                _unitOfWork.Save();
                            }
                            _unitOfWork.Repository<SALEDETAIL>().Update(entry);
                            _unitOfWork.Save();
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
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        [MesAuthorize("DailyTransactions")]
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
        public JsonResult GetPendingInvoices(string sidx, string sord, int page, int rows)
        {
            var pendingInvoicesList = (from saleDetails in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                       where saleDetails.STATUS_SD.Equals("N")
                                       select saleDetails).Select(a => new { a.SALEDATE_SD, a.MRVNO_SD, a.JOBNO_SD, a.PRCODE_SD, a.JOBID_SD });
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = pendingInvoicesList.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
            if (sord.ToUpper() == "DESC")
            {
                pendingInvoicesList = pendingInvoicesList.OrderByDescending(a => a.SALEDATE_SD);
                pendingInvoicesList = pendingInvoicesList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                pendingInvoicesList = pendingInvoicesList.OrderBy(a => a.SALEDATE_SD);
                pendingInvoicesList = pendingInvoicesList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = pendingInvoicesList

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPostedInvoices(string sidx, string sord, int page, int rows, string invNo = null)
        {
            var postedInvoicesList = (from arLedgerDetails in _unitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                                      join arapMaster in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                                      on arLedgerDetails.ARAPCODE_ART equals arapMaster.ARCODE_ARM
                                      where arLedgerDetails.DOCNUMBER_ART.StartsWith("INV") && arLedgerDetails.STATUS_ART != "R"
                                      select new { arLedgerDetails.DOCNUMBER_ART, arapMaster.DESCRIPTION_ARM, arLedgerDetails.GLDATE_ART });
            if (!string.IsNullOrEmpty(invNo))
            {
                postedInvoicesList = postedInvoicesList.Where(o => o.DOCNUMBER_ART.Contains(invNo));
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = postedInvoicesList.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
            if (sord.ToUpper() == "DESC")
            {
                postedInvoicesList = postedInvoicesList.OrderByDescending(a => a.GLDATE_ART);
                postedInvoicesList = postedInvoicesList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                postedInvoicesList = postedInvoicesList.OrderBy(a => a.GLDATE_ART);
                postedInvoicesList = postedInvoicesList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = postedInvoicesList

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getPostedInvDetails(string invNo)
        {
            var ledgerData = (from arapLedger in _unitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                              where arapLedger.DOCNUMBER_ART.Equals(invNo)
                              select arapLedger).FirstOrDefault();
            return Json(ledgerData, JsonRequestBehavior.AllowGet);
        }
    }
}