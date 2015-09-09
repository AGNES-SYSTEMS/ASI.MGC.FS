using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;
using ASI.MGC.FS.Models;
using ASI.MGC.FS.WebCommon;

namespace ASI.MGC.FS.Controllers
{
    public class MrvController : Controller
    {
        readonly IUnitOfWork _unitOfWork;

        public MrvController()
        {
            _unitOfWork = new UnitOfWork();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MrvCreation()
        {
            int mrvCountCode = (1001 + CommonModelAccessUtility.GetCurrMrvCount(_unitOfWork));
            string currYear = DateTime.Now.Year.ToString();
            string mrvCode = "MRV/" + Convert.ToString(mrvCountCode) + "/" + currYear;
            ViewBag.MRVCode = mrvCode;
            var objMrv = new MATERIALRECEIPTMASTER();
            return View(objMrv);
        }

        [HttpPost]
        public ActionResult SaveMrvCreation(FormCollection form, MATERIALRECEIPTMASTER objMrv)
        {
            try
            {
                string jsonProductDetails = form["mrvProds"];
                var serializer = new JavaScriptSerializer();
                var lstMrvProducts = serializer.Deserialize<List<MRVREFERENCE>>(jsonProductDetails);
                objMrv.DOC_DATE_MRV = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                objMrv.STATUS_MRV = "N";
                _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Insert(objMrv);
                _unitOfWork.Save();

                SaveMrvProducts(lstMrvProducts, objMrv);
                return RedirectToAction("MrvCreation");
            }
            catch (Exception)
            {
                // ignored
            }
            return RedirectToAction("MrvCreation");
        }

        private void SaveMrvProducts(List<MRVREFERENCE> lstMrvProducts, MATERIALRECEIPTMASTER objMrv)
        {
            foreach (var prd in lstMrvProducts)
            {
                prd.MRVNO_MRR = objMrv.MRVNO_MRV;
                prd.JOBSTATUS_MRR = "N";
                _unitOfWork.Repository<MRVREFERENCE>().Insert(prd);
                _unitOfWork.Save();
                InsertJobData(prd, objMrv);
            }
        }

        private void InsertJobData(MRVREFERENCE prd, MATERIALRECEIPTMASTER objMrv)
        {
            var jobCount = (1001 + CommonModelAccessUtility.GetJobMasterCount(_unitOfWork));
            string currYear = DateTime.Now.Year.ToString();
            string jobCode = Convert.ToString("JOB/" + Convert.ToString(jobCount) + "/" + currYear);
            var jobMasterObj = _unitOfWork.Repository<JOBMASTER>().Create();
            jobMasterObj.JOBNO_JM = jobCode;
            jobMasterObj.DOCDATE_JM = objMrv.DOC_DATE_MRV;
            jobMasterObj.MRVNO_JM = objMrv.MRVNO_MRV;
            jobMasterObj.EMPCODE_JM = objMrv.EXECODE_MRV;
            jobMasterObj.JOBCODE_JM = prd.JOBID_MRR;
            jobMasterObj.JOBSTATUS_JM = prd.JOBSTATUS_MRR;
            jobMasterObj.PRODID_JIM = prd.PRODID_MRR;
            jobMasterObj.PRODQTY_JM = prd.QTY_MRR;
            _unitOfWork.Repository<JOBMASTER>().Insert(jobMasterObj);
            _unitOfWork.Save();
        }

        public JsonResult GetMrvList(string sidx, string sord, int page, int rows)
        {
            var mrvList = (from mrvMaster in _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                           select mrvMaster).Select(a => new { a.MRVNO_MRV, a.CUSTOMERCODE_MRV, a.CUSTOMERNAME_MRV });
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = mrvList.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / pageSize);
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

        public JsonResult GetJobDetailByMrv(string mrvid)
        {
            var jobData = (from jobDetails in _unitOfWork.Repository<JOBMASTER>().Query().Get()
                           join prdMaster in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                           on jobDetails.PRODID_JIM equals prdMaster.PROD_CODE_PM
                           where jobDetails.MRVNO_JM.Equals(mrvid)
                           select new { JobNo = jobDetails.JOBNO_JM, PrdCode = prdMaster.DESCRIPTION_PM, JobStatus = jobDetails.JOBSTATUS_JM }).ToList();
            return Json(new
            {
                JobData = jobData
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSaleDetailByMrv(string mrvid, string statusId)
        {
            IList<CustomSaleDetails> lstSales = new List<CustomSaleDetails>();

            var lstSaleDetails = (from saleDetails in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                where saleDetails.MRVNO_SD.Equals(mrvid) && saleDetails.STATUS_SD.Equals(statusId)
                select saleDetails).ToList();
            foreach (var sale in lstSaleDetails)
            {
                CustomSaleDetails objSales = new CustomSaleDetails
                {
                    SaleNo = sale.SLNO_SD,
                    JobNo = sale.JOBNO_SD,
                    PrCode = sale.PRCODE_SD,
                    SwCode = sale.JOBID_SD
                };
                if (!string.IsNullOrEmpty(sale.PRCODE_SD))
                {
                    var objPrd = (from prdDetails in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                                  where prdDetails.PROD_CODE_PM.Equals(sale.PRCODE_SD)
                                  select prdDetails).SingleOrDefault();
                    if (objPrd != null) objSales.Description = objPrd.DESCRIPTION_PM;
                }
                else if (!string.IsNullOrEmpty(sale.JOBID_SD))
                {
                    var objJob = (from jobDetails in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                                  where jobDetails.JOBID_JR.Equals(sale.JOBID_SD)
                                  select jobDetails).SingleOrDefault();
                    if (objJob != null) objSales.Description = objJob.JOBDESCRIPTION_JR;
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