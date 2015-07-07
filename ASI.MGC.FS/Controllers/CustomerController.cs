using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASI.MGC.FS.Model;
using ASI.MGC.FS.Model.HelperClasses;
using ASI.MGC.FS.Domain;

namespace ASI.MGC.FS.Controllers
{
    public class CustomerController : Controller
    {
        IUnitOfWork _unitOfWork;
        public CustomerController()
        {
            _unitOfWork = new UnitOfWork();
        }
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult getCustList(string sidx, string sord, int page, int rows)
        {
            var CustList = (from prodList in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                            select prodList).Select(a => new { a.ARCODE_ARM, a.DESCRIPTION_ARM });
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = CustList.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            if (sord.ToUpper() == "DESC")
            {
                CustList = CustList.OrderByDescending(a => a.ARCODE_ARM);
                CustList = CustList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                CustList = CustList.OrderBy(a => a.ARCODE_ARM);
                CustList = CustList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = CustList

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CustomerList()
        {
            return View();
        }

        public ActionResult GetAllCustomerDetails()
        {
            return View();
        }

        public JsonResult GetAllCustomers(jQueryDataTableParamModel Param)
        {
            var totalCustRecords = (from totalCustCount in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                                   select totalCustCount);
            IEnumerable<AR_AP_MASTER> filteredCust;
            if (!string.IsNullOrEmpty(Param.sSearch))
            {
                filteredCust = (from totalCustCount in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                                where totalCustCount.ARCODE_ARM.Contains(Param.sSearch) || totalCustCount.DESCRIPTION_ARM.Contains(Param.sSearch)
                                    || totalCustCount.CONDACTPERSON_ARM.Contains(Param.sSearch) || totalCustCount.ADDRESS1_ARM.Contains(Param.sSearch)
                                    || totalCustCount.TELEPHONE_ARM.Contains(Param.sSearch) || totalCustCount.EMAIL_ARM.Contains(Param.sSearch) || totalCustCount.TYPE_ARM.Contains(Param.sSearch)
                                    || totalCustCount.CREDITDAYS_ARM.ToString().Contains(Param.sSearch)
                                select totalCustCount);
            }
            else
            {
                filteredCust = totalCustRecords;
            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<AR_AP_MASTER, string> orderingFunction = (a => sortColumnIndex == 0 ? a.ARCODE_ARM : sortColumnIndex == 1 ? a.DESCRIPTION_ARM : sortColumnIndex == 2 ? a.CONDACTPERSON_ARM
                                                            : sortColumnIndex == 3 ? a.ADDRESS1_ARM : sortColumnIndex == 6 ? a.TYPE_ARM: Convert.ToString(a.CREDITDAYS_ARM));

            var sortDirection = Request["sSortDir_0"];
            if (sortDirection == "asc")
            {
                filteredCust = filteredCust.OrderBy(orderingFunction);
            }
            else
            {
                filteredCust = filteredCust.OrderByDescending(orderingFunction);
            }


            int totalRecords = totalCustRecords.Count();
            int totalDisplayedRecords = filteredCust.Count();
            var dislpayedJobs = filteredCust.Skip(Param.iDisplayStart)
                                            .Take(Param.iDisplayLength);
            var resultCustRecords = from Cust in dislpayedJobs select new { Cust.ARCODE_ARM, Cust.DESCRIPTION_ARM, Cust.CONDACTPERSON_ARM, Cust.ADDRESS1_ARM, Cust.TELEPHONE_ARM
                                                                            ,Cust.EMAIL_ARM, Cust.TYPE_ARM, Cust.CREDITDAYS_ARM};
            return Json(new
            {
                sEcho = Param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalDisplayedRecords,
                aaData = resultCustRecords
            }, JsonRequestBehavior.AllowGet);
        }
    }
}