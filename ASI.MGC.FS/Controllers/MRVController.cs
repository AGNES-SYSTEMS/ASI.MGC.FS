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
                //DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(MRVREFERENCE));
                var lstMrvProducts = serializer.Deserialize<List<MRVREFERENCE>>(jsonProductDetails);
                foreach (var prd in lstMrvProducts)
                {
                    MRVREFERENCE objMrvRef = new MRVREFERENCE();
                    objMrvRef.MRVNO_MRR = mrvNo;
                    objMrvRef.PRODID_MRR = prd.PRODID_MRR;
                    objMrvRef.JOBID_MRR = prd.JOBID_MRR;
                    objMrvRef.QTY_MRR = prd.QTY_MRR;
                    objMrvRef.RATE_MRR = prd.RATE_MRR;
                    objMrvRef.AMOUNT_MRR = prd.AMOUNT_MRR;
                    objMrvRef.JOBSTATUS_MRR = "N";
                    _unitOfWork.Repository<MRVREFERENCE>().Insert(objMrvRef);
                    _unitOfWork.Save();
                }
                objMRV.DOC_DATE_MRV = Convert.ToDateTime(System.DateTime.Now.ToShortDateString());
                objMRV.STATUS_MRV = "N";
                return RedirectToAction("MRVCreation");
            }
            catch (Exception e)
            {

            }
            return RedirectToAction("MRVCreation");
        }
    }
}