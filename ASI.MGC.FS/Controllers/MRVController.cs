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

namespace ASI.MGC.FS.Controllers
{
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
                saveMRVProducts(lstMrvProducts, mrvNo);
                return RedirectToAction("MRVCreation");
            }
            catch (Exception e)
            {

            }
            return RedirectToAction("MRVCreation");
        }

        private void saveMRVProducts(List<MRVREFERENCE> lstMrvProducts, string mrvNo)
        {
            foreach (var prd in lstMrvProducts)
            {
                prd.MRVNO_MRR = mrvNo;
                prd.JOBSTATUS_MRR = "N";
                _unitOfWork.Repository<MRVREFERENCE>().Insert(prd);
                _unitOfWork.Save();
            }
        }
    }
}