﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASI.MGC.FS.Controllers
{
    public class BankController : Controller
    {
        // GET: Bank
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BankReceipt()
        {
            return View();
        }

        public ActionResult BankPayment()
        {
            return View();
        }
    }
}