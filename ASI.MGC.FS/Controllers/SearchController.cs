using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;
using ASI.MGC.FS.WebCommon;

namespace ASI.MGC.FS.Controllers
{
    public class SearchController : Controller
    {
        readonly IUnitOfWork _unitOfWork;

        public SearchController()
        {
            _unitOfWork = new UnitOfWork();
        }
        // GET: Search
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search()
        {
            ViewBag.SearchTypeList = CommonModelAccessUtility.GetSearchTypeList();
            return View();
        }

        public JsonResult GetSearchDetails(string custCode, string custName, string telephone, int searchType)
        {
            IQueryable customerDetails = null;
            switch (searchType)
            {
                case 0:
                    if (!string.IsNullOrEmpty(custCode))
                    {
                        customerDetails = (from customers in _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                                           where customers.CUSTOMERCODE_MRV != ("CASH") && customers.CUSTOMERCODE_MRV.Contains(custCode)
                                           select customers).Select(a => new { a.MRVNO_MRV, a.CUSTOMERCODE_MRV, a.CUSTOMERNAME_MRV, a.PHONE_MRV });
                    }
                    else if (!string.IsNullOrEmpty(custName))
                    {
                        customerDetails = (from customers in _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                                           where customers.CUSTOMERCODE_MRV != ("CASH") && customers.CUSTOMERNAME_MRV.Contains(custName)
                                           select customers).Select(a => new { a.MRVNO_MRV, a.CUSTOMERCODE_MRV, a.CUSTOMERNAME_MRV, a.PHONE_MRV });
                    }
                    else if (!string.IsNullOrEmpty(telephone))
                    {
                        customerDetails = (from customers in _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                                           where customers.CUSTOMERCODE_MRV != ("CASH") && customers.PHONE_MRV.Contains(telephone)
                                           select customers).Select(a => new { a.MRVNO_MRV, a.CUSTOMERCODE_MRV, a.CUSTOMERNAME_MRV, a.PHONE_MRV });
                    }
                    else
                    {
                        customerDetails = (from customers in _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                                           where customers.CUSTOMERCODE_MRV != ("CASH")
                                           select customers).Select(a => new { a.MRVNO_MRV, a.CUSTOMERCODE_MRV, a.CUSTOMERNAME_MRV, a.PHONE_MRV });
                    }
                    break;
            case 1:
                    if (!string.IsNullOrEmpty(custCode))
                    {
                        customerDetails = (from customers in _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                                           where customers.CUSTOMERCODE_MRV.Equals("CASH") && customers.CUSTOMERCODE_MRV.Contains(custCode)
                                           select customers).Select(a => new { a.MRVNO_MRV, a.CUSTOMERCODE_MRV, a.CUSTOMERNAME_MRV, a.PHONE_MRV });
                    }
                    else if (!string.IsNullOrEmpty(custName))
                    {
                        customerDetails = (from customers in _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                                           where customers.CUSTOMERCODE_MRV.Equals("CASH") && customers.CUSTOMERNAME_MRV.Contains(custName)
                                           select customers).Select(a => new { a.MRVNO_MRV, a.CUSTOMERCODE_MRV, a.CUSTOMERNAME_MRV, a.PHONE_MRV });
                    }
                    else if (!string.IsNullOrEmpty(telephone))
                    {
                        customerDetails = (from customers in
                            _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                                           where customers.CUSTOMERCODE_MRV.Equals("CASH") && customers.PHONE_MRV.Contains(telephone)
                            select customers).Select(
                                a => new {a.MRVNO_MRV, a.CUSTOMERCODE_MRV, a.CUSTOMERNAME_MRV, a.PHONE_MRV});
                    }
                    else
                    {
                        customerDetails = (from customers in _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                                           where customers.CUSTOMERCODE_MRV.Equals("CASH")
                                           select customers).Select(a => new { a.MRVNO_MRV, a.CUSTOMERCODE_MRV, a.CUSTOMERNAME_MRV, a.PHONE_MRV });
                    }
                    break;
            }
            return Json(customerDetails.ToListAsync(), JsonRequestBehavior.AllowGet);
        }
    }
}