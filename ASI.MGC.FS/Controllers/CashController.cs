using ASI.MGC.FS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASI.MGC.FS.Domain;

namespace ASI.MGC.FS.Controllers
{
    public class CashController : Controller
    {
        IUnitOfWork _unitOfWork;
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
            return View();
        }

        public ActionResult CashReceipt()
        {
            return View();
        }

        public ActionResult SaveCashMemo()
        {
            return View();
        }

        public JsonResult getBankCodes(string term)
        {
            IList<string> lstBankCodes = (from bankList in _unitOfWork.Repository<BANKMASTER>().Query().Get()
                                         where bankList.BANKCODE_BM.StartsWith(term)
                                         select bankList).Distinct().Select(x => x.BANKCODE_BM).ToList();
            return Json(lstBankCodes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getBankName(string term)
        {
            IList<string> lstBankName = (from bankList in _unitOfWork.Repository<BANKMASTER>().Query().Get()
                                          where bankList.BANKNAME_BM.StartsWith(term)
                                          select bankList).Distinct().Select(x => x.BANKNAME_BM).ToList();
            return Json(lstBankName, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBankDetailsList(string sidx, string sord, int page, int rows, string bankCode, string bankName)
        {
            var bankList = (from banks in _unitOfWork.Repository<BANKMASTER>().Query().Get()
                           select banks).Select(a => new { a.BANKCODE_BM, a.BANKNAME_BM});
            if (!string.IsNullOrEmpty(bankCode))
            {
                bankList = (from jobs in _unitOfWork.Repository<BANKMASTER>().Query().Get()
                           where jobs.BANKCODE_BM.Contains(bankCode)
                            select jobs).Select(a => new { a.BANKCODE_BM, a.BANKNAME_BM });
            }
            if (!string.IsNullOrEmpty(bankName))
            {
                bankList = (from jobs in _unitOfWork.Repository<BANKMASTER>().Query().Get()
                           where jobs.BANKCODE_BM.Contains(bankName)
                            select jobs).Select(a => new { a.BANKCODE_BM, a.BANKNAME_BM });
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = bankList.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            if (sord.ToUpper() == "DESC")
            {
                bankList = bankList.OrderByDescending(a => a.BANKCODE_BM);
                bankList = bankList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                bankList = bankList.OrderBy(a => a.BANKCODE_BM);
                bankList = bankList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = bankList

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}