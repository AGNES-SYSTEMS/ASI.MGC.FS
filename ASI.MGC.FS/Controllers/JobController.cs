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

        public ActionResult GetAllJobDetails()
        {
            return View();
        }

        public ActionResult JobsList()
        {
            return View();
        }

        public JsonResult getJobsList(string sidx, string sord, int page, int rows)
        {
            var JobList = (from jobList in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                           select jobList).Select(a => new {a.JOBID_JR,a.JOBDESCRIPTION_JR });
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = JobList.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            if (sord.ToUpper() == "DESC")
            {
                JobList = JobList.OrderByDescending(a => a.JOBID_JR);
                JobList = JobList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                JobList = JobList.OrderBy(a => a.JOBID_JR);
                JobList = JobList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new {
                total = totalPages,
                page,
                records = totalRecords,
                rows = JobList

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
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

        public JsonResult getJobIDs(string term)
        {
            IList<string> lstJobCodes = (from jobsList in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                                         where jobsList.JOBID_JR.StartsWith(term)
                                         select jobsList).Distinct().Select(x => x.JOBID_JR).ToList();
            return Json(lstJobCodes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getJobDetails(string term)
        {
            IList<string> lstJobDetail = (from jobsList in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                                         where jobsList.JOBDESCRIPTION_JR.Contains(term)
                                         select jobsList).Distinct().Select(x => x.JOBDESCRIPTION_JR).ToList();
            return Json(lstJobDetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getPrdRecord(string jobCode, string jobName)
        {
            JOBIDREFERENCE objJob = null;
            if (!string.IsNullOrEmpty(jobCode) && !string.IsNullOrWhiteSpace(jobCode))
            {
                objJob = (from jobList in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                               where jobList.JOBID_JR.Equals(jobCode)
                               select jobList).FirstOrDefault();
            }
            else if (!string.IsNullOrEmpty(jobName) && !string.IsNullOrWhiteSpace(jobName))
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
    }
}