using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASI.MGC.FS.Controllers
{
    public class CashController : Controller
    {
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
            return View();
        }
    }
}