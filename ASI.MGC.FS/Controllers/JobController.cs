using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Domain.Repositories;
using ASI.MGC.FS.ExtendedAPI;
using ASI.MGC.FS.Model;
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

        public JsonResult GetJobDetailsList(string sidx, string sord, int page, int rows, string jobId, string jobName)
        {
            var jobList = (from jobs in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                           select jobs).Select(a => new { a.JOBID_JR, a.JOBDESCRIPTION_JR, a.RATE_RJ });
            if (!string.IsNullOrEmpty(jobId))
            {
                jobList = jobList.Where(a => a.JOBID_JR.Contains(jobId)).Select(a => new { a.JOBID_JR, a.JOBDESCRIPTION_JR, a.RATE_RJ });
            }
            if (!string.IsNullOrEmpty(jobName))
            {
                jobList = jobList.Where(a => a.JOBDESCRIPTION_JR.Contains(jobName)).Select(a => new { a.JOBID_JR, a.JOBDESCRIPTION_JR, a.RATE_RJ });
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

        public JsonResult GetJobMrvList(string sidx, string sord, int page, int rows, string jobStatus, string mrvNo, string jobNo)
        {
            var jobList = (from jobMaster in _unitOfWork.Repository<JOBMASTER>().Query().Get()
                           select jobMaster).Select(a => new { a.JOBNO_JM, a.MRVNO_JM, a.DOCDATE_JM });
            if (!string.IsNullOrEmpty(jobStatus))
            {
                jobList = (from jobMaster in _unitOfWork.Repository<JOBMASTER>().Query().Get()
                           where jobMaster.JOBSTATUS_JM.Equals(jobStatus)
                           select jobMaster).Select(a => new { a.JOBNO_JM, a.MRVNO_JM, a.DOCDATE_JM });
            }
            if (!string.IsNullOrEmpty(jobNo))
            {
                jobList = jobList.Where(a => a.JOBNO_JM.Contains(jobNo)).Select(a => new { a.JOBNO_JM, a.MRVNO_JM, a.DOCDATE_JM });
            }
            if (!string.IsNullOrEmpty(mrvNo))
            {
                jobList = jobList.Where(a => a.MRVNO_JM.Contains(mrvNo)).Select(a => new { a.JOBNO_JM, a.MRVNO_JM, a.DOCDATE_JM });
            }

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = jobList.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
            if (sord.ToUpper() == "ASC")
            {
                jobList = jobList.OrderBy(a => a.DOCDATE_JM);
                jobList = jobList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                jobList = jobList.OrderByDescending(a => a.DOCDATE_JM);
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
        //public JsonResult GetAllJobs(jQueryDataTableParamModel param)
        //{
        //    var totalJobRecords = (from totalJobCount in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
        //                           select totalJobCount);
        //    IEnumerable<JOBIDREFERENCE> filteredJobs;
        //    if (!string.IsNullOrEmpty(param.sSearch))
        //    {
        //        filteredJobs = (from totalJobCount in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
        //                        where totalJobCount.JOBID_JR.Contains(param.sSearch) || totalJobCount.JOBDESCRIPTION_JR.Contains(param.sSearch)
        //                        select totalJobCount);
        //    }
        //    else
        //    {
        //        filteredJobs = totalJobRecords;
        //    }

        //    var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);

        //    Func<JOBIDREFERENCE, string> orderingFunction = (a => sortColumnIndex == 0 ? a.JOBID_JR : sortColumnIndex == 1 ? a.JOBDESCRIPTION_JR : Convert.ToString(a.RATE_RJ));

        //    var sortDirection = Request["sSortDir_0"];
        //    filteredJobs = sortDirection == "asc" ? filteredJobs.OrderBy(orderingFunction) : filteredJobs.OrderByDescending(orderingFunction);
        //    int totalRecords = totalJobRecords.Count();
        //    int totalDisplayedRecords = filteredJobs.Count();
        //    var dislpayedJobs = filteredJobs.Skip(param.iDisplayStart)
        //                                    .Take(param.iDisplayLength);
        //    var resultJobRecords = from job in dislpayedJobs select new { job.JOBID_JR, job.JOBDESCRIPTION_JR, job.RATE_RJ };
        //    return Json(new
        //    {
        //        param.sEcho,
        //        iTotalRecords = totalRecords,
        //        iTotalDisplayRecords = totalDisplayedRecords,
        //        aaData = resultJobRecords
        //    }, JsonRequestBehavior.AllowGet);
        //}

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
        [MesAuthorize("DailyTransactions")]
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
        public JsonResult SaveJobEntry(FormCollection form, SALEDETAIL objSaleDetail)
        {
            string currentUser = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);
            bool success = false;
            bool isEntryExist = false;
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    if (!string.IsNullOrEmpty(objSaleDetail.JOBID_SD))
                    {
                        var currJobSale = (from saleData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                           where saleData.JOBNO_SD.Equals(objSaleDetail.JOBNO_SD)
                                           && saleData.JOBID_SD.Equals(objSaleDetail.JOBID_SD)
                                           select saleData).FirstOrDefault();
                        if (currJobSale != null)
                        {
                            currJobSale.QTY_SD = currJobSale.QTY_SD + objSaleDetail.QTY_SD;
                            if (form["PayMode"] == "Credit")
                            {
                                currJobSale.CREDITTOTAL_SD = (currJobSale.QTY_SD * currJobSale.RATE_SD) - currJobSale.DISCOUNT_SD + currJobSale.SHIPPINGCHARGES_SD;
                            }
                            else
                            {
                                currJobSale.CASHTOTAL_SD = (currJobSale.QTY_SD * currJobSale.RATE_SD) - currJobSale.DISCOUNT_SD + currJobSale.SHIPPINGCHARGES_SD;
                            }
                            _unitOfWork.Repository<SALEDETAIL>().Update(currJobSale);
                            _unitOfWork.Save();
                            isEntryExist = true;
                        }
                    }
                    else if (!string.IsNullOrEmpty(objSaleDetail.PRCODE_SD))
                    {
                        var currPrdSale = (from saleData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                           where saleData.JOBNO_SD.Equals(objSaleDetail.JOBNO_SD)
                                           && saleData.PRCODE_SD.Equals(objSaleDetail.PRCODE_SD)
                                           select saleData).FirstOrDefault();
                        if (currPrdSale != null)
                        {
                            currPrdSale.QTY_SD = currPrdSale.QTY_SD + objSaleDetail.QTY_SD;
                            if (form["PayMode"] == "Credit")
                            {
                                currPrdSale.CREDITTOTAL_SD = (currPrdSale.QTY_SD * currPrdSale.RATE_SD) + currPrdSale.SHIPPINGCHARGES_SD - currPrdSale.DISCOUNT_SD;
                            }
                            else
                            {
                                currPrdSale.CREDITTOTAL_SD = (currPrdSale.QTY_SD * currPrdSale.RATE_SD) + currPrdSale.SHIPPINGCHARGES_SD - currPrdSale.DISCOUNT_SD;
                            }
                            _unitOfWork.Repository<SALEDETAIL>().Update(currPrdSale);
                            _unitOfWork.Save();
                            isEntryExist = true;
                        }
                    }

                    if (isEntryExist == false)
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
                        salesEntry.USERID_SD = currentUser;
                        salesEntry.STATUS_SD = "N";
                        _unitOfWork.Repository<SALEDETAIL>().Insert(salesEntry);
                        _unitOfWork.Save();
                        JobMaster_UpdateEmpCode(form["EmpCode"], objSaleDetail.JOBNO_SD);
                    }
                    success = true;
                    transaction.Commit();
                }
                catch (Exception)
                {
                    success = false;
                    transaction.Rollback();
                }
            }
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllJobDetails(string mrvCode, string jobCode)
        {
            return null;
        }
        [MesAuthorize("DailyTransactions")]
        public ActionResult JobCancellation()
        {
            return View();
        }
        [HttpPost]
        public JsonResult SaveJobCancellation(FormCollection frm, JOBMASTER objJmaster)
        {
            bool isJobPosted = true;
            bool success = false;
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var postedJobCount = (from sData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                          where sData.JOBNO_SD.Equals(objJmaster.JOBNO_JM) && sData.STATUS_SD.Equals("P")
                                          select sData).Count();
                    if (postedJobCount == 0)
                    {
                        isJobPosted = false;
                        var jobSaleData = (from sData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                           where sData.JOBNO_SD.Equals(objJmaster.JOBNO_JM)
                                           select sData).ToList();
                        foreach (var item in jobSaleData)
                        {
                            _unitOfWork.Repository<SALEDETAIL>().Delete(item);
                            _unitOfWork.Save();
                        }
                        var jobMasterData = (from jmData in _unitOfWork.Repository<JOBMASTER>().Query().Get()
                                             where jmData.JOBNO_JM.Equals(objJmaster.JOBNO_JM)
                                             select jmData).ToList();
                        foreach (var item in jobMasterData)
                        {
                            item.REMARIKS_JM = objJmaster.REMARIKS_JM;
                            item.JOBSTATUS_JM = "C";
                            _unitOfWork.Repository<JOBMASTER>().Update(item);
                            _unitOfWork.Save();
                        }
                        var mrvJobsCount = (from mrv in _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                                            join jmData in _unitOfWork.Repository<JOBMASTER>().Query().Get()
                                                on mrv.MRVNO_MRV equals jmData.MRVNO_JM
                                            where jmData.JOBSTATUS_JM != "C"
                                            select jmData).Count();
                        if (mrvJobsCount == 0)
                        {
                            var mrvItem = (from mrv in _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                                           where mrv.MRVNO_MRV.Equals(objJmaster.MRVNO_JM)
                                           select mrv).SingleOrDefault();
                            if (mrvItem != null)
                            {
                                mrvItem.STATUS_MRV = "C";
                                _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Update(mrvItem);
                                _unitOfWork.Save();
                            }
                        }
                        var JobData = (from jmData in _unitOfWork.Repository<JOBMASTER>().Query().Get()
                                       where jmData.JOBNO_JM.Equals(objJmaster.JOBNO_JM)
                                       select jmData).FirstOrDefault();
                        if (JobData != null)
                        {
                            JobData.EMPCODE_JM = objJmaster.EMPCODE_JM;
                            _unitOfWork.Repository<JOBMASTER>().Update(JobData);
                            _unitOfWork.Save();
                        }
                        success = true;
                        transaction.Commit();
                    }
                }
                catch
                {
                    success = false;
                    transaction.Rollback();
                }
            }
            return Json(new { isJobPosted = isJobPosted, success = success }, JsonRequestBehavior.AllowGet);
        }
        private void JobMaster_UpdateEmpCode(string empCode, string jobNo)
        {
            try
            {
                var objSales = _unitOfWork.Repository<JOBMASTER>().FindByID(jobNo);
                objSales.EMPCODE_JM = empCode;
                _unitOfWork.Repository<JOBMASTER>().Update(objSales);
                _unitOfWork.Save();
            }
            catch (Exception)
            {
            }
        }
        public JsonResult DeleteJobsbyJobId(string jobId)
        {
            var repo = _unitOfWork.ExtRepositoryFor<ReportRepository>();
            var success = repo.Sp_DeleteSalesByJobId(jobId);
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSalesbyJobId(string sidx, string sord, int page, int rows, string jobCode)
        {
            var repo = _unitOfWork.ExtRepositoryFor<ReportRepository>();
            var jobDetails = repo.Sp_GetSalesByJobNo(jobCode);
            return Json(jobDetails, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteSalebyId(int salesId)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var objSales = _unitOfWork.Repository<SALEDETAIL>().FindByID(salesId);
                    _unitOfWork.Repository<SALEDETAIL>().Delete(objSales);
                    _unitOfWork.Save();
                    transaction.Commit();
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public JsonResult GetJobData(string sidx, string sord, int page, int rows, string jobNo)
        {
            var pendingJobsList = (from jmData in _unitOfWork.Repository<JOBMASTER>().Query().Get()
                                   where jmData.JOBSTATUS_JM.Equals("N")
                                   select new { jmData.JOBNO_JM, jmData.MRVNO_JM });
            if (!string.IsNullOrEmpty(jobNo))
            {
                pendingJobsList = pendingJobsList.Where(o => o.JOBNO_JM.Contains(jobNo));
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = pendingJobsList.Count();
            int totalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
            if (sord.ToUpper() == "DESC")
            {
                pendingJobsList = pendingJobsList.OrderByDescending(a => a.JOBNO_JM);
                pendingJobsList = pendingJobsList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                pendingJobsList = pendingJobsList.OrderBy(a => a.JOBNO_JM);
                pendingJobsList = pendingJobsList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = pendingJobsList

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getPendingJobDetails(string jobNo)
        {
            try
            {
                string prdName = "";
                string empName = "";
                string custName = "";
                string custCode = "";
                var jobData = (from jmData in _unitOfWork.Repository<JOBMASTER>().Query().Get()
                               where jmData.JOBNO_JM.Equals(jobNo)
                               select new { jmData.MRVNO_JM, jmData.DOCDATE_JM, jmData.PRODID_JIM, jmData.EMPCODE_JM, jmData.REMARIKS_JM }).SingleOrDefault();
                if (jobData != null)
                {
                    var prodDetail = (from pmData in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                                      where pmData.PROD_CODE_PM.Equals(jobData.PRODID_JIM)
                                      select pmData).SingleOrDefault();
                    if (prodDetail != null)
                    {
                        prdName = prodDetail.DESCRIPTION_PM;
                    }
                    var empData = (from empMData in _unitOfWork.Repository<EMPLOYEEMASTER>().Query().Get()
                                   where empMData.EMPCODE_EM.Equals(jobData.EMPCODE_JM)
                                   select empMData).SingleOrDefault();
                    if (empData != null)
                    {
                        empName = empData.EMPFNAME_EM;
                    }
                    var mrvData = (from mrvMData in _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                                   where mrvMData.MRVNO_MRV.Equals(jobData.MRVNO_JM)
                                   select mrvMData).SingleOrDefault();
                    if (mrvData != null)
                    {
                        custCode = mrvData.CUSTOMERCODE_MRV;
                        custName = mrvData.CUSTOMERNAME_MRV;
                    }
                    var jsonData = new
                    {
                        mrvNo = jobData.MRVNO_JM,
                        prodCode = jobData.PRODID_JIM,
                        prodName = prdName,
                        docDate = jobData.DOCDATE_JM,
                        empCode = jobData.EMPCODE_JM,
                        empName = empName,
                        custCode = custCode,
                        custName = custName,
                        details = jobData.REMARIKS_JM
                    };
                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {

            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }
}