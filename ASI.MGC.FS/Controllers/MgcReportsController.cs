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
        public ActionResult JobCardDetails()
        {
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
        public ActionResult JournalVoucher(string voucherNo = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(voucherNo))
                {
                    _unitOfWork.Truncate("JVREPORT");
                    var glDetails = (from genralLedger in _unitOfWork.Repository<GLTRANSACTION1>().Query().Get()
                                     where genralLedger.DOCNUMBER_GLT.Equals(voucherNo)
                                     select genralLedger);

                    foreach (var detail in glDetails.ToList())
                    {
                        var objJvReport = _unitOfWork.Repository<JVREPORT>().Create();
                        objJvReport.DOCDATE_VPR = detail.DOCDATE_GLT;
                        objJvReport.GLDATE_VPR = detail.GLDATE_GLT;
                        objJvReport.DOCNO_VPT = detail.DOCNUMBER_GLT;
                        objJvReport.ALCODE_VPT = "GL";
                        objJvReport.ACCODE_VPR = detail.GLACCODE_GLT;
                        objJvReport.ACDESCRIPTION_VPR = GetGlMasterCode(detail.GLACCODE_GLT);
                        objJvReport.DEBITAMOUNT_VPR = detail.DEBITAMOUNT_GLT;
                        objJvReport.CREDITAMOUNT_VPR = detail.CREDITAMOUNT_GLT;
                        objJvReport.NARRATION_VPT = detail.NARRATION_GLT;
                        _unitOfWork.Repository<JVREPORT>().Insert(objJvReport);
                        _unitOfWork.Save();
                    }

                    var arApLedgerDetails = (from ledgerList in _unitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                                             where ledgerList.DOCNUMBER_ART.Equals(voucherNo)
                                             select ledgerList);
                    foreach (var detail in arApLedgerDetails.ToList())
                    {
                        var accountDetails = GetArApMasterDeatils(detail.ARAPCODE_ART);
                        var objJvReport = _unitOfWork.Repository<JVREPORT>().Create();
                        objJvReport.DOCDATE_VPR = detail.DODATE_ART;
                        objJvReport.GLDATE_VPR = detail.GLDATE_ART;
                        objJvReport.DOCNO_VPT = detail.DOCNUMBER_ART;
                        objJvReport.ACCODE_VPR = detail.ARAPCODE_ART;
                        objJvReport.ALCODE_VPT = accountDetails.TYPE_ARM;
                        objJvReport.ACDESCRIPTION_VPR = accountDetails.DESCRIPTION_ARM;
                        objJvReport.DEBITAMOUNT_VPR = detail.DEBITAMOUNT_ART;
                        objJvReport.CREDITAMOUNT_VPR = detail.CREDITAMOUNT_ART;
                        objJvReport.NARRATION_VPT = detail.NARRATION_ART;
                        _unitOfWork.Repository<JVREPORT>().Insert(objJvReport);
                        _unitOfWork.Save();
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
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
    }
}