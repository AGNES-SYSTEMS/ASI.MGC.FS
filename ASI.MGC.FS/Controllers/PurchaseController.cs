﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.ExtendedAPI;
using ASI.MGC.FS.Model;
using ASI.MGC.FS.Models;
using ASI.MGC.FS.WebCommon;

namespace ASI.MGC.FS.Controllers
{
    public class PurchaseController : Controller
    {
        readonly IUnitOfWork _unitOfWork;
        readonly TimeZoneInfo tzInfo;
        DateTime today;
        public PurchaseController()
        {
            _unitOfWork = new UnitOfWork();
            tzInfo = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time");
            today = TimeZoneInfo.ConvertTime(DateTime.Now, tzInfo);
        }
        // GET: Purchase
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PurchaseEntryOld()
        {
            int purCountCode = (1001 + CommonModelAccessUtility.GetCurrPurchaseCount(_unitOfWork));
            string currYear = today.Year.ToString();
            string purCode = "CRP/" + Convert.ToString(purCountCode) + "/" + currYear;
            ViewBag.purCode = purCode;
            var objArApLedger = new AR_AP_LEDGER();
            return View(objArApLedger);
        }

        //[HttpPost]
        //public JsonResult SavePurchaseEntryOld(AR_AP_LEDGER objArApLedger, FormCollection frm)
        //{
        //    string purchaseNo = "";
        //    try
        //    {
        //        purchaseNo = Convert.ToString(frm["DocNo"]);
        //        var jsonProductDetails = frm["prdDetails"];
        //        var serializer = new JavaScriptSerializer();
        //        var lstProducts = serializer.Deserialize<List<STOCKLEDGER>>(jsonProductDetails);
        //        //var objArApLedger = _unitOfWork.Repository<AR_AP_LEDGER>().Create();
        //        objArApLedger.DOCNUMBER_ART = Convert.ToString(frm["DocNo"]);
        //        objArApLedger.ARAPCODE_ART = Convert.ToString(frm["APCode"]);
        //        objArApLedger.DODATE_ART = Convert.ToDateTime(frm["DocDate"]);
        //        objArApLedger.GLDATE_ART = Convert.ToDateTime(frm["PurDate"]);
        //        objArApLedger.OTHERREF_ART = Convert.ToString(frm["Invoice"]);
        //        objArApLedger.NARRATION_ART = Convert.ToString(frm["Note"]);
        //        objArApLedger.DEBITAMOUNT_ART = 0;
        //        objArApLedger.CREDITAMOUNT_ART = Convert.ToInt32(frm["NetAmount"]);
        //        objArApLedger.STATUS_ART = "P";
        //        _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objArApLedger);
        //        _unitOfWork.Save();

        //        var objPurchase = _unitOfWork.Repository<GLTRANSACTION1>().Create();
        //        objPurchase.DOCNUMBER_GLT = Convert.ToString(frm["DocNo"]);
        //        objPurchase.DOCDATE_GLT = Convert.ToDateTime(frm["DocDate"]);
        //        objPurchase.GLDATE_GLT = Convert.ToDateTime(frm["PurDate"]);
        //        objPurchase.GLACCODE_GLT = "4000";
        //        objPurchase.DEBITAMOUNT_GLT = Convert.ToDecimal(frm["TotalAmount"]);
        //        objPurchase.CREDITAMOUNT_GLT = 0;
        //        objPurchase.OTHERREF_GLT = Convert.ToString(frm["Invoice"]);
        //        objPurchase.NARRATION_GLT = Convert.ToString(frm["Note"]);
        //        objPurchase.GLSTATUS_GLT = "P";
        //        _unitOfWork.Repository<GLTRANSACTION1>().Insert(objPurchase);
        //        _unitOfWork.Save();

        //        if (!string.IsNullOrEmpty(frm["ShipChrg"]) && Convert.ToInt32(frm["ShipChrg"]) != 0)
        //        {
        //            var objShippingChrg = _unitOfWork.Repository<GLTRANSACTION1>().Create();
        //            objShippingChrg.DOCNUMBER_GLT = Convert.ToString(frm["DocNo"]);
        //            objShippingChrg.DOCDATE_GLT = Convert.ToDateTime(frm["DocDate"]);
        //            objShippingChrg.GLDATE_GLT = Convert.ToDateTime(frm["PurDate"]);
        //            objShippingChrg.GLACCODE_GLT = "3505";
        //            objShippingChrg.CREDITAMOUNT_GLT = 0;
        //            objShippingChrg.DEBITAMOUNT_GLT = Convert.ToDecimal(frm["ShipChrg"]);
        //            objShippingChrg.OTHERREF_GLT = Convert.ToString(frm["Invoice"]);
        //            objShippingChrg.NARRATION_GLT = Convert.ToString(frm["Note"]);
        //            objShippingChrg.GLSTATUS_GLT = "P";
        //            _unitOfWork.Repository<GLTRANSACTION1>().Insert(objShippingChrg);
        //            _unitOfWork.Save();
        //        }

        //        if (!string.IsNullOrEmpty(frm["Discount"]) && Convert.ToInt32(frm["Discount"]) != 0)
        //        {
        //            var objDiscount = _unitOfWork.Repository<GLTRANSACTION1>().Create();
        //            objDiscount.DOCNUMBER_GLT = Convert.ToString(frm["DocNo"]);
        //            objDiscount.DOCDATE_GLT = Convert.ToDateTime(frm["DocDate"]);
        //            objDiscount.GLDATE_GLT = Convert.ToDateTime(frm["PurDate"]);
        //            objDiscount.GLACCODE_GLT = "3501";
        //            objDiscount.CREDITAMOUNT_GLT = Convert.ToDecimal(frm["Discount"]);
        //            objDiscount.DEBITAMOUNT_GLT = 0;
        //            objDiscount.OTHERREF_GLT = Convert.ToString(frm["Invoice"]);
        //            objDiscount.NARRATION_GLT = Convert.ToString(frm["Note"]);
        //            objDiscount.GLSTATUS_GLT = "P";
        //            _unitOfWork.Repository<GLTRANSACTION1>().Insert(objDiscount);
        //            _unitOfWork.Save();
        //        }

        //        foreach (var prd in lstProducts)
        //        {
        //            prd.VOUCHERNO_SL = Convert.ToString(frm["DocNo"]);
        //            prd.OTHERREF_SL = Convert.ToString(frm["Invoice"]);
        //            prd.DOC_DATE_SL = Convert.ToDateTime(frm["DocDate"]);
        //            prd.LEDGER_DATE_SL = Convert.ToDateTime(frm["PurDate"]);
        //            prd.STATUS_SL = "P";
        //            _unitOfWork.Repository<STOCKLEDGER>().Insert(prd);
        //            _unitOfWork.Save();

        //            UpdateProductMaster(prd);
        //        }

        //    }
        //    catch (Exception)
        //    {
        //        // ignored
        //    }
        //    return Json(purchaseNo, JsonRequestBehavior.AllowGet);
        //}

        private void UpdateProductMaster(STOCKLEDGER prd)
        {
            var products = (from prdList in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                            where prdList.PROD_CODE_PM.Equals(prd.PRODID_SL)
                            select prdList);
            foreach (var product in products.ToList())
            {
                product.CUR_QTY_PM = product.CUR_QTY_PM + prd.RECEPT_QTY_SL - prd.ISSUE_QTY_SL;
                _unitOfWork.Repository<PRODUCTMASTER>().Update(product);
                _unitOfWork.Save();
            }
        }
        [MesAuthorize("DailyTransactions")]
        public ActionResult PurchaseReturn()
        {
            int retCount = (1001 + CommonModelAccessUtility.GetReturnPurchaseCount(_unitOfWork));
            string currYear = today.Year.ToString();
            string revPurCode = "RPC/" + Convert.ToString(retCount) + "/" + currYear;
            ViewBag.RevPurCode = revPurCode;
            ViewBag.Today = today.ToShortDateString();
            var objPurchase = new PurchaseModel();
            return View(objPurchase);
        }

        [HttpPost]
        public JsonResult SavePurchaseReturn(PurchaseModel objPurchaseModal, FormCollection frm)
        {
            string purchaseReturnNo = "";
            string currentUser = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    purchaseReturnNo = Convert.ToString(frm["DocNo"]);
                    var jsonProductDetails = frm["prdDetails"];
                    var serializer = new JavaScriptSerializer();
                    var lstProducts = serializer.Deserialize<List<STOCKLEDGER>>(jsonProductDetails);
                    var objArApLedger = _unitOfWork.Repository<AR_AP_LEDGER>().Create();
                    objArApLedger.DOCNUMBER_ART = Convert.ToString(frm["DocNo"]);
                    objArApLedger.ARAPCODE_ART = Convert.ToString(frm["APCode"]);
                    objArApLedger.DODATE_ART = Convert.ToDateTime(frm["DocDate"]);
                    objArApLedger.GLDATE_ART = Convert.ToDateTime(frm["PurDate"]);
                    objArApLedger.OTHERREF_ART = Convert.ToString(frm["Invoice"]);
                    objArApLedger.NARRATION_ART = Convert.ToString(frm["Note"]);
                    objArApLedger.MATCHVALUE_AR = 0;
                    objArApLedger.CREDITAMOUNT_ART = 0;
                    objArApLedger.DEBITAMOUNT_ART = Convert.ToDecimal(frm["NetAmount"]);
                    objArApLedger.USER_ART = currentUser;
                    objArApLedger.STATUS_ART = "P";
                    _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objArApLedger);
                    _unitOfWork.Save();

                    var objPurchase = _unitOfWork.Repository<GLTRANSACTION1>().Create();
                    objPurchase.DOCNUMBER_GLT = Convert.ToString(frm["DocNo"]);
                    objPurchase.DOCDATE_GLT = Convert.ToDateTime(frm["DocDate"]);
                    objPurchase.GLDATE_GLT = Convert.ToDateTime(frm["PurDate"]);
                    objPurchase.GLACCODE_GLT = "4001";
                    objPurchase.CREDITAMOUNT_GLT = Convert.ToDecimal(frm["TotalAmount"]);
                    objPurchase.DEBITAMOUNT_GLT = 0;
                    objPurchase.OTHERREF_GLT = Convert.ToString(frm["Invoice"]);
                    objPurchase.NARRATION_GLT = Convert.ToString(frm["Note"]);
                    objPurchase.VARUSER = currentUser;
                    objPurchase.GLSTATUS_GLT = "P";
                    _unitOfWork.Repository<GLTRANSACTION1>().Insert(objPurchase);
                    _unitOfWork.Save();

                    if (Convert.ToBoolean(frm["hdnIncludeVAT"]) && !string.IsNullOrEmpty(frm["TotalVAT"]) && Convert.ToDecimal(frm["TotalVAT"]) > 0)
                    {
                        var objVATChrg = _unitOfWork.Repository<GLTRANSACTION1>().Create();
                        objVATChrg.DOCNUMBER_GLT = Convert.ToString(frm["DocNo"]);
                        objVATChrg.DOCDATE_GLT = Convert.ToDateTime(frm["DocDate"]);
                        objVATChrg.GLDATE_GLT = Convert.ToDateTime(frm["PurDate"]);
                        objVATChrg.GLACCODE_GLT = "3511";
                        objVATChrg.CREDITAMOUNT_GLT = Convert.ToDecimal(frm["TotalVAT"]);
                        objVATChrg.DEBITAMOUNT_GLT = 0;
                        objVATChrg.OTHERREF_GLT = Convert.ToString(frm["Invoice"]);
                        objVATChrg.NARRATION_GLT = Convert.ToString(frm["Note"]);
                        objVATChrg.GLSTATUS_GLT = "P";
                        objVATChrg.VARUSER = currentUser;
                        _unitOfWork.Repository<GLTRANSACTION1>().Insert(objVATChrg);
                        _unitOfWork.Save();
                    }

                    foreach (var prd in lstProducts)
                    {
                        prd.VOUCHERNO_SL = Convert.ToString(frm["DocNo"]);
                        prd.OTHERREF_SL = Convert.ToString(frm["Invoice"]);
                        prd.DOC_DATE_SL = Convert.ToDateTime(frm["DocDate"]);
                        prd.LEDGER_DATE_SL = Convert.ToDateTime(frm["PurDate"]);
                        prd.STATUS_SL = "P";
                        _unitOfWork.Repository<STOCKLEDGER>().Insert(prd);
                        _unitOfWork.Save();

                        UpdateProductMaster(prd);
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            return Json(purchaseReturnNo, JsonRequestBehavior.AllowGet);
        }
        [MesAuthorize("DailyTransactions")]
        public ActionResult PurchaseEntry()
        {
            int purCountCode = (1001 + CommonModelAccessUtility.GetCurrPurchaseCount(_unitOfWork));
            string currYear = today.Year.ToString();
            string purCode = "CRP/" + Convert.ToString(purCountCode) + "/" + currYear;
            ViewBag.purCode = purCode;
            ViewBag.Today = today.ToShortDateString();
            var objPurchase = new PurchaseModel();
            return View(objPurchase);
        }
        [HttpPost]
        public JsonResult SavePurchaseEntry(PurchaseModel objPurchaseModal, FormCollection frm)
        {
            string purchaseNo = "";
            string currentUser = CommonModelAccessUtility.GetCurrentUser(_unitOfWork);
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    purchaseNo = Convert.ToString(frm["DocNo"]);
                    var jsonProductDetails = frm["prdDetails"];
                    var serializer = new JavaScriptSerializer();
                    var lstProducts = serializer.Deserialize<List<STOCKLEDGER>>(jsonProductDetails);
                    var objArApLedger = _unitOfWork.Repository<AR_AP_LEDGER>().Create();
                    objArApLedger.DOCNUMBER_ART = Convert.ToString(frm["DocNo"]);
                    objArApLedger.ARAPCODE_ART = Convert.ToString(frm["APCode"]);
                    objArApLedger.DODATE_ART = Convert.ToDateTime(frm["DocDate"]);
                    objArApLedger.GLDATE_ART = Convert.ToDateTime(frm["PurDate"]);
                    objArApLedger.OTHERREF_ART = Convert.ToString(frm["Invoice"]);
                    objArApLedger.NARRATION_ART = Convert.ToString(frm["Note"]);
                    objArApLedger.DEBITAMOUNT_ART = 0;
                    objArApLedger.MATCHVALUE_AR = 0;
                    objArApLedger.CREDITAMOUNT_ART = Convert.ToDecimal(frm["NetAmount"]);
                    objArApLedger.USER_ART = currentUser;
                    objArApLedger.STATUS_ART = "P";
                    _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objArApLedger);
                    _unitOfWork.Save();

                    var objPurchase = _unitOfWork.Repository<GLTRANSACTION1>().Create();
                    objPurchase.DOCNUMBER_GLT = Convert.ToString(frm["DocNo"]);
                    objPurchase.DOCDATE_GLT = Convert.ToDateTime(frm["DocDate"]);
                    objPurchase.GLDATE_GLT = Convert.ToDateTime(frm["PurDate"]);
                    objPurchase.GLACCODE_GLT = "4000";
                    objPurchase.DEBITAMOUNT_GLT = Convert.ToDecimal(frm["TotalAmount"]);
                    objPurchase.CREDITAMOUNT_GLT = 0;
                    objPurchase.OTHERREF_GLT = Convert.ToString(frm["Invoice"]);
                    objPurchase.NARRATION_GLT = Convert.ToString(frm["Note"]);
                    objPurchase.VARUSER = currentUser;
                    objPurchase.GLSTATUS_GLT = "P";
                    _unitOfWork.Repository<GLTRANSACTION1>().Insert(objPurchase);
                    _unitOfWork.Save();

                    if (!string.IsNullOrEmpty(frm["ShipChrg"]) && Convert.ToDecimal(frm["ShipChrg"]) != 0)
                    {
                        var objShippingChrg = _unitOfWork.Repository<GLTRANSACTION1>().Create();
                        objShippingChrg.DOCNUMBER_GLT = Convert.ToString(frm["DocNo"]);
                        objShippingChrg.DOCDATE_GLT = Convert.ToDateTime(frm["DocDate"]);
                        objShippingChrg.GLDATE_GLT = Convert.ToDateTime(frm["PurDate"]);
                        objShippingChrg.GLACCODE_GLT = "3505";
                        objShippingChrg.CREDITAMOUNT_GLT = 0;
                        objShippingChrg.DEBITAMOUNT_GLT = Convert.ToDecimal(frm["ShipChrg"]);
                        objShippingChrg.OTHERREF_GLT = Convert.ToString(frm["Invoice"]);
                        objShippingChrg.NARRATION_GLT = Convert.ToString(frm["Note"]);
                        objShippingChrg.GLSTATUS_GLT = "P";
                        objShippingChrg.VARUSER = currentUser;
                        _unitOfWork.Repository<GLTRANSACTION1>().Insert(objShippingChrg);
                        _unitOfWork.Save();
                    }
                    if (Convert.ToBoolean(frm["hdnIncludeVAT"]) && !string.IsNullOrEmpty(frm["TotalVAT"]) && Convert.ToDecimal(frm["TotalVAT"]) > 0)
                    {
                        var objVATChrg = _unitOfWork.Repository<GLTRANSACTION1>().Create();
                        objVATChrg.DOCNUMBER_GLT = Convert.ToString(frm["DocNo"]);
                        objVATChrg.DOCDATE_GLT = Convert.ToDateTime(frm["DocDate"]);
                        objVATChrg.GLDATE_GLT = Convert.ToDateTime(frm["PurDate"]);
                        objVATChrg.GLACCODE_GLT = "3510";
                        objVATChrg.CREDITAMOUNT_GLT = 0;
                        objVATChrg.DEBITAMOUNT_GLT = Convert.ToDecimal(frm["TotalVAT"]);
                        objVATChrg.OTHERREF_GLT = Convert.ToString(frm["Invoice"]);
                        objVATChrg.NARRATION_GLT = Convert.ToString(frm["Note"]);
                        objVATChrg.GLSTATUS_GLT = "P";
                        objVATChrg.VARUSER = currentUser;
                        _unitOfWork.Repository<GLTRANSACTION1>().Insert(objVATChrg);
                        _unitOfWork.Save();
                    }

                    if (!string.IsNullOrEmpty(frm["Discount"]) && Convert.ToDecimal(frm["Discount"]) != 0)
                    {
                        var objDiscount = _unitOfWork.Repository<GLTRANSACTION1>().Create();
                        objDiscount.DOCNUMBER_GLT = Convert.ToString(frm["DocNo"]);
                        objDiscount.DOCDATE_GLT = Convert.ToDateTime(frm["DocDate"]);
                        objDiscount.GLDATE_GLT = Convert.ToDateTime(frm["PurDate"]);
                        objDiscount.GLACCODE_GLT = "3501";
                        objDiscount.CREDITAMOUNT_GLT = Convert.ToDecimal(frm["Discount"]);
                        objDiscount.DEBITAMOUNT_GLT = 0;
                        objDiscount.OTHERREF_GLT = Convert.ToString(frm["Invoice"]);
                        objDiscount.NARRATION_GLT = Convert.ToString(frm["Note"]);
                        objDiscount.VARUSER = currentUser;
                        objDiscount.GLSTATUS_GLT = "P";
                        _unitOfWork.Repository<GLTRANSACTION1>().Insert(objDiscount);
                        _unitOfWork.Save();
                    }

                    foreach (var prd in lstProducts)
                    {
                        prd.VOUCHERNO_SL = Convert.ToString(frm["DocNo"]);
                        prd.OTHERREF_SL = Convert.ToString(frm["Invoice"]);
                        prd.DOC_DATE_SL = Convert.ToDateTime(frm["DocDate"]);
                        prd.LEDGER_DATE_SL = Convert.ToDateTime(frm["PurDate"]);
                        prd.STATUS_SL = "P";
                        _unitOfWork.Repository<STOCKLEDGER>().Insert(prd);
                        _unitOfWork.Save();

                        UpdateProductMaster(prd);
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            return Json(purchaseNo, JsonRequestBehavior.AllowGet);
        }
    }
}