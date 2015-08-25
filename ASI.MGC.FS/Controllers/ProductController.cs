using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;
using ASI.MGC.FS.Model.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASI.MGC.FS.Controllers
{
    public class ProductController : Controller
    {
        IUnitOfWork _unitOfWork;

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

        public JsonResult getProdsList(string sidx, string sord, int page, int rows)
        {
            var ProdList = (from prodList in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                           select prodList).Select(a => new { a.PROD_CODE_PM, a.DESCRIPTION_PM });
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = ProdList.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            if (sord.ToUpper() == "DESC")
            {
                ProdList = ProdList.OrderByDescending(a => a.PROD_CODE_PM);
                ProdList = ProdList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                ProdList = ProdList.OrderBy(a => a.PROD_CODE_PM);
                ProdList = ProdList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = ProdList

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllProductDetails()
        {
            return View();
        }

        public JsonResult GetAllProducts(jQueryDataTableParamModel Param)
        {
            var totalProductRecords = (from totalPRDCount in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                                   select totalPRDCount);
            IEnumerable<PRODUCTMASTER> filteredProducts;
            if (!string.IsNullOrEmpty(Param.sSearch))
            {
                filteredProducts = (from totalPRDCount in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                                where totalPRDCount.PROD_CODE_PM.Contains(Param.sSearch) || totalPRDCount.DESCRIPTION_PM.Contains(Param.sSearch) 
                                || totalPRDCount.CUR_QTY_PM.ToString().Contains(Param.sSearch) || totalPRDCount.RATE_PM.ToString().Contains(Param.sSearch)
                                || totalPRDCount.SELLINGPRICE_RM.ToString().Contains(Param.sSearch) || totalPRDCount.STATUS_PM.Contains(Param.sSearch)
                                select totalPRDCount);
            }
            else
            {
                filteredProducts = totalProductRecords;
            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<PRODUCTMASTER, string> orderingFunction = (a => sortColumnIndex == 0 ? a.PROD_CODE_PM : sortColumnIndex == 1 ? a.DESCRIPTION_PM : sortColumnIndex == 2 ? Convert.ToString(a.CUR_QTY_PM)
                                                            : sortColumnIndex == 3 ? Convert.ToString(a.RATE_PM) : sortColumnIndex == 4 ? Convert.ToString(a.SELLINGPRICE_RM) : sortColumnIndex == 5
                                                            ? a.PURCHSEUNIT_PM : sortColumnIndex == 6 ? a.SALESUNIT_PM : a.STATUS_PM);

            var sortDirection = Request["sSortDir_0"];
            if (sortDirection == "asc")
            {
                filteredProducts = filteredProducts.OrderBy(orderingFunction);
            }
            else
            {
                filteredProducts = filteredProducts.OrderByDescending(orderingFunction);
            }


            int totalRecords = totalProductRecords.Count();
            int totalDisplayedRecords = filteredProducts.Count();
            var dislpayedPRDs = filteredProducts.Skip(Param.iDisplayStart)
                                            .Take(Param.iDisplayLength);
            var resultJobRecords = from PRD in dislpayedPRDs select new { PRD.PROD_CODE_PM, PRD.DESCRIPTION_PM, PRD.CUR_QTY_PM, PRD.RATE_PM, PRD.SELLINGPRICE_RM, PRD.PURCHSEUNIT_PM, PRD.SALESUNIT_PM, PRD.STATUS_PM};
            return Json(new
            {
                sEcho = Param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalDisplayedRecords,
                aaData = resultJobRecords
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getPrdRecordByID(string prdCode)
        {
            PRODUCTMASTER objProduct = null;
            if (!string.IsNullOrEmpty(prdCode) && !string.IsNullOrWhiteSpace(prdCode))
            {
               objProduct = (from lstProducts in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                              where lstProducts.PROD_CODE_PM.Equals(prdCode)
                              select lstProducts).FirstOrDefault();
            }
            return Json(new { productID = objProduct.PROD_CODE_PM, productDesc = objProduct.DESCRIPTION_PM, productRate = objProduct.RATE_PM }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getPrdRecordByName(string prdName)
        {
            PRODUCTMASTER objProduct = null;
            if (!string.IsNullOrEmpty(prdName) && !string.IsNullOrWhiteSpace(prdName))
            {
                objProduct = (from lstProducts in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                              where lstProducts.DESCRIPTION_PM.Equals(prdName)
                              select lstProducts).FirstOrDefault();
            }
            return Json(new { productID = objProduct.PROD_CODE_PM, productDesc = objProduct.DESCRIPTION_PM, productRate = objProduct.RATE_PM }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getProductCodes()
        {
            IList<string> lstPrDCode = (from productList in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                                        where productList.STATUS_PM.Equals("SP")
                                        select productList).Distinct().Select(x => x.PROD_CODE_PM).ToList();
            return Json(lstPrDCode, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getProductDetails()
        {
            IList<string> lstPrDetail = (from productList in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                                         where productList.STATUS_PM.Equals("SP")
                                         select productList).Distinct().Select(x => x.DESCRIPTION_PM).ToList();
            return Json(lstPrDetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getProductCodeByIP(string term)
        {
            IList<string> lstPrDCode = (from productList in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                                        where productList.PROD_CODE_PM.StartsWith(term) && productList.STATUS_PM.Equals("IP")
                                        select productList).Distinct().Select(x => x.PROD_CODE_PM).ToList();
            return Json(lstPrDCode, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getProductDetailByIP(string term)
        {
            IList<string> lstPrDetail = (from productList in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                                         where productList.DESCRIPTION_PM.StartsWith(term) && productList.STATUS_PM.Equals("IP")
                                         select productList).Distinct().Select(x => x.DESCRIPTION_PM).ToList();
            return Json(lstPrDetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getUnitlist(string term)
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
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
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

        public JsonResult GetIPProductDetailsList(string sidx, string sord, int page, int rows, string prdCode, string prdName)
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
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
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