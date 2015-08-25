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
                                                            : sortColumnIndex == 3 ? a.ADDRESS1_ARM : sortColumnIndex == 6 ? a.TYPE_ARM : Convert.ToString(a.CREDITDAYS_ARM));

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
            var resultCustRecords = from Cust in dislpayedJobs
                                    select new
                                    {
                                        Cust.ARCODE_ARM,
                                        Cust.DESCRIPTION_ARM,
                                        Cust.CONDACTPERSON_ARM,
                                        Cust.ADDRESS1_ARM,
                                        Cust.TELEPHONE_ARM
                                      ,
                                        Cust.EMAIL_ARM,
                                        Cust.TYPE_ARM,
                                        Cust.CREDITDAYS_ARM
                                    };
            return Json(new
            {
                sEcho = Param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalDisplayedRecords,
                aaData = resultCustRecords
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomersCode(string term)
        {
            IList<string> lstCustCode = (from custList in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                                         where custList.ARCODE_ARM.StartsWith(term) && custList.TYPE_ARM.Equals("AR")
                                         select custList).Distinct().Select(x => x.ARCODE_ARM).ToList();
            return Json(lstCustCode, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomersName(string term)
        {
            IList<string> lstCustName = (from custList in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                                         where custList.DESCRIPTION_ARM.Contains(term) && custList.TYPE_ARM.Equals("AR")
                                         select custList).Distinct().Select(x => x.DESCRIPTION_ARM).ToList();
            return Json(lstCustName, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getCustomerRecord(string custCode, string custName)
        {
            AR_AP_MASTER objCustomer = null;
            if (!string.IsNullOrEmpty(custCode) && !string.IsNullOrWhiteSpace(custCode))
            {
                objCustomer = (from custList in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                               where custList.ARCODE_ARM.Equals(custCode)
                               select custList).FirstOrDefault();
            }
            else if (!string.IsNullOrEmpty(custName) && !string.IsNullOrWhiteSpace(custName))
            {
                objCustomer = (from custList in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                               where custList.DESCRIPTION_ARM.Equals(custName)
                               select custList).FirstOrDefault();
            }
            return Json(objCustomer, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerDetailsList(string sidx, string sord, int page, int rows, string custID, string custName)
        {
            var custList = (from customers in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                           where customers.TYPE_ARM.Equals("AR")
                           select customers).Select(a => new { a.ARCODE_ARM, a.DESCRIPTION_ARM});
            if (!string.IsNullOrEmpty(custID))
            {
                custList = (from customers in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                           where customers.ARCODE_ARM.Contains(custID) && customers.TYPE_ARM.Equals("AR")
                           select customers).Select(a => new { a.ARCODE_ARM, a.DESCRIPTION_ARM });
            }
            if (!string.IsNullOrEmpty(custName))
            {
                custList = (from customers in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                           where customers.DESCRIPTION_ARM.Contains(custName) && customers.TYPE_ARM.Equals("AR")
                           select customers).Select(a => new { a.ARCODE_ARM, a.DESCRIPTION_ARM});
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = custList.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            if (sord.ToUpper() == "DESC")
            {
                custList = custList.OrderByDescending(a => a.ARCODE_ARM);
                custList = custList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                custList = custList.OrderBy(a => a.ARCODE_ARM);
                custList = custList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = custList

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAPCustomerDetailsList(string sidx, string sord, int page, int rows, string custID, string custName)
        {
            var custList = (from customers in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                            where customers.TYPE_ARM.Equals("AP")
                            select customers).Select(a => new { a.ARCODE_ARM, a.DESCRIPTION_ARM });
            if (!string.IsNullOrEmpty(custID))
            {
                custList = (from customers in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                            where customers.ARCODE_ARM.Contains(custID) && customers.TYPE_ARM.Equals("AP")
                            select customers).Select(a => new { a.ARCODE_ARM, a.DESCRIPTION_ARM });
            }
            if (!string.IsNullOrEmpty(custName))
            {
                custList = (from customers in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                            where customers.DESCRIPTION_ARM.Contains(custName) && customers.TYPE_ARM.Equals("AP")
                            select customers).Select(a => new { a.ARCODE_ARM, a.DESCRIPTION_ARM });
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = custList.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            if (sord.ToUpper() == "DESC")
            {
                custList = custList.OrderByDescending(a => a.ARCODE_ARM);
                custList = custList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                custList = custList.OrderBy(a => a.ARCODE_ARM);
                custList = custList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = custList

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}