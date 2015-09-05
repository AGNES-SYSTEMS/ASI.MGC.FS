using System;
using System.Linq;
using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;

namespace ASI.MGC.FS.Controllers
{
    public class BankController : Controller
    {
        readonly IUnitOfWork _unitOfWork;
        public BankController()
        {
            _unitOfWork = new UnitOfWork();
        }
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
        public JsonResult GetBankDetailsList(string sidx, string sord, int page, int rows, string bankCode, string bankName)
        {
            var bankList = (from banks in _unitOfWork.Repository<BANKMASTER>().Query().Get()
                            select banks).Select(a => new { a.BANKCODE_BM, a.BANKNAME_BM });
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
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
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

        public JsonResult GetBankDetailsByType(string sidx, string sord, int page, int rows, string bankType)
        {
            var bankList = (from banks in _unitOfWork.Repository<BANKMASTER>().Query().Get()
                            select banks).Select(a => new { a.BANKCODE_BM, a.BANKNAME_BM });
            if (!string.IsNullOrEmpty(bankType))
            {
                bankList = (from jobs in _unitOfWork.Repository<BANKMASTER>().Query().Get()
                            where jobs.MODE_BM.Equals(bankType)
                            select jobs).Select(a => new { a.BANKCODE_BM, a.BANKNAME_BM });
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = bankList.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
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