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