using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASI.MGC.FS.Model;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model.HelperClasses;
using ASI.MGC.FS.WebCommon;

namespace ASI.MGC.FS.Controllers
{
    public class JobController : Controller
    {
        IUnitOfWork _unitOfWork;

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

        public JsonResult GetJobDetailsList(string sidx, string sord, int page, int rows, string jobID, string jobDetails)
        {
            var jobList = (from jobs in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                           select jobs).Select(a => new { a.JOBID_JR, a.JOBDESCRIPTION_JR, a.RATE_RJ });
            if (!string.IsNullOrEmpty(jobID))
            {
                jobList = (from jobs in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                               where jobs.JOBID_JR.Contains(jobID)
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
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
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

        public JsonResult getJobMRVList(string sidx, string sord, int page, int rows)
        {
            var JobList = (from jobMaster in _unitOfWork.Repository<JOBMASTER>().Query().Get()
                           select jobMaster).Select(a => new { a.JOBNO_JM, a.MRVNO_JM });
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = JobList.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            if (sord.ToUpper() == "ASC")
            {
                JobList = JobList.OrderBy(a => a.JOBNO_JM);
                JobList = JobList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                JobList = JobList.OrderByDescending(a => a.JOBNO_JM);
                JobList = JobList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = JobList

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJobMRVData(string JobID, string MRVNo)
        {
            var JobDetails = (from jobList in _unitOfWork.Repository<JOBMASTER>().Query().Get()
                              where jobList.JOBNO_JM.Equals(JobID)
                              select jobList).FirstOrDefault();
            var productDetails = (from prdList in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                                  where prdList.PROD_CODE_PM.Equals(JobDetails.PRODID_JIM)
                                  select prdList).FirstOrDefault();
            var mrvDetails = (from mrvList in _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                              where mrvList.MRVNO_MRV.Equals(MRVNo)
                              select mrvList).FirstOrDefault();
            return Json(new
            {
                custCode = mrvDetails.CUSTOMERCODE_MRV,
                custName = mrvDetails.CUSTOMERNAME_MRV,
                prdCode = productDetails.PROD_CODE_PM,
                prdDetail = productDetails.DESCRIPTION_PM
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllJobs(jQueryDataTableParamModel Param)
        {
            var totalJobRecords = (from totalJobCount in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                                   select totalJobCount);
            IEnumerable<JOBIDREFERENCE> filteredJobs;
            if (!string.IsNullOrEmpty(Param.sSearch))
            {
                filteredJobs = (from totalJobCount in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                                where totalJobCount.JOBID_JR.Contains(Param.sSearch) || totalJobCount.JOBDESCRIPTION_JR.Contains(Param.sSearch)
                                select totalJobCount);
            }
            else
            {
                filteredJobs = totalJobRecords;
            }

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

            Func<JOBIDREFERENCE, string> orderingFunction = (a => sortColumnIndex == 0 ? a.JOBID_JR : sortColumnIndex == 1 ? a.JOBDESCRIPTION_JR : Convert.ToString(a.RATE_RJ));

            var sortDirection = Request["sSortDir_0"];
            if (sortDirection == "asc")
            {
                filteredJobs = filteredJobs.OrderBy(orderingFunction);
            }
            else
            {
                filteredJobs = filteredJobs.OrderByDescending(orderingFunction);
            }


            int totalRecords = totalJobRecords.Count();
            int totalDisplayedRecords = filteredJobs.Count();
            var dislpayedJobs = filteredJobs.Skip(Param.iDisplayStart)
                                            .Take(Param.iDisplayLength);
            var resultJobRecords = from Job in dislpayedJobs select new { Job.JOBID_JR, Job.JOBDESCRIPTION_JR, Job.RATE_RJ };
            return Json(new
            {
                sEcho = Param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalDisplayedRecords,
                aaData = resultJobRecords
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getJobRecordByID(string jobCode)
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
        public JsonResult getJobRecordByName(string jobName)
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
            ViewBag.PayModeList = CommonModelAccessUtility.getPaymentMethodList();
            ViewBag.SaleTypeList = CommonModelAccessUtility.getSaleTypeList();
            var objSaleEntry = new SALEDETAIL();
            return View(objSaleEntry);
        }

        public ActionResult getJobCode()
        {
            IList<string> lstJobCodes = (from jobsList in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                                         select jobsList).Distinct().Select(x => x.JOBID_JR).ToList();
            return Json(lstJobCodes, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getJobDesc()
        {
            IList<string> lstJobDetail = (from jobsList in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                                          select jobsList).Distinct().Select(x => x.JOBDESCRIPTION_JR).ToList();
            return Json(lstJobDetail, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveJobEntry(FormCollection form,SALEDETAIL objSaleDetail)
        {
            try
            {
                objSaleDetail.STATUS_SD = "N";
                _unitOfWork.Repository<SALEDETAIL>().Insert(objSaleDetail);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
            }

            return RedirectToAction("JobEntry");
        }

        public ActionResult getAllJobDetails(string MRVCode, string JobCode)
        {
            return null;
        }


    }
}