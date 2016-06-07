using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;

namespace ASI.MGC.FS.Controllers
{
    public class ProductController : Controller
    {
        readonly IUnitOfWork _unitOfWork;

        public ProductController()
        {
            _unitOfWork = new UnitOfWork();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProductList()
        {
            return View();
        }

        public JsonResult GetProdsList(string sidx, string sord, int page, int rows, string status, string searchValue)
        {
            var lstProducts = (from prodList in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                               select prodList).Select(a => new
                               {
                                   a.PROD_CODE_PM,
                                   a.DESCRIPTION_PM,
                                   a.CUR_QTY_PM,
                                   a.RATE_PM,
                                   a.SELLINGPRICE_RM,
                                   a.PURCHSEUNIT_PM,
                                   a.SALESUNIT_PM,
                                   a.UNIT_PR,
                                   a.STATUS_PM
                               });
            if (!string.IsNullOrEmpty(status))
            {
                lstProducts = (from prodList in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                               where prodList.STATUS_PM.Equals(status)
                               select prodList).Select(a => new
                               {
                                   a.PROD_CODE_PM,
                                   a.DESCRIPTION_PM,
                                   a.CUR_QTY_PM,
                                   a.RATE_PM,
                                   a.SELLINGPRICE_RM,
                                   a.PURCHSEUNIT_PM,
                                   a.SALESUNIT_PM,
                                   a.UNIT_PR,
                                   a.STATUS_PM
                               });
            }
            if (!string.IsNullOrEmpty(searchValue))
            {
                lstProducts = (from prodList in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                               where prodList.STATUS_PM.Equals(status) && prodList.DESCRIPTION_PM.Contains(searchValue)
                               select prodList).Select(a => new
                               {
                                   a.PROD_CODE_PM,
                                   a.DESCRIPTION_PM,
                                   a.CUR_QTY_PM,
                                   a.RATE_PM,
                                   a.SELLINGPRICE_RM,
                                   a.PURCHSEUNIT_PM,
                                   a.SALESUNIT_PM,
                                   a.UNIT_PR,
                                   a.STATUS_PM
                               });
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = lstProducts.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
            if (sord.ToUpper() == "DESC")
            {
                lstProducts = lstProducts.OrderByDescending(a => a.PROD_CODE_PM);
                lstProducts = lstProducts.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                lstProducts = lstProducts.OrderBy(a => a.PROD_CODE_PM);
                lstProducts = lstProducts.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = lstProducts

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllUnitsList(string sidx, string sord, int page, int rows, string searchValue)
        {
            var lstUnits = (from units in _unitOfWork.Repository<UNITMESSUREMENT>().Query().Get()
                            select units).Select(a => new { a.UNIT_UM, a.DESCRIPTION_UM, a.TYPE_UM, a.UNITQTY_UM, a.BASICPRIMARYUNIT_UM, a.BASICPRIMARYQTY_UM });
            if (!string.IsNullOrEmpty(searchValue))
            {
                lstUnits = (from units in _unitOfWork.Repository<UNITMESSUREMENT>().Query().Get()
                            where units.DESCRIPTION_UM.Contains(searchValue)
                            select units).Select(a => new { a.UNIT_UM, a.DESCRIPTION_UM, a.TYPE_UM, a.UNITQTY_UM, a.BASICPRIMARYUNIT_UM, a.BASICPRIMARYQTY_UM });
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = lstUnits.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
            if (sord.ToUpper() == "DESC")
            {
                lstUnits = lstUnits.OrderByDescending(a => a.UNIT_UM);
                lstUnits = lstUnits.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                lstUnits = lstUnits.OrderBy(a => a.UNIT_UM);
                lstUnits = lstUnits.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = lstUnits

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllProductDetails()
        {
            return View();
        }

        //public JsonResult GetAllProducts(jQueryDataTableParamModel param)
        //{
        //    var totalProductRecords = (from totalPrdCount in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
        //                           select totalPrdCount);
        //    IEnumerable<PRODUCTMASTER> filteredProducts;
        //    if (!string.IsNullOrEmpty(param.sSearch))
        //    {
        //        filteredProducts = (from totalPrdCount in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
        //                        where totalPrdCount.PROD_CODE_PM.Contains(param.sSearch) || totalPrdCount.DESCRIPTION_PM.Contains(param.sSearch) 
        //                        || totalPrdCount.CUR_QTY_PM.ToString().Contains(param.sSearch) || totalPrdCount.RATE_PM.ToString().Contains(param.sSearch)
        //                        || totalPrdCount.SELLINGPRICE_RM.ToString().Contains(param.sSearch) || totalPrdCount.STATUS_PM.Contains(param.sSearch)
        //                        select totalPrdCount);
        //    }
        //    else
        //    {
        //        filteredProducts = totalProductRecords;
        //    }

        //    var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

        //    Func<PRODUCTMASTER, string> orderingFunction = (a => sortColumnIndex == 0 ? a.PROD_CODE_PM : sortColumnIndex == 1 ? a.DESCRIPTION_PM : sortColumnIndex == 2 ? Convert.ToString(a.CUR_QTY_PM)
        //                                                    : sortColumnIndex == 3 ? Convert.ToString(a.RATE_PM) : sortColumnIndex == 4 ? Convert.ToString(a.SELLINGPRICE_RM) : sortColumnIndex == 5
        //                                                    ? a.PURCHSEUNIT_PM : sortColumnIndex == 6 ? a.SALESUNIT_PM : a.STATUS_PM);

        //    var sortDirection = Request["sSortDir_0"];
        //    filteredProducts = sortDirection == "asc" ? filteredProducts.OrderBy(orderingFunction) : filteredProducts.OrderByDescending(orderingFunction);
        //    int totalRecords = totalProductRecords.Count();
        //    int totalDisplayedRecords = filteredProducts.Count();
        //    var dislpayedPrDs = filteredProducts.Skip(param.iDisplayStart)
        //                                    .Take(param.iDisplayLength);
        //    var resultJobRecords = from prd in dislpayedPrDs select new { prd.PROD_CODE_PM, prd.DESCRIPTION_PM, prd.CUR_QTY_PM, prd.RATE_PM, prd.SELLINGPRICE_RM, prd.PURCHSEUNIT_PM, prd.SALESUNIT_PM, prd.STATUS_PM};
        //    return Json(new
        //    {
        //        param.sEcho,
        //        iTotalRecords = totalRecords,
        //        iTotalDisplayRecords = totalDisplayedRecords,
        //        aaData = resultJobRecords
        //    }, JsonRequestBehavior.AllowGet);
        //}

        public void GetPrdRecordById(string prdCode)
        {
            PRODUCTMASTER objProduct = null;
            if (!string.IsNullOrEmpty(prdCode) && !string.IsNullOrWhiteSpace(prdCode))
            {
                objProduct = (from lstProducts in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                              where lstProducts.PROD_CODE_PM.Equals(prdCode)
                              select lstProducts).FirstOrDefault();
            }
            if (objProduct != null)
            {
                Json(new { productID = objProduct.PROD_CODE_PM, productDesc = objProduct.DESCRIPTION_PM, productRate = objProduct.RATE_PM }, JsonRequestBehavior.AllowGet);
            }
        }

        public void GetPrdRecordByName(string prdName)
        {
            PRODUCTMASTER objProduct = null;
            if (!string.IsNullOrEmpty(prdName) && !string.IsNullOrWhiteSpace(prdName))
            {
                objProduct = (from lstProducts in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                              where lstProducts.DESCRIPTION_PM.Equals(prdName)
                              select lstProducts).FirstOrDefault();
            }
            if (objProduct != null)
            {
                Json(new { productID = objProduct.PROD_CODE_PM, productDesc = objProduct.DESCRIPTION_PM, productRate = objProduct.RATE_PM }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetProductCodes()
        {
            IList<string> lstPrDCode = (from productList in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                                        where productList.STATUS_PM.Equals("SP")
                                        select productList).Distinct().Select(x => x.PROD_CODE_PM).ToList();
            return Json(lstPrDCode, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductDetails()
        {
            IList<string> lstPrDetail = (from productList in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                                         where productList.STATUS_PM.Equals("SP")
                                         select productList).Distinct().Select(x => x.DESCRIPTION_PM).ToList();
            return Json(lstPrDetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductCodeByIp(string term)
        {
            IList<string> lstPrDCode = (from productList in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                                        where productList.PROD_CODE_PM.StartsWith(term) && productList.STATUS_PM.Equals("IP")
                                        select productList).Distinct().Select(x => x.PROD_CODE_PM).ToList();
            return Json(lstPrDCode, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductDetailByIp(string term)
        {
            IList<string> lstPrDetail = (from productList in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                                         where productList.DESCRIPTION_PM.StartsWith(term) && productList.STATUS_PM.Equals("IP")
                                         select productList).Distinct().Select(x => x.DESCRIPTION_PM).ToList();
            return Json(lstPrDetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUnitlist(string term)
        {
            IList<string> lstUnits = (from unitList in _unitOfWork.Repository<UNITMESSUREMENT>().Query().Get()
                                      where unitList.UNIT_UM.StartsWith(term)
                                      select unitList).Distinct().Select(x => x.UNIT_UM).ToList();
            return Json(lstUnits, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductDetailsList(string sidx, string sord, int page, int rows, string prdCode, string prdName)
        {
            var prdList = (from products in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                           select products).Select(a => new { a.PROD_CODE_PM, a.DESCRIPTION_PM });
            if (!string.IsNullOrEmpty(prdCode))
            {
                prdList = (from products in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                           where products.PROD_CODE_PM.Contains(prdCode)
                           select products).Select(a => new { a.PROD_CODE_PM, a.DESCRIPTION_PM });
            }
            else if (!string.IsNullOrEmpty(prdName))
            {
                prdList = (from products in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                           where products.DESCRIPTION_PM.Contains(prdName)
                           select products).Select(a => new { a.PROD_CODE_PM, a.DESCRIPTION_PM });
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = prdList.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
            if (sord.ToUpper() == "DESC")
            {
                prdList = prdList.OrderByDescending(a => a.PROD_CODE_PM);
                prdList = prdList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                prdList = prdList.OrderBy(a => a.PROD_CODE_PM);
                prdList = prdList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = prdList

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetIpProductDetailsList(string sidx, string sord, int page, int rows, string prdCode, string prdName)
        {
            var prdList = (from products in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                           where products.STATUS_PM.Equals("IP")
                           select products).Select(a => new { a.PROD_CODE_PM, a.DESCRIPTION_PM, a.PURCHSEUNIT_PM, a.SALESUNIT_PM });
            if (!string.IsNullOrEmpty(prdCode))
            {
                prdList = (from products in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                           where products.PROD_CODE_PM.Contains(prdCode) && products.STATUS_PM.Equals("IP")
                           select products).Select(a => new { a.PROD_CODE_PM, a.DESCRIPTION_PM, a.PURCHSEUNIT_PM, a.SALESUNIT_PM });
            }
            else if (!string.IsNullOrEmpty(prdName))
            {
                prdList = (from products in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                           where products.DESCRIPTION_PM.Contains(prdName) && products.STATUS_PM.Equals("IP")
                           select products).Select(a => new { a.PROD_CODE_PM, a.DESCRIPTION_PM, a.PURCHSEUNIT_PM, a.SALESUNIT_PM });
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = prdList.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
            if (sord.ToUpper() == "DESC")
            {
                prdList = prdList.OrderByDescending(a => a.PROD_CODE_PM);
                prdList = prdList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                prdList = prdList.OrderBy(a => a.PROD_CODE_PM);
                prdList = prdList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = prdList

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}