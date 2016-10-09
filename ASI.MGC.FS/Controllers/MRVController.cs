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
        public JsonResult SaveMrvCreation(FormCollection form, MATERIALRECEIPTMASTER objMrv)
        {
            List<string> listMrvJobCode = new List<string>();
            try
            {
                var jsonProductDetails = form["mrvProds"];
                var serializer = new JavaScriptSerializer();
                var lstMrvProducts = serializer.Deserialize<List<MRVREFERENCE>>(jsonProductDetails);
                objMrv.DOC_DATE_MRV = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                objMrv.STATUS_MRV = "N";
                _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Insert(objMrv);
                _unitOfWork.Save();
                listMrvJobCode.Add(objMrv.MRVNO_MRV);
                SaveMrvReportObject(objMrv);
                SaveMrvProducts(lstMrvProducts, objMrv, listMrvJobCode);
            }
            catch (Exception)
            {
                // ignored
            }
            return Json(listMrvJobCode, JsonRequestBehavior.AllowGet);
            //return View("MrvCreation");
        }

        private void SaveMrvProducts(List<MRVREFERENCE> lstMrvProducts, MATERIALRECEIPTMASTER objMrv, List<string> listMrvJobCode)
        {
            _unitOfWork.Repository<MRV_REPORT_CHD>().Truncate("MRV_REPORT_CHD");
            foreach (var prd in lstMrvProducts)
            {
                prd.MRVNO_MRR = objMrv.MRVNO_MRV;
                prd.JOBSTATUS_MRR = "N";
                _unitOfWork.Repository<MRVREFERENCE>().Insert(prd);
                _unitOfWork.Save();
                InsertJobData(prd, objMrv, listMrvJobCode);
            }
        }

        private void InsertJobData(MRVREFERENCE prd, MATERIALRECEIPTMASTER objMrv, List<string> listMrvJobCode)
        {
            for (int i = 0; i < prd.QTY_MRR; i++)
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
                listMrvJobCode.Add(jobMasterObj.JOBNO_JM);
                SaveJobReortObject(jobMasterObj);
            }
            SaveMrvReportChildObject(prd);
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
                objSales.Rate = Convert.ToDouble(sale.RATE_SD);
                objSales.Discount = Convert.ToDouble(sale.DISCOUNT_SD);
                objSales.ShipChrg = Convert.ToDouble(sale.SHIPPINGCHARGES_SD);
                objSales.CashAmount = (Convert.ToInt32(sale.QTY_SD) * Convert.ToDouble(sale.RATE_SD)) - Convert.ToDouble(sale.DISCOUNT_SD) + Convert.ToDouble(sale.SHIPPINGCHARGES_SD);
                lstSales.Add(objSales);
            }

            return Json(new
            {
                lstSales
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MrvSearch()
        {
            return View();
        }

        public ActionResult SearchMrvDetails()
        {
            throw new NotImplementedException();
        }

        private void SaveMrvReportObject(MATERIALRECEIPTMASTER objMrv)
        {
            _unitOfWork.Repository<MRVNO_REPORT>().Truncate("MRVNO_REPORT");
            var mrvReportObj = _unitOfWork.Repository<MRVNO_REPORT>().Create();
            mrvReportObj.MRVNO_RPT = objMrv.MRVNO_MRV;
            mrvReportObj.MRVDATE_RPT = objMrv.DOC_DATE_MRV;
            mrvReportObj.MRVDELEDATE_RPT = objMrv.DELE_DATE_MRV;
            mrvReportObj.CUSTCODE_RPT = objMrv.CUSTOMERCODE_MRV;
            mrvReportObj.CUSTNAME_RPT = objMrv.CUSTOMERNAME_MRV;
            mrvReportObj.CUSTADD_RPT = string.Concat(objMrv.ADDRESS1_MRV, " ", objMrv.ADDRESS2_MRV);
            mrvReportObj.PHONE_RPT = objMrv.PHONE_MRV;
            _unitOfWork.Repository<MRVNO_REPORT>().Insert(mrvReportObj);
            _unitOfWork.Save();
        }

        private void SaveMrvReportChildObject(MRVREFERENCE productDetails)
        {
            var mrvReportChildObj = _unitOfWork.Repository<MRV_REPORT_CHD>().Create();
            mrvReportChildObj.MRVNO_CHD = productDetails.MRVNO_MRR;
            mrvReportChildObj.PCODE_CHD = productDetails.PRODID_MRR;
            mrvReportChildObj.JOBID_CHD = productDetails.JOBID_MRR;
            mrvReportChildObj.QTY_CHD = productDetails.QTY_MRR;
            mrvReportChildObj.RATE_CHD = productDetails.RATE_MRR;
            mrvReportChildObj.AMOUNT = productDetails.AMOUNT_MRR;
            _unitOfWork.Repository<MRV_REPORT_CHD>().Insert(mrvReportChildObj);
            _unitOfWork.Save();
        }

        private void SaveJobReortObject(JOBMASTER jobDetails)
        {
            var jobReportObj = _unitOfWork.Repository<JOBCARD_REPRT>().Create();
            jobReportObj.JOBNO_JRP = jobDetails.JOBNO_JM;
            jobReportObj.JOBDATE_JRP = Convert.ToDateTime(jobDetails.DOCDATE_JM);
            jobReportObj.MRVNO_JRP = jobDetails.MRVNO_JM;
            jobReportObj.PRODID_JRP = jobDetails.PRODID_JIM;
            _unitOfWork.Repository<JOBCARD_REPRT>().Insert(jobReportObj);
            _unitOfWork.Save();
        }
    }
}