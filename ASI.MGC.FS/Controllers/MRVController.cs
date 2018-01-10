using System;
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
    public class MrvController : Controller
    {
        readonly IUnitOfWork _unitOfWork;
        readonly TimeZoneInfo timeZoneInfo;
        public MrvController()
        {
            _unitOfWork = new UnitOfWork();
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time");
        }
        public ActionResult Index()
        {
            return View();
        }

        [MesAuthorize("DailyTransactions")]
        public ActionResult MrvCreation()
        {
            string mrvCode = CommonModelAccessUtility.GetCurrMrvCount(_unitOfWork);
            ViewBag.MRVCode = mrvCode;
            var objMrv = new MATERIALRECEIPTMASTER();
            return View(objMrv);
        }

        [HttpPost]
        public JsonResult SaveMrvCreation(FormCollection form, MATERIALRECEIPTMASTER objMrv)
        {
            List<string> listMrvJobCode = new List<string>();
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var jsonProductDetails = form["mrvProds"];
                    var serializer = new JavaScriptSerializer();
                    var lstMrvProducts = serializer.Deserialize<List<MRVREFERENCE>>(jsonProductDetails);
                    objMrv.MRVNO_MRV = CommonModelAccessUtility.GetCurrMrvCount(_unitOfWork);
                    objMrv.DOC_DATE_MRV = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                    objMrv.STATUS_MRV = "N";
                    _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Insert(objMrv);
                    _unitOfWork.Save();
                    listMrvJobCode.Add(objMrv.MRVNO_MRV);
                    SaveMrvReportObject(objMrv);
                    SaveMrvProducts(lstMrvProducts, objMrv, listMrvJobCode);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
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
                objSales.ValueAddedTax = Convert.ToDouble(sale.VAT_SD);
                objSales.CashAmount = (Convert.ToInt32(sale.QTY_SD) * Convert.ToDouble(sale.RATE_SD)) - Convert.ToDouble(sale.DISCOUNT_SD) + Convert.ToDouble(sale.SHIPPINGCHARGES_SD) + Convert.ToDouble(sale.VAT_SD);
                lstSales.Add(objSales);
            }

            return Json(new
            {
                lstSales
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCreditSaleDetailByMrv(string mrvid, string statusId)
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
                objSales.ValueAddedTax = Convert.ToDouble(sale.VAT_SD);
                objSales.CashAmount = (Convert.ToInt32(sale.QTY_SD) * Convert.ToDouble(sale.RATE_SD)) - Convert.ToDouble(sale.DISCOUNT_SD) + Convert.ToDouble(sale.SHIPPINGCHARGES_SD) + Convert.ToDouble(sale.VAT_SD);
                lstSales.Add(objSales);
            }

            return Json(new
            {
                lstSales
            }, JsonRequestBehavior.AllowGet);
        }
        [MesAuthorize("DailyTransactions")]
        public ActionResult MrvSearch()
        {
            return View();
        }
        public ActionResult FindMrvDetails()
        {
            return View();
        }
        public ActionResult SearchMrvDetails()
        {
            throw new NotImplementedException();
        }
        public ActionResult DuplicateMrvPrinting()
        {
            ViewBag.DocType = CommonModelAccessUtility.GetDocTypesForPrint(_unitOfWork);
            return View();
        }
        [HttpPost]
        public JsonResult SaveDuplicatePrinting(FormCollection frm)
        {
            string type = Convert.ToString(frm["DocType"]);
            string docNo = Convert.ToString(frm["DocNo"]);
            bool isInvalid = false;
            List<string> listMrvJobCode = new List<string>();
            var jsonData = new
            {
                type = type,
                docNo = docNo,
                listMrvJobCode = listMrvJobCode,
                isInvalid = isInvalid
            };
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    switch (type)
                    {
                        case "0":
                            isInvalid = Proc_MRVReportGenerater(docNo);
                            if (!isInvalid)
                            {
                                proc_JobCardGeneration(listMrvJobCode, docNo);
                            }
                            jsonData = new
                            {
                                type = type,
                                docNo = docNo,
                                listMrvJobCode = listMrvJobCode,
                                isInvalid = isInvalid
                            };
                            break;
                        case "1":
                            isInvalid = proc_DeleveryNoteNo(docNo);
                            jsonData = new
                            {
                                type = type,
                                docNo = docNo,
                                listMrvJobCode = listMrvJobCode,
                                isInvalid = isInvalid
                            };
                            break;
                        case "2":
                            isInvalid = proc_InvDataExist(docNo);
                            if (!isInvalid)
                            {
                                var dlNo = proc_GetDeleveryNoteNo(docNo);
                                proc_DeleveryNoteNo(dlNo);
                                proc_GenerateInvoice(docNo);
                            }
                            jsonData = new
                            {
                                type = type,
                                docNo = docNo,
                                listMrvJobCode = listMrvJobCode,
                                isInvalid = isInvalid
                            };
                            break;
                        case "3":
                            isInvalid = proc_CMDataExist(docNo);
                            if (!isInvalid)
                            {
                                proc_GenerateCashMemo(docNo);
                                var dlNo = proc_DeleveryNoteNoCashMemo(docNo);
                                proc_DeleveryNoteNo(dlNo);
                            }
                            jsonData = new
                            {
                                type = type,
                                docNo = docNo,
                                listMrvJobCode = listMrvJobCode,
                                isInvalid = isInvalid
                            };
                            break;
                        case "4":
                            //proc_JobCardReport(docNo);
                            jsonData = new
                            {
                                type = type,
                                docNo = docNo,
                                listMrvJobCode = listMrvJobCode,
                                isInvalid = false
                            };
                            break;
                        case "5":
                            jsonData = new
                            {
                                type = type,
                                docNo = docNo,
                                listMrvJobCode = listMrvJobCode,
                                isInvalid = false
                            };
                            break;
                        case "6":
                            jsonData = new
                            {
                                type = type,
                                docNo = docNo,
                                listMrvJobCode = listMrvJobCode,
                                isInvalid = false
                            };
                            break;
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                }
            }
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        private bool proc_CMDataExist(string docNo)
        {
            var cmData = (from sData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                          where sData.CASHRVNO_SD.Equals(docNo)
                          select sData).ToList();
            if (cmData.Count > 0)
            {
                return false;
            }
            return true;
        }
        private void proc_GenerateInvoice(string docNo)
        {
            try
            {
                _unitOfWork.Truncate("INVDETAILS");
                _unitOfWork.Truncate("INVMASTER");
                string mrvNo = "";
                string LPO = "";
                var procSData = (from sData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                 join pmData in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                                     on sData.PRCODE_SD equals pmData.PROD_CODE_PM
                                 where sData.INVNO_SD.Equals(docNo)
                                 select new { sData.PRCODE_SD, sData.MRVNO_SD, sData.QTY_SD, sData.RATE_SD, sData.UNIT_SD, pmData.DESCRIPTION_PM, sData.CREDITTOTAL_SD, sData.JOBNO_SD, sData.VAT_SD }).ToList();
                foreach (var item in procSData)
                {
                    var invDetails = _unitOfWork.Repository<INVDETAIL>().Create();
                    invDetails.CODE_INVD = item.PRCODE_SD;
                    invDetails.DESCRIPTION_INVD = item.DESCRIPTION_PM;
                    invDetails.QTY_INVD = item.QTY_SD;
                    invDetails.RATE_INVD = item.RATE_SD;
                    invDetails.AMOUNT_INVNO = item.CREDITTOTAL_SD - Convert.ToDecimal(item.VAT_SD);
                    invDetails.INVNO_INVD = docNo;
                    invDetails.JOBNO_INVD = item.JOBNO_SD;
                    invDetails.UNIT_INVD = item.UNIT_SD;
                    if (!string.IsNullOrEmpty(item.MRVNO_SD))
                    {
                        mrvNo = item.MRVNO_SD;
                    }
                    _unitOfWork.Repository<INVDETAIL>().Insert(invDetails);
                    _unitOfWork.Save();
                }
                var procJobIdRefData = (from sData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                        join jRefData in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                                            on sData.JOBID_SD equals jRefData.JOBID_JR
                                        where sData.INVNO_SD.Equals(docNo)
                                        select new { sData.JOBID_SD, sData.MRVNO_SD, sData.QTY_SD, sData.RATE_SD, sData.UNIT_SD, jRefData.JOBDESCRIPTION_JR, sData.CREDITTOTAL_SD, sData.JOBNO_SD, sData.VAT_SD }).ToList();
                foreach (var item in procJobIdRefData)
                {
                    var invDetails = _unitOfWork.Repository<INVDETAIL>().Create();
                    invDetails.CODE_INVD = item.JOBID_SD;
                    invDetails.DESCRIPTION_INVD = item.JOBDESCRIPTION_JR;
                    invDetails.QTY_INVD = item.QTY_SD;
                    invDetails.RATE_INVD = item.RATE_SD;
                    invDetails.AMOUNT_INVNO = item.CREDITTOTAL_SD - Convert.ToDecimal(item.VAT_SD);
                    invDetails.INVNO_INVD = docNo;
                    invDetails.JOBNO_INVD = item.JOBNO_SD;
                    invDetails.UNIT_INVD = item.UNIT_SD;
                    if (!string.IsNullOrEmpty(item.MRVNO_SD))
                    {
                        mrvNo = item.MRVNO_SD;
                    }
                    _unitOfWork.Repository<INVDETAIL>().Insert(invDetails);
                    _unitOfWork.Save();
                }

                var mrvData = (from mrv in _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                               where mrv.MRVNO_MRV.Equals(mrvNo)
                               select mrv).SingleOrDefault();
                if (mrvData != null)
                {
                    LPO = mrvData.NOTES_MRV;
                }

                var arapLedger = (from arLedger in _unitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                                  where arLedger.DOCNUMBER_ART.Equals(docNo)
                                  select arLedger).FirstOrDefault();
                if (arapLedger != null)
                {
                    var discountTotal = (from sData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                         where sData.INVNO_SD.Equals(docNo)
                                         select sData.DISCOUNT_SD).Sum();
                    var shippingTotal = (from sData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                         where sData.INVNO_SD.Equals(docNo)
                                         select sData.SHIPPINGCHARGES_SD).Sum();
                    var vatTotal = (from sData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                    where sData.INVNO_SD.Equals(docNo)
                                    select sData.VAT_SD).Sum();
                    var CustDetails = (from arMaster in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                                       where arMaster.ARCODE_ARM.Equals(arapLedger.ARAPCODE_ART)
                                       select arMaster).SingleOrDefault();
                    var invMasterData = _unitOfWork.Repository<INVMASTER>().Create();
                    invMasterData.INVNO_IPM = docNo;
                    invMasterData.INVDATE_IPM = arapLedger.GLDATE_ART;
                    invMasterData.CUST_CODE_IPM = CustDetails.ARCODE_ARM;
                    invMasterData.CUSTNAME_IPM = CustDetails.DESCRIPTION_ARM;
                    invMasterData.CUSTADDRESS_IPM = CustDetails.ADDRESS1_ARM;
                    invMasterData.CUSTPIN_IPM = CustDetails.POBOX_ARM;
                    invMasterData.SHIPPING_IPM = shippingTotal;
                    invMasterData.DISCOUNT_IPM = discountTotal;
                    invMasterData.VAT_IPM = vatTotal;
                    invMasterData.LPONO_IPM = LPO;
                    invMasterData.INVTYPE_IPM = "INV";
                    invMasterData.CUSTVATNO_IPM = "TRN: " + CommonModelAccessUtility.GetCustomerVAT(CustDetails.ARCODE_ARM, _unitOfWork);
                    _unitOfWork.Repository<INVMASTER>().Insert(invMasterData);
                    _unitOfWork.Save();
                }
            }
            catch (Exception)
            {

            }
        }
        private string proc_GetDeleveryNoteNo(string docNo)
        {
            string dlNo = "";
            var dlNoteData = (from sData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                              join jmData in _unitOfWork.Repository<JOBMASTER>().Query().Get()
                                  on sData.JOBNO_SD equals jmData.JOBNO_JM
                              where sData.INVNO_SD.Equals(docNo)
                              select new { jmData.DELEVERNOTENO_JM }).FirstOrDefault();
            if (dlNoteData != null)
            {
                dlNo = Convert.ToString(dlNoteData.DELEVERNOTENO_JM);
            }
            return dlNo;
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
        private bool Proc_MRVReportGenerater(string docNo)
        {
            try
            {
                _unitOfWork.Truncate("MRVNO_REPORT");
                _unitOfWork.Truncate("MRV_REPORT_CHD");
                var mrvData = (from mrvList in _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                               where mrvList.MRVNO_MRV.Equals(docNo)
                               select mrvList).SingleOrDefault();
                if (mrvData != null)
                {
                    var mrvReport = _unitOfWork.Repository<MRVNO_REPORT>().Create();
                    mrvReport.MRVNO_RPT = mrvData.MRVNO_MRV;
                    mrvReport.MRVDATE_RPT = mrvData.MRVDATE_MRV;
                    mrvReport.MRVDELEDATE_RPT = mrvData.DELE_DATE_MRV;
                    mrvReport.CUSTCODE_RPT = mrvData.CUSTOMERCODE_MRV;
                    mrvReport.CUSTNAME_RPT = mrvData.CUSTOMERNAME_MRV;
                    mrvReport.CUSTADD_RPT = mrvData.ADDRESS1_MRV;
                    _unitOfWork.Repository<MRVNO_REPORT>().Insert(mrvReport);
                    _unitOfWork.Save();

                    var mrvRefData = (from mrvRef in _unitOfWork.Repository<MRVREFERENCE>().Query().Get()
                                      where mrvRef.MRVNO_MRR.Equals(docNo)
                                      select mrvRef).ToList();
                    foreach (var refData in mrvRefData)
                    {
                        var mrvChild = _unitOfWork.Repository<MRV_REPORT_CHD>().Create();
                        mrvChild.MRVNO_CHD = refData.MRVNO_MRR;
                        mrvChild.PCODE_CHD = refData.PRODID_MRR;
                        mrvChild.JOBID_CHD = refData.JOBID_MRR;
                        mrvChild.QTY_CHD = refData.QTY_MRR;
                        mrvChild.RATE_CHD = refData.RATE_MRR;
                        mrvChild.AMOUNT = refData.AMOUNT_MRR;
                        _unitOfWork.Repository<MRV_REPORT_CHD>().Insert(mrvChild);
                        _unitOfWork.Save();
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
            }
            return true;
        }
        private void proc_JobCardGeneration(List<string> listMrvJobCode, string docNo)
        {
            _unitOfWork.Truncate("JOBCARD_REPRT");
            var jobCardData = (from jmData in _unitOfWork.Repository<JOBMASTER>().Query().Get()
                               join pmData in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                                   on jmData.PRODID_JIM equals pmData.PROD_CODE_PM
                               where jmData.MRVNO_JM.Equals(docNo)
                               select new
                               {
                                   jmData.JOBNO_JM,
                                   jmData.MRVNO_JM,
                                   jmData.DOCDATE_JM,
                                   jmData.JOBCODE_JM,
                                   jmData.PRODID_JIM,
                                   pmData.DESCRIPTION_PM
                               }).ToList();
            foreach (var item in jobCardData)
            {
                var rptJobCard = _unitOfWork.Repository<JOBCARD_REPRT>().Create();
                rptJobCard.JOBNO_JRP = item.JOBNO_JM;
                rptJobCard.MRVNO_JRP = item.MRVNO_JM;
                rptJobCard.JOBDATE_JRP = item.DOCDATE_JM;
                rptJobCard.PRODID_JRP = item.PRODID_JIM;
                rptJobCard.PRODUCTNAME_JRP = item.DESCRIPTION_PM;
                listMrvJobCode.Add(item.JOBNO_JM);
                _unitOfWork.Repository<JOBCARD_REPRT>().Insert(rptJobCard);
                _unitOfWork.Save();
            }
        }
        private bool proc_DeleveryNoteNo(string docNo)
        {
            bool success = false;
            try
            {
                _unitOfWork.Truncate("DELEVERYNOTE_RPT");
                _unitOfWork.Truncate("DELREPORT_MASTER");

                var jmDelRptM = (from jmData in _unitOfWork.Repository<JOBMASTER>().Query().Get()
                                 where jmData.DELEVERNOTENO_JM.Equals(docNo)
                                 group new { jmData.PRODID_JIM, jmData.PRODQTY_JM }
                                 by new { jmData.PRODID_JIM } into delData
                                 orderby delData.Key.PRODID_JIM
                                 select new { qtySum = delData.Sum(o => o.PRODQTY_JM), delData.Key.PRODID_JIM }).ToList();
                foreach (var item in jmDelRptM)
                {
                    var dlnRepM = _unitOfWork.Repository<DELREPORT_MASTER>().Create();
                    dlnRepM.PRODID_DLMASTER = item.PRODID_JIM;
                    dlnRepM.QTY_DLMASTER = item.qtySum;
                    _unitOfWork.Repository<DELREPORT_MASTER>().Insert(dlnRepM);
                    _unitOfWork.Save();
                }

                string jobNo = "";
                string serviceProduct = "";
                var jmDevData = (from jmData in _unitOfWork.Repository<JOBMASTER>().Query().Get()
                                 where jmData.DELEVERNOTENO_JM.Equals(docNo)
                                 select new { jmData.JOBNO_JM, jmData.PRODID_JIM }).ToList();
                foreach (var jnItem in jmDevData)
                {
                    jobNo = Convert.ToString(jnItem.JOBNO_JM);
                    serviceProduct = Convert.ToString(jnItem.PRODID_JIM);

                    var saleData = (from sData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                    join pm in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                                        on sData.PRCODE_SD equals pm.PROD_CODE_PM
                                    where sData.JOBNO_SD.Equals(jobNo)
                                    select new { sData.PRCODE_SD, pm.DESCRIPTION_PM, sData.QTY_SD }).ToList();
                    foreach (var item in saleData)
                    {
                        var dlnRep = _unitOfWork.Repository<DELEVERYNOTE_RPT>().Create();
                        dlnRep.DLNR_DLNRPT = docNo;
                        dlnRep.ID_DLNRPT = item.PRCODE_SD;
                        dlnRep.DESCRIPTION_DLNRPT = item.DESCRIPTION_PM;
                        dlnRep.QTY_DLNRPT = item.QTY_SD;
                        dlnRep.JOBNO_DLNRPT = jobNo;
                        dlnRep.SERVICEPROID_DLNRPT = serviceProduct;
                        dlnRep.DLNTYPE_DLNRPT = "CM";
                        _unitOfWork.Repository<DELEVERYNOTE_RPT>().Insert(dlnRep);
                        _unitOfWork.Save();
                    }

                    var jIDRefData = (from sData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                      join jidRef in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                                          on sData.JOBID_SD equals jidRef.JOBID_JR
                                      where sData.JOBNO_SD.Equals(jobNo)
                                      select new { sData.JOBID_SD, jidRef.JOBDESCRIPTION_JR, sData.QTY_SD }).ToList();
                    foreach (var item in jIDRefData)
                    {
                        var dlnRep = _unitOfWork.Repository<DELEVERYNOTE_RPT>().Create();
                        dlnRep.DLNR_DLNRPT = docNo;
                        dlnRep.ID_DLNRPT = item.JOBID_SD;
                        dlnRep.DESCRIPTION_DLNRPT = item.JOBDESCRIPTION_JR;
                        dlnRep.QTY_DLNRPT = item.QTY_SD;
                        dlnRep.JOBNO_DLNRPT = jobNo;
                        dlnRep.SERVICEPROID_DLNRPT = serviceProduct;
                        dlnRep.DLNTYPE_DLNRPT = "CM";
                        _unitOfWork.Repository<DELEVERYNOTE_RPT>().Insert(dlnRep);
                        _unitOfWork.Save();
                    }
                    success = true;
                }
                success = false;
            }
            catch (Exception)
            {
                success = false;
            }
            return success;
        }
        private bool proc_InvDataExist(string docNo)
        {
            var invData = (from sData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                           where sData.INVNO_SD.Equals(docNo)
                           select sData).ToList();
            if (invData.Count > 0)
            {
                return false;
            }
            return true;
        }
        private void proc_GenerateCashMemo(string docNo)
        {
            try
            {
                _unitOfWork.Truncate("INVDETAILS");
                _unitOfWork.Truncate("INVMASTER");
                string mrvNo = "";
                string LPO = "";
                string custCode = "";
                string custName = "";
                string custAddress = "";
                var procSData = (from sData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                 join pmData in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                                     on sData.PRCODE_SD equals pmData.PROD_CODE_PM
                                 where sData.CASHRVNO_SD.Equals(docNo)
                                 select new { sData.PRCODE_SD, sData.MRVNO_SD, sData.QTY_SD, sData.RATE_SD, sData.UNIT_SD, pmData.DESCRIPTION_PM, sData.CASHTOTAL_SD, sData.JOBNO_SD, sData.VAT_SD }).ToList();
                foreach (var item in procSData)
                {
                    var invDetails = _unitOfWork.Repository<INVDETAIL>().Create();
                    invDetails.CODE_INVD = item.PRCODE_SD;
                    invDetails.DESCRIPTION_INVD = item.DESCRIPTION_PM;
                    invDetails.QTY_INVD = item.QTY_SD;
                    invDetails.RATE_INVD = item.RATE_SD;
                    invDetails.AMOUNT_INVNO = item.CASHTOTAL_SD - Convert.ToDecimal(item.VAT_SD);
                    invDetails.INVNO_INVD = docNo;
                    invDetails.JOBNO_INVD = item.JOBNO_SD;
                    invDetails.UNIT_INVD = item.UNIT_SD;
                    if (!string.IsNullOrEmpty(item.MRVNO_SD))
                    {
                        mrvNo = item.MRVNO_SD;
                    }
                    _unitOfWork.Repository<INVDETAIL>().Insert(invDetails);
                    _unitOfWork.Save();
                }
                var procJobIdRefData = (from sData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                        join jRefData in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                                            on sData.JOBID_SD equals jRefData.JOBID_JR
                                        where sData.CASHRVNO_SD.Equals(docNo)
                                        select new { sData.JOBID_SD, sData.MRVNO_SD, sData.QTY_SD, sData.RATE_SD, sData.UNIT_SD, jRefData.JOBDESCRIPTION_JR, sData.CASHTOTAL_SD, sData.JOBNO_SD, sData.VAT_SD }).ToList();
                foreach (var item in procJobIdRefData)
                {
                    var invDetails = _unitOfWork.Repository<INVDETAIL>().Create();
                    invDetails.CODE_INVD = item.JOBID_SD;
                    invDetails.DESCRIPTION_INVD = item.JOBDESCRIPTION_JR;
                    invDetails.QTY_INVD = item.QTY_SD;
                    invDetails.RATE_INVD = item.RATE_SD;
                    invDetails.AMOUNT_INVNO = item.CASHTOTAL_SD - Convert.ToDecimal(item.VAT_SD);
                    invDetails.INVNO_INVD = docNo;
                    invDetails.JOBNO_INVD = item.JOBNO_SD;
                    invDetails.UNIT_INVD = item.UNIT_SD;
                    if (!string.IsNullOrEmpty(item.MRVNO_SD))
                    {
                        mrvNo = item.MRVNO_SD;
                    }
                    _unitOfWork.Repository<INVDETAIL>().Insert(invDetails);
                    _unitOfWork.Save();
                }

                var mrvData = (from mrv in _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                               where mrv.MRVNO_MRV.Equals(mrvNo)
                               select mrv).SingleOrDefault();
                if (mrvData != null)
                {
                    LPO = mrvData.NOTES_MRV;
                    custCode = mrvData.CUSTOMERCODE_MRV;
                    custName = mrvData.CUSTOMERNAME_MRV;
                    custAddress = mrvData.ADDRESS1_MRV;
                }

                var bnkTrans = (from btData in _unitOfWork.Repository<BANKTRANSACTION>().Query().Get()
                                where btData.DOCNUMBER_BT.Equals(docNo)
                                select btData).FirstOrDefault();
                if (bnkTrans != null)
                {
                    var discountTotal = (from sData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                         where sData.CASHRVNO_SD.Equals(docNo)
                                         select sData.DISCOUNT_SD).Sum();
                    var shippingTotal = (from sData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                         where sData.CASHRVNO_SD.Equals(docNo)
                                         select sData.SHIPPINGCHARGES_SD).Sum();
                    var vatTotal = (from sData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                    where sData.CASHRVNO_SD.Equals(docNo)
                                    select sData.VAT_SD).Sum();
                    var invMasterData = _unitOfWork.Repository<INVMASTER>().Create();
                    invMasterData.INVNO_IPM = docNo;
                    invMasterData.INVDATE_IPM = bnkTrans.GLDATE_BT;
                    invMasterData.CUST_CODE_IPM = custCode;
                    invMasterData.CUSTNAME_IPM = custName;
                    invMasterData.CUSTADDRESS_IPM = custAddress;
                    invMasterData.SHIPPING_IPM = shippingTotal;
                    invMasterData.DISCOUNT_IPM = discountTotal;
                    invMasterData.VAT_IPM = vatTotal;
                    invMasterData.LPONO_IPM = LPO;
                    invMasterData.INVTYPE_IPM = "CM";
                    _unitOfWork.Repository<INVMASTER>().Insert(invMasterData);
                    _unitOfWork.Save();
                }
            }
            catch (Exception)
            {

            }
        }
        private string proc_DeleveryNoteNoCashMemo(string docNo)
        {
            string dlNo = "";
            var dlNoteData = (from sData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                              join jmData in _unitOfWork.Repository<JOBMASTER>().Query().Get()
                                  on sData.JOBNO_SD equals jmData.JOBNO_JM
                              where sData.CASHRVNO_SD.Equals(docNo)
                              select new { jmData.DELEVERNOTENO_JM }).FirstOrDefault();
            if (dlNoteData != null)
            {
                dlNo = Convert.ToString(dlNoteData.DELEVERNOTENO_JM);
            }
            return dlNo;
        }
        private bool proc_JobCardReport(string docNo)
        {
            try
            {
                _unitOfWork.Truncate("JOB_REPORT");
                var procSData = (from sData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                 join pmData in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
                                     on sData.PRCODE_SD equals pmData.PROD_CODE_PM
                                 where sData.JOBNO_SD.Equals(docNo)
                                 select new { sData.PRCODE_SD, pmData.DESCRIPTION_PM, sData.QTY_SD, sData.UNIT_SD, sData.RATE_SD, sData.DISCOUNT_SD, sData.SHIPPINGCHARGES_SD }).ToList();
                foreach (var item in procSData)
                {
                    var jobRpt = _unitOfWork.Repository<JOB_REPORT>().Create();
                    jobRpt.JOBNO_JR = docNo;
                    jobRpt.DESCRIPTION_JR = item.DESCRIPTION_PM;
                    jobRpt.QTY_JR = item.QTY_SD;
                    jobRpt.RATE_JR = item.RATE_SD;
                    jobRpt.DISCOUNT_JR = item.DISCOUNT_SD;
                    jobRpt.SHIPPINGCHARGES_JR = item.SHIPPINGCHARGES_SD;
                    jobRpt.UNIT_JR = item.UNIT_SD;
                    _unitOfWork.Repository<JOB_REPORT>().Insert(jobRpt);
                    _unitOfWork.Save();
                }
                var procJobIdRefData = (from sData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                                        join jRefData in _unitOfWork.Repository<JOBIDREFERENCE>().Query().Get()
                                            on sData.JOBID_SD equals jRefData.JOBID_JR
                                        where sData.JOBNO_SD.Equals(docNo)
                                        select new { sData.JOBID_SD, jRefData.JOBDESCRIPTION_JR, sData.QTY_SD, sData.RATE_SD, sData.DISCOUNT_SD, sData.SHIPPINGCHARGES_SD }).ToList();
                foreach (var item in procJobIdRefData)
                {
                    var jobRpt = _unitOfWork.Repository<JOB_REPORT>().Create();
                    jobRpt.JOBNO_JR = docNo;
                    jobRpt.DESCRIPTION_JR = item.JOBDESCRIPTION_JR;
                    jobRpt.QTY_JR = item.QTY_SD;
                    jobRpt.RATE_JR = item.RATE_SD;
                    jobRpt.DISCOUNT_JR = item.DISCOUNT_SD;
                    jobRpt.SHIPPINGCHARGES_JR = item.SHIPPINGCHARGES_SD;
                    _unitOfWork.Repository<JOB_REPORT>().Insert(jobRpt);
                    _unitOfWork.Save();
                }
            }
            catch (Exception)
            {

            }
            return false;
        }
        [HttpPost]
        public JsonResult MRVSearchDetails(string mrvNo)
        {
            MRVSerachDetails searchHeader = proc_DisplayJobDetails(mrvNo);
            List<MRVSearchDetailsResult> searchResult = fn_SearchJobDetails(mrvNo);
            var jsonData = new
            {
                searchHeader = searchHeader,
                searchResult = searchResult
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        private MRVSerachDetails proc_DisplayJobDetails(string MrvNo)
        {
            MRVSerachDetails searchDetails = new MRVSerachDetails();
            if (!string.IsNullOrEmpty(MrvNo))
            {
                var MrvDetails = (from mrvData in _unitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                                  where mrvData.MRVNO_MRV.Equals(MrvNo)
                                  select mrvData).SingleOrDefault();
                if (MrvDetails != null)
                {
                    searchDetails.MrvNumber = MrvDetails.MRVNO_MRV;
                    searchDetails.CustomerCode = MrvDetails.CUSTOMERCODE_MRV;
                    searchDetails.CustomerName = MrvDetails.CUSTOMERNAME_MRV;
                    searchDetails.Address = MrvDetails.ADDRESS1_MRV;
                    searchDetails.Telephone = MrvDetails.PHONE_MRV;
                    searchDetails.JobNumber = "";
                    searchDetails.Employee = "";
                }
                else
                {
                    return null;
                }
            }
            return searchDetails;
        }
        private List<MRVSearchDetailsResult> fn_SearchJobDetails(string MrvNo)
        {
            List<MRVSearchDetailsResult> saleSearchResult = new List<MRVSearchDetailsResult>();
            if (!string.IsNullOrEmpty(MrvNo))
            {
                var sales = (from saleData in _unitOfWork.Repository<SALEDETAIL>().Query().Get()
                             where saleData.MRVNO_SD.Equals(MrvNo)
                             select saleData).ToList();
                foreach (var sale in sales)
                {
                    MRVSearchDetailsResult saleData = new MRVSearchDetailsResult();
                    saleData.JOBNO_SD = sale.JOBNO_SD;
                    saleData.PRCODE_SD = sale.PRCODE_SD;
                    saleData.JOBID_SD = sale.JOBID_SD;
                    saleData.QTY_SD = Convert.ToInt32(sale.QTY_SD);
                    saleData.RATE_SD = Convert.ToDecimal(sale.RATE_SD);
                    saleData.Amount = Convert.ToInt32(sale.QTY_SD) * Convert.ToDecimal(sale.RATE_SD);
                    saleData.DISCOUNT_SD = Convert.ToDecimal(sale.DISCOUNT_SD);
                    saleData.SHIPPINGCHARGES_SD = Convert.ToDecimal(sale.SHIPPINGCHARGES_SD);
                    saleData.SALEDATE_SD = Convert.ToDateTime(sale.SALEDATE_SD);
                    saleData.USERID_SD = sale.USERID_SD;
                    saleData.CASHTOTAL_SD = Convert.ToDecimal(sale.CASHTOTAL_SD);
                    saleData.CREDITTOTAL_SD = Convert.ToDecimal(sale.CREDITTOTAL_SD);
                    saleData.CASHRVNO_SD = sale.CASHRVNO_SD;
                    saleData.INVNO_SD = sale.INVNO_SD;
                    saleData.CREDITACCODE_SD = sale.CREDITACCODE_SD;
                    saleData.LPONO_SD = sale.LPONO_SD;
                    saleData.DAYENDDOC_NO = sale.DAYENDDOC_NO;
                    saleSearchResult.Add(saleData);
                }
            }
            else
            {
                return saleSearchResult;
            }
            return saleSearchResult;
        }
    }
}