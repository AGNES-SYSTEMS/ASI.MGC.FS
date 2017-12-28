using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;
using ASI.MGC.FS.Models;
using ASI.MGC.FS.Domain.Repositories;

namespace ASI.MGC.FS.Controllers
{
    public class CustomerController : Controller
    {
        readonly IUnitOfWork _unitOfWork;
        readonly TimeZoneInfo timeZoneInfo;
        public CustomerController()
        {
            _unitOfWork = new UnitOfWork();
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time");
        }
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetAllCustomers(string sidx, string sord, int page, int rows, string custType, string custId = null, string custName = null)
        {
            var custList = (from customers in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                            where customers.TYPE_ARM.Equals(custType)
                            select customers).Select(a => new
                            {
                                a.ARCODE_ARM,
                                a.DESCRIPTION_ARM,
                                a.ADDRESS1_ARM,
                                a.POBOX_ARM,
                                a.TELEPHONE_ARM,
                                a.FAX_ARM,
                                a.EMAIL_ARM,
                                a.CONDACTPERSON_ARM

                            });

            if (!string.IsNullOrEmpty(custId))
            {
                custList = custList.Where(o => o.ARCODE_ARM.Contains(custId)).Select(a => new
                                {
                                    a.ARCODE_ARM,
                                    a.DESCRIPTION_ARM,
                                    a.ADDRESS1_ARM,
                                    a.POBOX_ARM,
                                    a.TELEPHONE_ARM,
                                    a.FAX_ARM,
                                    a.EMAIL_ARM,
                                    a.CONDACTPERSON_ARM

                                });
            }
            if (!string.IsNullOrEmpty(custName))
            {
                custList = custList.Where(o => o.DESCRIPTION_ARM.Contains(custName)).Select(a => new
                {
                    a.ARCODE_ARM,
                    a.DESCRIPTION_ARM,
                    a.ADDRESS1_ARM,
                    a.POBOX_ARM,
                    a.TELEPHONE_ARM,
                    a.FAX_ARM,
                    a.EMAIL_ARM,
                    a.CONDACTPERSON_ARM

                });
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = custList.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
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

        public JsonResult GetCustList(string sidx, string sord, int page, int rows, string custType)
        {
            var custList = (from customers in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                            select customers).Select(a => new { a.ARCODE_ARM, a.DESCRIPTION_ARM, a.TYPE_ARM });
            if (!string.IsNullOrEmpty(custType))
            {
                custList = custList.Where(o => o.TYPE_ARM.Equals(custType)).Select(a => new { a.ARCODE_ARM, a.DESCRIPTION_ARM, a.TYPE_ARM });
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = custList.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
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

        public ActionResult CustomerList()
        {
            return View();
        }

        public ActionResult GetAllCustomerDetails()
        {
            return View();
        }

        //public JsonResult GetAllCustomers(jQueryDataTableParamModel param)
        //{
        //    var totalCustRecords = (from totalCustCount in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
        //                            select totalCustCount);
        //    IEnumerable<AR_AP_MASTER> filteredCust;
        //    if (!string.IsNullOrEmpty(param.sSearch))
        //    {
        //        filteredCust = (from totalCustCount in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
        //                        where totalCustCount.ARCODE_ARM.Contains(param.sSearch) || totalCustCount.DESCRIPTION_ARM.Contains(param.sSearch)
        //                            || totalCustCount.CONDACTPERSON_ARM.Contains(param.sSearch) || totalCustCount.ADDRESS1_ARM.Contains(param.sSearch)
        //                            || totalCustCount.TELEPHONE_ARM.Contains(param.sSearch) || totalCustCount.EMAIL_ARM.Contains(param.sSearch) || totalCustCount.TYPE_ARM.Contains(param.sSearch)
        //                            || totalCustCount.CREDITDAYS_ARM.ToString().Contains(param.sSearch)
        //                        select totalCustCount);
        //    }
        //    else
        //    {
        //        filteredCust = totalCustRecords;
        //    }

        //    var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

        //    Func<AR_AP_MASTER, string> orderingFunction = (a => sortColumnIndex == 0 ? a.ARCODE_ARM : sortColumnIndex == 1 ? a.DESCRIPTION_ARM : sortColumnIndex == 2 ? a.CONDACTPERSON_ARM
        //                                                    : sortColumnIndex == 3 ? a.ADDRESS1_ARM : sortColumnIndex == 6 ? a.TYPE_ARM : Convert.ToString(a.CREDITDAYS_ARM));

        //    var sortDirection = Request["sSortDir_0"];
        //    filteredCust = sortDirection == "asc" ? filteredCust.OrderBy(orderingFunction) : filteredCust.OrderByDescending(orderingFunction);


        //    int totalRecords = totalCustRecords.Count();
        //    int totalDisplayedRecords = filteredCust.Count();
        //    var dislpayedJobs = filteredCust.Skip(param.iDisplayStart)
        //                                    .Take(param.iDisplayLength);
        //    var resultCustRecords = from cust in dislpayedJobs
        //                            select new
        //                            {
        //                                cust.ARCODE_ARM,
        //                                cust.DESCRIPTION_ARM,
        //                                cust.CONDACTPERSON_ARM,
        //                                cust.ADDRESS1_ARM,
        //                                cust.TELEPHONE_ARM
        //                              ,
        //                                cust.EMAIL_ARM,
        //                                cust.TYPE_ARM,
        //                                cust.CREDITDAYS_ARM
        //                            };
        //    return Json(new
        //    {
        //        param.sEcho,
        //        iTotalRecords = totalRecords,
        //        iTotalDisplayRecords = totalDisplayedRecords,
        //        aaData = resultCustRecords
        //    }, JsonRequestBehavior.AllowGet);
        //}

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

        public JsonResult GetCustomerRecord(string custCode, string custName = null)
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

        public JsonResult GetCustomerDetailsList(string sidx, string sord, int page, int rows, string custId, string custName)
        {
            var custList = (from customers in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                            where customers.TYPE_ARM.Equals("AR")
                            select customers).Select(a => new { a.ARCODE_ARM, a.DESCRIPTION_ARM });
            if (!string.IsNullOrEmpty(custId))
            {
                custList = custList.Where(a => a.ARCODE_ARM.Contains(custId)).Select(a => new { a.ARCODE_ARM, a.DESCRIPTION_ARM });
            }
            if (!string.IsNullOrEmpty(custName))
            {
                custList = custList.Where(a => a.DESCRIPTION_ARM.Contains(custName)).Select(a => new { a.ARCODE_ARM, a.DESCRIPTION_ARM });
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = custList.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
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
        public JsonResult GetApCustomerDetailsList(string sidx, string sord, int page, int rows, string custId, string custName)
        {
            var custList = (from customers in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                            where customers.TYPE_ARM.Equals("AP")
                            select customers).Select(a => new { a.ARCODE_ARM, a.DESCRIPTION_ARM });
            if (!string.IsNullOrEmpty(custId))
            {
                custList = custList.Where(o => o.ARCODE_ARM.Contains(custId)).Select(a => new { a.ARCODE_ARM, a.DESCRIPTION_ARM });
            }
            if (!string.IsNullOrEmpty(custName))
            {
                custList = custList.Where(o => o.DESCRIPTION_ARM.Contains(custName)).Select(a => new { a.ARCODE_ARM, a.DESCRIPTION_ARM });
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = custList.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
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
        [HttpPost]
        public JsonResult SearchDetailsByCustomer(int type = 0, string searchParam = null)
        {
            var repo = _unitOfWork.ExtRepositoryFor<ReportRepository>();
            var searchResult = repo.sp_FindMrvDetails(searchParam, type);
            //List<MRVSearchDetailsResult> searchResult = fn_SearchJobDetails(searchParam, type);
            var jsonData = new
            {
                searchResult = searchResult
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        private List<MRVSearchDetailsResult> fn_SearchJobDetails(string searchParam, int type = 0)
        {
            List<string> MrvDetails = new List<string>();
            if (type == 1)
            {
                if (!string.IsNullOrEmpty(searchParam))
                {
                    MrvDetails = (from mrvData in _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                                  where mrvData.CUSTOMERCODE_MRV.Contains(searchParam)
                                  select mrvData.MRVNO_MRV).ToList();
                }
            }
            if (type == 2)
            {
                if (!string.IsNullOrEmpty(searchParam))
                {
                    MrvDetails = (from mrvData in _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                                  where mrvData.CUSTOMERNAME_MRV.Contains(searchParam)
                                  select mrvData.MRVNO_MRV).ToList();
                }
            }
            if (type == 3)
            {
                if (!string.IsNullOrEmpty(searchParam))
                {
                    MrvDetails = (from mrvData in _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                                  where mrvData.PHONE_MRV.Equals(searchParam)
                                  select mrvData.MRVNO_MRV).ToList();
                }
            }
            List<MRVSearchDetailsResult> saleSearchResult = new List<MRVSearchDetailsResult>();
            foreach (var mrv in MrvDetails)
            {
                var sales = (from saleData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                             where saleData.MRVNO_SD.Equals(mrv)
                             select saleData).ToList();
                foreach (var sale in sales)
                {
                    MRVSearchDetailsResult saleData = new MRVSearchDetailsResult();
                    saleData.JOBNO_SD = sale.JOBNO_SD;
                    saleData.PRCODE_SD = sale.PRCODE_SD;
                    saleData.JOBID_SD = sale.JOBID_SD;
                    saleData.QTY_SD = Convert.ToInt32(sale.QTY_SD);
                    saleData.RATE_SD = Convert.ToDecimal(sale.RATE_SD);
                    saleData.Amount = Convert.ToInt32(sale.QTY_SD) * Convert.ToDecimal(sale.RATE_SD);
                    saleData.DISCOUNT_SD = Convert.ToDecimal(sale.DISCOUNT_SD);
                    saleData.SHIPPINGCHARGES_SD = Convert.ToDecimal(sale.SHIPPINGCHARGES_SD);
                    saleData.SALEDATE_SD = Convert.ToDateTime(sale.SALEDATE_SD);
                    saleData.USERID_SD = sale.USERID_SD;
                    saleData.CASHTOTAL_SD = Convert.ToDecimal(sale.CASHTOTAL_SD);
                    saleData.CREDITTOTAL_SD = Convert.ToDecimal(sale.CREDITTOTAL_SD);
                    saleData.CASHRVNO_SD = sale.CASHRVNO_SD;
                    saleData.INVNO_SD = sale.INVNO_SD;
                    saleData.CREDITACCODE_SD = sale.CREDITACCODE_SD;
                    saleData.LPONO_SD = sale.LPONO_SD;
                    saleData.DAYENDDOC_NO = sale.DAYENDDOC_NO;
                    saleSearchResult.Add(saleData);
                }
            }
            return saleSearchResult;
        }
    }
}