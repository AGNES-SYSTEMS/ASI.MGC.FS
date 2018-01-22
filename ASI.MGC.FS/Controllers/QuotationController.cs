using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.ExtendedAPI;
using ASI.MGC.FS.Model;
using ASI.MGC.FS.Models;
using ASI.MGC.FS.WebCommon;

namespace ASI.MGC.FS.Controllers
{
    public class QuotationController : Controller
    {
        readonly IUnitOfWork _unitOfWork;
        readonly TimeZoneInfo tzInfo;
        DateTime today;
        public QuotationController()
        {
            _unitOfWork = new UnitOfWork();
            tzInfo = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time");
            today = TimeZoneInfo.ConvertTime(DateTime.Now, tzInfo);
        }
        // GET: Quotation
        public ActionResult Index()
        {
            return View();
        }
        [MesAuthorize("DailyTransactions")]
        public ActionResult QuotationEntry()
        {
            var currYear = today.Year.ToString();
            var qotCount = 1001 + CommonModelAccessUtility.GetQuotationCount(_unitOfWork);
            var qotNumber = Convert.ToString("QOT" + "/" + qotCount + "/" + currYear);
            ViewBag.QotNumber = qotNumber;
            ViewBag.Today = today.ToShortDateString();
            var objQuotationMaster = new QUOTATION_MASTER();
            return View(objQuotationMaster);
        }

        [HttpPost]
        public JsonResult SaveQuotation(FormCollection form, QUOTATION_MASTER objQuotationMaster)
        {
            string quotNo = "";
            var prdCount = 0;
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    quotNo = objQuotationMaster.QUOTNO_QM;
                    _unitOfWork.Repository<QUOTATION_MASTER>().Insert(objQuotationMaster);
                    _unitOfWork.Save();

                    string jsonPrdDetails = form["quotProds"];
                    var serializer = new JavaScriptSerializer();
                    var lstPrdDetails = serializer.Deserialize<List<QuotationCustom>>(jsonPrdDetails);
                    foreach (var prd in lstPrdDetails)
                    {
                        prdCount++;
                        var objQuotProduct = _unitOfWork.Repository<QUOT_PROD_MASTER>().Create();
                        objQuotProduct.QUOTNO_QPRM = Convert.ToString(objQuotationMaster.QUOTNO_QM);
                        objQuotProduct.PRODID_QPRM = Convert.ToString(prd.PrCode);
                        objQuotProduct.QTY_QPRM = Convert.ToInt32(prd.Qty);
                        objQuotProduct.SLNO_QPRM = Convert.ToInt32(prdCount);
                        _unitOfWork.Repository<QUOT_PROD_MASTER>().Insert(objQuotProduct);
                        _unitOfWork.Save();

                        var objQuotationRef = _unitOfWork.Repository<QOTATION_REF>().Create();
                        objQuotationRef.QUOTNO_QREF = Convert.ToString(objQuotationMaster.QUOTNO_QM);
                        objQuotationRef.CODE_QREF = Convert.ToString(prd.JobId);
                        objQuotationRef.DESCRIPTION_QREF = Convert.ToString(prd.JobDesc);
                        objQuotationRef.QTY_QREF = Convert.ToInt32(prd.Qty);
                        objQuotationRef.RATE_QREF = Convert.ToDecimal(prd.Rate);
                        objQuotationRef.AMOUNT_QREF = Convert.ToDecimal(prd.Rate * prd.Qty);
                        objQuotationRef.ID_QREF = Convert.ToInt32(prdCount);
                        objQuotationRef.VAT_QREF = Convert.ToDecimal(prd.VAT);
                        _unitOfWork.Repository<QOTATION_REF>().Insert(objQuotationRef);
                        _unitOfWork.Save();
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            return Json(quotNo, JsonRequestBehavior.AllowGet);
        }
    }
}