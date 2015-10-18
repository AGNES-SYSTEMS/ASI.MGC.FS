using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;
using ASI.MGC.FS.Model.HelperClasses;
using ASI.MGC.FS.WebCommon;

namespace ASI.MGC.FS.Controllers
{
    public class JobController : Controller
    {
        readonly IUnitOfWork _unitOfWork;

        public JobController()
        {
            _unitOfWork = new UnitOfWork();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllJobsView()
        {
            return View();
        }

        public ActionResult JobsList()
        {
            return View();
        }

        public JsonResult GetJobDetailsList(string sidx, string sord, int page, int rows, string jobId, string jobDetails)
        {
            var jobList = (from jobs in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                           select jobs).Select(a => new { a.JOBID_JR, a.JOBDESCRIPTION_JR, a.RATE_RJ });
            if (!string.IsNullOrEmpty(jobId))
            {
                jobList = (from jobs in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                           where jobs.JOBID_JR.Contains(jobId)
                           select jobs).Select(a => new { a.JOBID_JR, a.JOBDESCRIPTION_JR, a.RATE_RJ });
            }
            if (!string.IsNullOrEmpty(jobDetails))
            {
                jobList = (from jobs in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                           where jobs.JOBID_JR.Contains(jobDetails)
                           select jobs).Select(a => new { a.JOBID_JR, a.JOBDESCRIPTION_JR, a.RATE_RJ });
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = jobList.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
            if (sord.ToUpper() == "DESC")
            {
                jobList = jobList.OrderByDescending(a => a.JOBID_JR);
                jobList = jobList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                jobList = jobList.OrderBy(a => a.JOBID_JR);
                jobList = jobList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = jobList

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJobMrvList(string sidx, string sord, int page, int rows, string jobStatus)
        {
            var jobList = (from jobMaster in _unitOfWork.Repository<JOBMASTER>().Query().Get()
                           select jobMaster).Select(a => new { a.JOBNO_JM, a.MRVNO_JM });
            if (!string.IsNullOrEmpty(jobStatus))
            {
                jobList = (from jobMaster in _unitOfWork.Repository<JOBMASTER>().Query().Get()
                           where jobMaster.JOBSTATUS_JM.Equals(jobStatus)
                           select jobMaster).Select(a => new { a.JOBNO_JM, a.MRVNO_JM });
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = jobList.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
            if (sord.ToUpper() == "ASC")
            {
                jobList = jobList.OrderBy(a => a.JOBNO_JM);
                jobList = jobList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                jobList = jobList.OrderByDescending(a => a.JOBNO_JM);
                jobList = jobList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = jobList

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJobMrvData(string jobId, string mrvNo)
        {
            var jobDetails = (from jobList in _unitOfWork.Repository<JOBMASTER>().Query().Get()
                              where jobList.JOBNO_JM.Equals(jobId)
                              select jobList).FirstOrDefault();
            var productDetails = (from prdList in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                                  where prdList.PROD_CODE_PM.Equals(jobDetails.PRODID_JIM)
                                  select prdList).FirstOrDefault();
            var mrvDetails = (from mrvList in _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                              where mrvList.MRVNO_MRV.Equals(mrvNo)
                              select mrvList).FirstOrDefault();
            if (mrvDetails != null)
            {
                if (productDetails != null)
                    return Json(new
                    {
                        custCode = mrvDetails.CUSTOMERCODE_MRV,
                        custName = mrvDetails.CUSTOMERNAME_MRV,
                        prdCode = productDetails.PROD_CODE_PM,
                        prdDetail = productDetails.DESCRIPTION_PM
                    }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public JsonResult GetAllJobs(jQueryDataTableParamModel param)
        {
            var totalJobRecords = (from totalJobCount in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                                   select totalJobCount);
            IEnumerable<JOBIDREFERENCE> filteredJobs;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredJobs = (from totalJobCount in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                                where totalJobCount.JOBID_JR.Contains(param.sSearch) || totalJobCount.JOBDESCRIPTION_JR.Contains(param.sSearch)
                                select totalJobCount);
            }
            else
            {
                filteredJobs = totalJobRecords;
            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<JOBIDREFERENCE, string> orderingFunction = (a => sortColumnIndex == 0 ? a.JOBID_JR : sortColumnIndex == 1 ? a.JOBDESCRIPTION_JR : Convert.ToString(a.RATE_RJ));

            var sortDirection = Request["sSortDir_0"];
            filteredJobs = sortDirection == "asc" ? filteredJobs.OrderBy(orderingFunction) : filteredJobs.OrderByDescending(orderingFunction);
            int totalRecords = totalJobRecords.Count();
            int totalDisplayedRecords = filteredJobs.Count();
            var dislpayedJobs = filteredJobs.Skip(param.iDisplayStart)
                                            .Take(param.iDisplayLength);
            var resultJobRecords = from job in dislpayedJobs select new { job.JOBID_JR, job.JOBDESCRIPTION_JR, job.RATE_RJ };
            return Json(new
            {
                param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalDisplayedRecords,
                aaData = resultJobRecords
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJobRecordById(string jobCode)
        {
            JOBIDREFERENCE objJob = null;
            if (!string.IsNullOrEmpty(jobCode) && !string.IsNullOrWhiteSpace(jobCode))
            {
                objJob = (from jobList in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                          where jobList.JOBID_JR.Equals(jobCode)
                          select jobList).FirstOrDefault();
            }
            return Json(objJob, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobRecordByName(string jobName)
        {
            JOBIDREFERENCE objJob = null;
            if (!string.IsNullOrEmpty(jobName) && !string.IsNullOrWhiteSpace(jobName))
            {
                objJob = (from jobList in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                          where jobList.JOBDESCRIPTION_JR.Equals(jobName)
                          select jobList).FirstOrDefault();
            }
            return Json(objJob, JsonRequestBehavior.AllowGet);
        }
        public ActionResult JobEntry()
        {
            ViewBag.PayModeList = CommonModelAccessUtility.GetPaymentMethodList();
            ViewBag.SaleTypeList = CommonModelAccessUtility.GetSaleTypeList();
            var objSaleEntry = new SALEDETAIL();
            return View(objSaleEntry);
        }

        public ActionResult GetJobCode()
        {
            IList<string> lstJobCodes = (from jobsList in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                                         select jobsList).Distinct().Select(x => x.JOBID_JR).ToList();
            return Json(lstJobCodes, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetJobDesc()
        {
            IList<string> lstJobDetail = (from jobsList in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                                          select jobsList).Distinct().Select(x => x.JOBDESCRIPTION_JR).ToList();
            return Json(lstJobDetail, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveJobEntry(FormCollection form, SALEDETAIL objSaleDetail)
        {
            try
            {
                var salesEntry = _unitOfWork.Repository<SALEDETAIL>().Create();
                salesEntry.MRVNO_SD = objSaleDetail.MRVNO_SD;
                salesEntry.JOBNO_SD = objSaleDetail.JOBNO_SD;
                salesEntry.PRCODE_SD = objSaleDetail.PRCODE_SD;
                salesEntry.JOBID_SD = objSaleDetail.JOBID_SD;
                salesEntry.QTY_SD = objSaleDetail.QTY_SD;
                salesEntry.UNIT_SD = objSaleDetail.UNIT_SD;
                salesEntry.RATE_SD = objSaleDetail.RATE_SD;
                salesEntry.DISCOUNT_SD = objSaleDetail.DISCOUNT_SD;
                salesEntry.SHIPPINGCHARGES_SD = objSaleDetail.SHIPPINGCHARGES_SD;
                salesEntry.ADDITIONALCHARGES_SD = objSaleDetail.ADDITIONALCHARGES_SD;
                salesEntry.SALEDATE_SD = objSaleDetail.SALEDATE_SD;
                salesEntry.CASHTOTAL_SD = objSaleDetail.CASHTOTAL_SD;
                salesEntry.CREDITTOTAL_SD = objSaleDetail.CREDITTOTAL_SD;
                salesEntry.CREDITACCODE_SD = objSaleDetail.CREDITACCODE_SD;
                salesEntry.STATUS_SD = "N";
                _unitOfWork.Repository<SALEDETAIL>().Insert(salesEntry);
                _unitOfWork.Save();
            }
            catch (Exception)
            {
                // ignored
            }

            return RedirectToAction("JobEntry");
        }

        public ActionResult GetAllJobDetails(string mrvCode, string jobCode)
        {
            return null;
        }

        public ActionResult JobCancellation()
        {
            return View();
        }

        public ActionResult SaveJobCancellation()
        {
            throw new NotImplementedException();
        }
    }
}