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
                insertJobData(prd,objMRV);
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
    }
}