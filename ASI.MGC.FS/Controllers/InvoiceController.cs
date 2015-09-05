using System;
using System.Web.Mvc;
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
            return View("InvoicePreparation");
        }
    }
}