using System;
using System.Linq;
using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;

namespace ASI.MGC.FS.Controllers
{
    public class MgcReportsController : Controller
    {
        readonly IUnitOfWork _unitOfWork;
        public MgcReportsController()
        {
            _unitOfWork = new UnitOfWork();
        }
        public ActionResult ArStatement()
        {
            return View();
        }
        public ActionResult ApStatement()
        {
            return View();
        }
        public ActionResult ApStatementOutstanding()
        {
            return View();
        }
        public ActionResult ArStatementOutstanding()
        {
            return View();
        }
        public ActionResult ArSummary()
        {
            return View();
        }
        public ActionResult BalanceSheet()
        {
            return View();
        }
        public ActionResult BankPayment(string bpNo = "")
        {
            ViewBag.bpNo = bpNo;
            return View();
        }
        public ActionResult BankReceipt(string brNo = "")
        {
            ViewBag.brNo = brNo;
            return View();
        }
        public ActionResult BankStatement()
        {
            return View();
        }
        public ActionResult CancelledJobs()
        {
            return View();
        }
        public ActionResult CashMemo(string cmNo = "")
        {
            ViewBag.cmNo = cmNo;
            return View();
        }
        public ActionResult CashMemoReverse()
        {
            return View();
        }
        public ActionResult CashMemoWiseReport()
        {
            return View();
        }
        public ActionResult CashPayment(string cpNo = "")
        {
            ViewBag.cpNo = cpNo;
            return View();
        }
        public ActionResult CashReceipt(string crNo = "")
        {
            ViewBag.crNo = crNo;
            return View();
        }
        public ActionResult DeliveredMrvDetails()
        {
            return View();
        }
        public ActionResult DeliveryDetails()
        {
            return View();
        }
        public ActionResult DeliveryNote()
        {
            return View();
        }
        public ActionResult DeliveryNoteDetails()
        {
            return View();
        }
        public ActionResult EmpSales()
        {
            return View();
        }
        public ActionResult EmpWiseJobDetails()
        {
            return View();
        }
        public ActionResult GlStatement()
        {
            return View();
        }
        public ActionResult Invoice(string invNo = "")
        {
            ViewBag.invNo = invNo;
            return View();
        }
        public ActionResult InvoiceWiseReport()
        {
            return View();
        }
        public ActionResult JobCardDetails(string jobNo = "")
        {
            ViewBag.jobNo = jobNo;
            return View();
        }
        public ActionResult JobCardFormat(string jobNo = "")
        {
            ViewBag.jobNo = jobNo;
            return View();
        }
        public ActionResult JobWiseStatement()
        {
            return View();
        }
        public ActionResult JournalVoucher(string jvNo = "")
        {
            ViewBag.jvNo = jvNo;
            return View();
        }
        public ActionResult MetarialReceiptVoucher(string mrvNo = "")
        {
            ViewBag.mrvNo = mrvNo;
            return View();
        }
        public ActionResult MrvStatement()
        {
            return View();
        }
        public ActionResult PendingMrvDetails()
        {
            return View();
        }
        public ActionResult ProfitLossStatement()
        {
            return View();
        }
        [HttpPost]
        public JsonResult FetchPLData(FormCollection frm)
        {
            bool success = false;
            try
            {
                if (!string.IsNullOrEmpty(frm["startDate"]) && !string.IsNullOrEmpty(frm["endDate"]))
                {
                    var startDate = Convert.ToDateTime(frm["startDate"]);
                    var endDate = Convert.ToDateTime(frm["endDate"]);
                    if (endDate > startDate)
                    {
                        decimal openingBalance = default(decimal);
                        decimal uptoYear = default(decimal);

                        decimal debitAmount = default(decimal);
                        decimal creditAmount = default(decimal);
                        decimal netAmount = default(decimal);
                        string plAccType = string.Empty;
                        short i = 0;
                        decimal qty = default(decimal);
                        decimal rate = default(decimal);
                        decimal stockAmount = default(decimal);

                        decimal curMonth = default(decimal);
                        decimal curYear = default(decimal);
                        DateTime yearStart = Convert.ToDateTime("01-jan-" + endDate.Year);
                        string plCode = string.Empty;
                        _unitOfWork.Truncate("PROFITANDLOSS_RPT");
                        var glMaster = (from glm in _unitOfWork.Repository<GLMASTER>().Query().Get()
                                        where Convert.ToInt32(glm.GLCODE_LM) >= 3000 & Convert.ToInt32(glm.GLCODE_LM) <= 4999
                                        select new { glm.GLCODE_LM, glm.GLDESCRIPTION_LM }).ToList();
                        foreach (var item in glMaster)
                        {
                            var glCode = Convert.ToInt32(item.GLCODE_LM);
                            if (glCode >= 3000 & glCode < 3500)
                            {
                                plCode = "B";
                                plAccType = "Sales";
                            }
                            if (glCode >= 3500 & glCode < 4000)
                            {
                                plCode = "D";
                                plAccType = "Other Income";
                            }
                            if (glCode >= 4000 & glCode < 4500)
                            {
                                plCode = "A";
                                plAccType = "Cost of Sales";
                            }
                            if (glCode >= 4500 & glCode < 5000)
                            {
                                plCode = "C";
                                plAccType = "Other Expense";
                            }
                            openingBalance = 0;

                            var sumUptoYear = (from glt in _unitOfWork.Repository<GLTRANSACTION1>().Query().Get()
                                               where glt.GLACCODE_GLT.Equals(item.GLCODE_LM) &&
                                               (glt.GLDATE_GLT >= yearStart && glt.GLDATE_GLT <= endDate)
                                               select (glt.DEBITAMOUNT_GLT - glt.CREDITAMOUNT_GLT)).Sum();
                            uptoYear = sumUptoYear != null ? Convert.ToDecimal(sumUptoYear) : 0;
                            var sumUptoSelectedDate = (from glt in _unitOfWork.Repository<GLTRANSACTION1>().Query().Get()
                                                       where glt.GLACCODE_GLT.Equals(item.GLCODE_LM) &&
                                                       (glt.GLDATE_GLT >= startDate && glt.GLDATE_GLT <= endDate)
                                                       select (glt.DEBITAMOUNT_GLT - glt.CREDITAMOUNT_GLT)).Sum();
                            openingBalance = sumUptoYear != null ? Convert.ToDecimal(sumUptoYear) : 0;
                            var rptPLStatementObj = _unitOfWork.Repository<PROFITANDLOSS_RPT>().Create();
                            rptPLStatementObj.ACCOUNTCODE_PL = plAccType;
                            rptPLStatementObj.DESCRIPTION_PL = item.GLDESCRIPTION_LM;
                            rptPLStatementObj.CURMONTH_PL = openingBalance;
                            rptPLStatementObj.CURRENT_YEARTODATE = uptoYear;
                            rptPLStatementObj.GROUPCODE = plCode;
                            _unitOfWork.Repository<PROFITANDLOSS_RPT>().Insert(rptPLStatementObj);
                            _unitOfWork.Save();
                        }
                        openingBalance = fn_StockValution(startDate);
                        uptoYear = fn_StockValution(yearStart);

                        var rptPLStatementOpn = _unitOfWork.Repository<PROFITANDLOSS_RPT>().Create();
                        rptPLStatementOpn.ACCOUNTCODE_PL = plAccType;
                        rptPLStatementOpn.DESCRIPTION_PL = "Opening Stock";
                        rptPLStatementOpn.CURMONTH_PL = openingBalance;
                        rptPLStatementOpn.CURRENT_YEARTODATE = uptoYear;
                        rptPLStatementOpn.GROUPCODE = "A";
                        _unitOfWork.Repository<PROFITANDLOSS_RPT>().Insert(rptPLStatementOpn);
                        _unitOfWork.Save();

                        var rptPLStatementCls = _unitOfWork.Repository<PROFITANDLOSS_RPT>().Create();
                        rptPLStatementCls.ACCOUNTCODE_PL = "Sales";
                        rptPLStatementCls.DESCRIPTION_PL = "Closing Stock";
                        rptPLStatementCls.CURMONTH_PL = openingBalance;
                        rptPLStatementCls.CURRENT_YEARTODATE = uptoYear;
                        rptPLStatementCls.GROUPCODE = "B";
                        _unitOfWork.Repository<PROFITANDLOSS_RPT>().Insert(rptPLStatementCls);
                        _unitOfWork.Save();
                    }
                }

            }
            catch (Exception)
            {

            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PurchaseRegister()
        {
            return View();
        }
        public ActionResult Quotation(string quotNo = "")
        {
            ViewBag.quotNo = quotNo;
            return View();
        }
        public ActionResult ReversedInvoice()
        {
            return View();
        }
        public ActionResult SalesInvoice()
        {
            return View();
        }
        public ActionResult ServiceItems()
        {
            return View();
        }
        public ActionResult StockJournal()
        {
            return View();
        }
        public ActionResult StockLedgerReport()
        {
            return View();
        }
        public ActionResult StockReceipt(string voucherNo = "")
        {
            ViewBag.voucherNo = voucherNo;
            return View();
        }
        public ActionResult StockReport()
        {
            return View();
        }
        public ActionResult TrialBalance()
        {
            return View();
        }

        private string GetGlMasterCode(string glCode)
        {
            var glDetails = (from genralMaster in _unitOfWork.Repository<GLMASTER>().Query().Get()
                             where genralMaster.GLCODE_LM.Equals(glCode)
                             select genralMaster);
            return glDetails.ToList()[0].GLDESCRIPTION_LM;
        }

        private AR_AP_MASTER GetArApMasterDeatils(string arApCode)
        {
            var arApMaster = (from arApMasterList in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                              where arApMasterList.ARCODE_ARM.Equals(arApCode)
                              select arApMasterList).ToList();
            return arApMaster[0];
        }

        [HttpPost]
        public JsonResult GenerateBalanceSheet(FormCollection frm)
        {
            throw new NotImplementedException();
        }

        private decimal fn_StockValution(DateTime date)
        {
            return 0;
        }
    }
}