using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;
using ASI.MGC.FS.WebCommon;
using ASI.MGC.FS.ExtendedAPI;
using ASI.MGC.FS.Models;

namespace ASI.MGC.FS.Controllers
{
    [MESAuthorize]
    public class MRVController : Controller
    {
        IUnitOfWork _unitOfWork;

        public MRVController()
        {
            _unitOfWork = new UnitOfWork();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MRVCreation()
        {
            int mrvCountCode = (1001 + CommonModelAccessUtility.getCurrMRVCount(_unitOfWork));
            string currYear = System.DateTime.Now.Year.ToString();
            string mrvCode = "MRV/" + Convert.ToString(mrvCountCode) + "/" + currYear;
            ViewBag.MRVCode = mrvCode;
            var objMRV = new MATERIALRECEIPTMASTER();
            return View(objMRV);
        }

        public ActionResult SaveMRVCreation(FormCollection form, MATERIALRECEIPTMASTER objMRV)
        {
            try
            {
                string jsonProductDetails = form["mrvProds"];
                string mrvNo = objMRV.MRVNO_MRV.ToString();
                var serializer = new JavaScriptSerializer();
                var lstMrvProducts = serializer.Deserialize<List<MRVREFERENCE>>(jsonProductDetails);
                objMRV.DOC_DATE_MRV = Convert.ToDateTime(System.DateTime.Now.ToShortDateString());
                objMRV.STATUS_MRV = "N";
                _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Insert(objMRV);
                _unitOfWork.Save();
                saveMRVProducts(lstMrvProducts, objMRV);
                return RedirectToAction("MRVCreation");
            }
            catch (Exception e)
            {

            }
            return RedirectToAction("MRVCreation");
        }

        private void saveMRVProducts(List<MRVREFERENCE> lstMrvProducts, MATERIALRECEIPTMASTER objMRV)
        {
            foreach (var prd in lstMrvProducts)
            {
                prd.MRVNO_MRR = objMRV.MRVNO_MRV;
                prd.JOBSTATUS_MRR = "N";
                insertJobData(prd, objMRV);
                _unitOfWork.Repository<MRVREFERENCE>().Insert(prd);
                _unitOfWork.Save();
            }
        }

        private void insertJobData(MRVREFERENCE prd, MATERIALRECEIPTMASTER objMRV)
        {
            int jobCount = (1001 + CommonModelAccessUtility.getJobMasterCount(_unitOfWork));
            string currYear = System.DateTime.Now.Year.ToString();
            string jobCode = "JOB/" + Convert.ToString(jobCount) + "/" + currYear;
            var jobMasterObj = _unitOfWork.Repository<JOBMASTER>().Create();
            jobMasterObj.JOBNO_JM = jobCode;
            jobMasterObj.DOCDATE_JM = objMRV.DOC_DATE_MRV;
            jobMasterObj.MRVNO_JM = objMRV.MRVNO_MRV;
            jobMasterObj.EMPCODE_JM = objMRV.EXECODE_MRV;
            jobMasterObj.JOBCODE_JM = prd.JOBID_MRR;
            jobMasterObj.JOBSTATUS_JM = prd.JOBSTATUS_MRR;
            jobMasterObj.PRODID_JIM = prd.PRODID_MRR;
            jobMasterObj.PRODQTY_JM = prd.QTY_MRR;
            _unitOfWork.Repository<JOBMASTER>().Insert(jobMasterObj);
            _unitOfWork.Save();
        }

        public JsonResult getMRVList(string sidx, string sord, int page, int rows)
        {
            var mrvList = (from mrvMaster in _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                           select mrvMaster).Select(a => new { a.MRVNO_MRV, a.CUSTOMERCODE_MRV, a.CUSTOMERNAME_MRV });
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = mrvList.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            if (sord.ToUpper() == "ASC")
            {
                mrvList = mrvList.OrderBy(a => a.MRVNO_MRV);
                mrvList = mrvList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                mrvList = mrvList.OrderByDescending(a => a.MRVNO_MRV);
                mrvList = mrvList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = mrvList

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getJobDetailByMRV(string MRVID)
        {
            var JobData = (from JobDetails in _unitOfWork.Repository<JOBMASTER>().Query().Get()
                           join prdMaster in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                           on JobDetails.PRODID_JIM equals prdMaster.PROD_CODE_PM
                           where JobDetails.MRVNO_JM.Equals(MRVID)
                           select new { JobNo = JobDetails.JOBNO_JM, PrdCode = prdMaster.DESCRIPTION_PM, JobStatus = JobDetails.JOBSTATUS_JM }).ToList();
            return Json(new
            {
                JobData
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getSaleDetailByMRV(string MRVID)
        {
            IList<CustomSaleDetails> lstSales = new List<CustomSaleDetails>();

            var SaleDetails = (from saleDetails in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                               where saleDetails.MRVNO_SD.Equals(MRVID) && saleDetails.STATUS_SD.Equals("N")
                               select saleDetails).ToList();
            foreach (var sale in SaleDetails)
            {
                CustomSaleDetails objSales = new CustomSaleDetails();
                objSales.JobNo = sale.JOBNO_SD;
                objSales.PRCode = sale.PRCODE_SD;
                objSales.SWCode = sale.JOBID_SD;
                if (!string.IsNullOrEmpty(sale.PRCODE_SD))
                {
                    var objPrd = (from prdDetails in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                                  where prdDetails.PROD_CODE_PM.Equals(sale.PRCODE_SD)
                                  select prdDetails).SingleOrDefault();
                    objSales.Description = objPrd.DESCRIPTION_PM;
                }
                else if (!string.IsNullOrEmpty(sale.JOBID_SD))
                {
                    var objJob = (from JobDetails in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                                  where JobDetails.JOBID_JR.Equals(sale.JOBID_SD)
                                  select JobDetails).SingleOrDefault();
                    objSales.Description = objJob.JOBDESCRIPTION_JR;
                }
                else
                {
                    objSales.Description = "";
                }
                objSales.Qty = Convert.ToInt32(sale.QTY_SD);
                objSales.Unit = sale.UNIT_SD;
                objSales.Rate = Convert.ToInt32(sale.RATE_SD);
                objSales.Discount = Convert.ToDouble(sale.DISCOUNT_SD);
                objSales.ShipChrg = Convert.ToDouble(sale.SHIPPINGCHARGES_SD);
                objSales.CashAmount = (Convert.ToInt32(sale.QTY_SD) * Convert.ToInt32(sale.RATE_SD)) + Convert.ToDouble(sale.DISCOUNT_SD) + Convert.ToDouble(sale.SHIPPINGCHARGES_SD);
                lstSales.Add(objSales);
            }

            return Json(new
            {
                lstSales
            }, JsonRequestBehavior.AllowGet);
        }
    }
}