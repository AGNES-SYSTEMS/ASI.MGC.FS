using System;
using System.Linq;
using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;
using System.Data.Entity;
using ASI.MGC.FS.Domain.Repositories;

namespace ASI.MGC.FS.Controllers
{
    public class MgcReportsController : Controller
    {
        readonly IUnitOfWork _unitOfWork;
        readonly TimeZoneInfo timeZoneInfo;
        public MgcReportsController()
        {
            _unitOfWork = new UnitOfWork();
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time");
        }
        public ActionResult ArStatement()
        {
            return View();
        }
        public ActionResult ApStatement()
        {
            return View();
        }
        public ActionResult ApStatementOutstanding()
        {
            return View();
        }
        public ActionResult ArStatementOutstanding()
        {
            return View();
        }
        public ActionResult ArSummary()
        {
            return View();
        }
        public ActionResult BalanceSheet()
        {
            return View();
        }
        public ActionResult BankPayment(string bpNo = "")
        {
            ViewBag.bpNo = bpNo;
            return View();
        }
        public ActionResult BankReceipt(string brNo = "")
        {
            ViewBag.brNo = brNo;
            return View();
        }
        public ActionResult BankStatement()
        {
            return View();
        }
        public ActionResult CancelledJobs()
        {
            return View();
        }
        public ActionResult CashMemo(string cmNo = "")
        {
            ViewBag.cmNo = cmNo;
            return View();
        }
        public ActionResult DuplicateCashMemo(string cmNo = "")
        {
            ViewBag.cmNo = cmNo;
            return View();
        }
        public ActionResult CashMemoReverse()
        {
            return View();
        }
        public ActionResult CashMemoWiseReport()
        {
            return View();
        }
        public ActionResult CashPayment(string cpNo = "")
        {
            ViewBag.cpNo = cpNo;
            return View();
        }
        public ActionResult CashReceipt(string crNo = "")
        {
            ViewBag.crNo = crNo;
            return View();
        }
        public ActionResult DeliveredMrvDetails()
        {
            return View();
        }
        public ActionResult DeliveryDetails()
        {
            return View();
        }
        public ActionResult DeliveryNote()
        {
            return View();
        }
        public ActionResult DeliveryNoteDetails()
        {
            return View();
        }
        public ActionResult EmpSales()
        {
            return View();
        }
        public ActionResult EmpWiseJobDetails()
        {
            return View();
        }
        public ActionResult GlStatement()
        {
            return View();
        }
        public ActionResult Invoice(string invNo = "")
        {
            ViewBag.invNo = invNo;
            return View();
        }
        public ActionResult DuplicateInvoice(string invNo = "")
        {
            ViewBag.invNo = invNo;
            return View();
        }
        public ActionResult InvoiceWiseReport()
        {
            return View();
        }
        public ActionResult JobCardDetails(string jobNo = "")
        {
            ViewBag.jobNo = jobNo;
            return View();
        }
        public ActionResult JobCardFormat(string jobNo = "")
        {
            ViewBag.jobNo = jobNo;
            return View();
        }
        public ActionResult JobWiseStatement()
        {
            return View();
        }
        public ActionResult JournalVoucher(string jvNo = "")
        {
            ViewBag.jvNo = jvNo;
            return View();
        }
        public ActionResult MetarialReceiptVoucher(string mrvNo = "")
        {
            ViewBag.mrvNo = mrvNo;
            return View();
        }
        public ActionResult MrvStatement()
        {
            return View();
        }
        public ActionResult PendingMrvDetails()
        {
            return View();
        }
        public ActionResult ProfitLossStatement()
        {
            return View();
        }
        [HttpPost]
        public JsonResult FetchPLData(FormCollection frm)
        {
            bool success = false;
            ReportRepository repo = _unitOfWork.ExtRepositoryFor<ReportRepository>();
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    if (!string.IsNullOrEmpty(frm["startDate"]) && !string.IsNullOrEmpty(frm["endDate"]))
                    {
                        var startDate = Convert.ToDateTime(frm["startDate"]);
                        var endDate = Convert.ToDateTime(frm["endDate"]);
                        if (endDate > startDate)
                        {
                            decimal openingBalance = default(decimal);
                            decimal uptoYear = default(decimal);

                            //decimal debitAmount = default(decimal);
                            //decimal creditAmount = default(decimal);
                            //decimal netAmount = default(decimal);
                            string plAccType = string.Empty;
                            //short i = 0;
                            //decimal qty = default(decimal);
                            //decimal rate = default(decimal);
                            //decimal stockAmount = default(decimal);

                            //decimal curMonth = default(decimal);
                            //decimal curYear = default(decimal);
                            DateTime yearStart = Convert.ToDateTime("01-jan-" + endDate.Year);
                            string plCode = string.Empty;
                            _unitOfWork.Truncate("PROFITANDLOSS_RPT");
                            var glMaster = (from glm in _unitOfWork.Repository<GLMASTER>().Query().Get()
                                            select new { glm.GLCODE_LM, glm.GLDESCRIPTION_LM }).ToList();
                            var filteredGlMaster = glMaster.ToList().Where(glm => Convert.ToInt32(glm.GLCODE_LM) >= 3000 && Convert.ToInt32(glm.GLCODE_LM) <= 4999);

                            foreach (var item in filteredGlMaster)
                            {
                                var glCode = Convert.ToInt32(item.GLCODE_LM);
                                if (glCode >= 3000 & glCode < 3500)
                                {
                                    plCode = "B";
                                    plAccType = "Sales";
                                }
                                if (glCode >= 3500 & glCode < 4000)
                                {
                                    plCode = "D";
                                    plAccType = "Other Income";
                                }
                                if (glCode >= 4000 & glCode < 4500)
                                {
                                    plCode = "A";
                                    plAccType = "Cost of Sales";
                                }
                                if (glCode >= 4500 & glCode < 5000)
                                {
                                    plCode = "C";
                                    plAccType = "Other Expense";
                                }
                                openingBalance = 0;

                                var sumUptoYear = (from glt in _unitOfWork.Repository<GLTRANSACTION1>().Query().Get()
                                                   where glt.GLACCODE_GLT.Equals(item.GLCODE_LM) &&
                                                   (glt.GLDATE_GLT >= yearStart && glt.GLDATE_GLT <= endDate)
                                                   select (glt.DEBITAMOUNT_GLT - glt.CREDITAMOUNT_GLT)).Sum();
                                uptoYear = sumUptoYear != null ? Convert.ToDecimal(sumUptoYear) : 0;
                                var sumUptoSelectedDate = (from glt in _unitOfWork.Repository<GLTRANSACTION1>().Query().Get()
                                                           where glt.GLACCODE_GLT.Equals(item.GLCODE_LM) &&
                                                           (glt.GLDATE_GLT >= startDate && glt.GLDATE_GLT <= endDate)
                                                           select (glt.DEBITAMOUNT_GLT - glt.CREDITAMOUNT_GLT)).Sum();
                                openingBalance = sumUptoYear != null ? Convert.ToDecimal(sumUptoYear) : 0;
                                var rptPLStatementObj = _unitOfWork.Repository<PROFITANDLOSS_RPT>().Create();
                                rptPLStatementObj.ACCOUNTCODE_PL = plAccType;
                                rptPLStatementObj.DESCRIPTION_PL = item.GLCODE_LM + " " + item.GLDESCRIPTION_LM;
                                rptPLStatementObj.CURMONTH_PL = openingBalance;
                                rptPLStatementObj.CURRENT_YEARTODATE = uptoYear;
                                rptPLStatementObj.GROUPCODE = plCode;
                                _unitOfWork.Repository<PROFITANDLOSS_RPT>().Insert(rptPLStatementObj);
                                _unitOfWork.Save();
                            }
                            openingBalance = repo.sp_GetStockValuation(startDate);
                            uptoYear = repo.sp_GetStockValuation(yearStart);

                            //PROFITANDLOSS_RPT rptPLStatementOpn = null;
                            var rptPLStatementOpn = _unitOfWork.Repository<PROFITANDLOSS_RPT>().Create();
                            rptPLStatementOpn.ACCOUNTCODE_PL = plAccType;
                            rptPLStatementOpn.DESCRIPTION_PL = "Opening Stock";
                            rptPLStatementOpn.CURMONTH_PL = openingBalance;
                            rptPLStatementOpn.CURRENT_YEARTODATE = uptoYear;
                            rptPLStatementOpn.GROUPCODE = "A";
                            _unitOfWork.Repository<PROFITANDLOSS_RPT>().Insert(rptPLStatementOpn);
                            _unitOfWork.Save();

                            var rptPLStatementCls = _unitOfWork.Repository<PROFITANDLOSS_RPT>().Create();
                            rptPLStatementCls.ACCOUNTCODE_PL = "Sales";
                            rptPLStatementCls.DESCRIPTION_PL = "Closing Stock";
                            rptPLStatementCls.CURMONTH_PL = openingBalance;
                            rptPLStatementCls.CURRENT_YEARTODATE = uptoYear;
                            rptPLStatementCls.GROUPCODE = "B";
                            _unitOfWork.Repository<PROFITANDLOSS_RPT>().Insert(rptPLStatementCls);
                            _unitOfWork.Save();
                        }
                    }
                    success = true;
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PurchaseRegister()
        {
            return View();
        }
        public ActionResult Quotation(string quotNo = "")
        {
            ViewBag.quotNo = quotNo;
            return View();
        }
        public ActionResult ReversedInvoice()
        {
            return View();
        }
        public ActionResult SalesInvoice()
        {
            return View();
        }
        public ActionResult ServiceItems()
        {
            return View();
        }
        public ActionResult StockJournal()
        {
            return View();
        }
        public ActionResult StockLedgerReport()
        {
            return View();
        }
        public ActionResult StockReceipt(string voucherNo = "")
        {
            ViewBag.voucherNo = voucherNo;
            return View();
        }
        public ActionResult StockReport()
        {
            return View();
        }
        public ActionResult TrialBalance()
        {
            return View();
        }

        private string GetGlMasterCode(string glCode)
        {
            var glDetails = (from genralMaster in _unitOfWork.Repository<GLMASTER>().Query().Get()
                             where genralMaster.GLCODE_LM.Equals(glCode)
                             select genralMaster);
            return glDetails.ToList()[0].GLDESCRIPTION_LM;
        }

        private AR_AP_MASTER GetArApMasterDeatils(string arApCode)
        {
            var arApMaster = (from arApMasterList in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                              where arApMasterList.ARCODE_ARM.Equals(arApCode)
                              select arApMasterList).ToList();
            return arApMaster[0];
        }

        [HttpPost]
        public JsonResult GenerateBalanceSheet(FormCollection frm)
        {
            DateTime SDate = Convert.ToDateTime(frm["StartDate"]);
            DateTime FDate = Convert.ToDateTime(frm["EndDate"]);
            Proc_ProfitandLossGenerate(SDate, FDate);
            Proc_BalanseSheet(SDate, FDate);
            //Proc_BalanceSheet(SDate, FDate);
            //throw new NotImplementedException();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //stock calculate
        //private decimal fn_StockValution(DateTime SDate)
        //{
        //    decimal functionReturnValue = default(decimal);
        //    string PrCode = null;
        //    string PrName = null;
        //    double PrQty = 0;
        //    decimal RATE_Renamed = default(decimal);
        //    double totalqty = 0;
        //    string Unit = null;
        //    string Cunit = null;
        //    DateTime CurDate = DateTime.Now;
        //    double aqty = 0;
        //    DateTime GLDAte = DateTime.Now;
        //    //short No = 0;
        //    //string Particulars = null;
        //    //double ReceiptQty = 0;
        //    //decimal ReceiptRate = default(decimal);
        //    //double IssueQty = 0;
        //    //decimal IssueRate = default(decimal);
        //    //string ProId = null;
        //    //string ProdDescription = null;
        //    //string VarUnit = null;
        //    ReportRepository repo = _unitOfWork.ExtRepositoryFor<ReportRepository>();
        //    double ConvertedQty = default(double);
        //    string ConvertedUnit = "";
        //    decimal ConvertedRate = default(decimal);
        //    var prdMasterRecords = (from productMaster in _unitOfWork.Repository<PRODUCTMASTER>().Query().Get()
        //                            where productMaster.STATUS_PM.Equals("IP")
        //                            select productMaster).ToList();
        //    foreach (var record in prdMasterRecords)
        //    {
        //        //No = 1;
        //        PrCode = record.PROD_CODE_PM;
        //        PrName = record.DESCRIPTION_PM;
        //        totalqty = 0;
        //        PrQty = 0;
        //        Unit = "";
        //        PrName = "";
        //        totalqty = 0;

        //        //var stockLedgerSum = (from stockLedger in _unitOfWork.Repository<STOCKLEDGER>().Query().Get()
        //        //                      where stockLedger.PRODID_SL.Trim() == PrCode.Trim() &&
        //        //                      (DbFunctions.TruncateTime(stockLedger.LEDGER_DATE_SL) < SDate.Date)
        //        //                      select stockLedger).AsEnumerable().Select(c => new { c.UNIT_SL, c.ISSUE_QTY_SL, c.RECEPT_QTY_SL })
        //        //                      .GroupBy(c => new { c.UNIT_SL })
        //        //                      .Select(g => new { unit = g.Key.UNIT_SL, SumQtyTotal = g.Sum(x => (x.RECEPT_QTY_SL != null ? x.RECEPT_QTY_SL : 0) - (x.ISSUE_QTY_SL != null ? x.ISSUE_QTY_SL : 0)) })
        //        //                      .ToList();
        //        var stockLedgerSum = repo.sp_FinancialTables(6, PrCode, DateTime.Now, DateTime.Now);
        //        ConvertedRate = 0;
        //        ConvertedUnit = "";
        //        ConvertedQty = 0;
        //        foreach (var stkSumRecord in stockLedgerSum)
        //        {
        //            PrQty = stkSumRecord.Sum1 != null ? Convert.ToDouble(stkSumRecord.Sum1) : 0;
        //            Unit = stkSumRecord.Unit != null ? stkSumRecord.Unit : "";

        //            WebCommon.CommonModelAccessUtility.Proc_ConverUnit(Unit.Trim(), PrQty, 0, out ConvertedQty, out ConvertedUnit, out ConvertedRate, _unitOfWork);

        //            PrQty = ConvertedQty;
        //            Unit = ConvertedUnit;
        //            CurDate = Convert.ToDateTime(SDate);
        //            totalqty = totalqty + PrQty;
        //        }
        //        RATE_Renamed = repo.sp_StockLedger_Converstion(PrCode.Trim(), SDate);

        //        //var stockLedgerRecords = (from stockLedger in _unitOfWork.Repository<STOCKLEDGER>().Query().Get()
        //        //                          where stockLedger.PRODID_SL.Trim() == PrCode.Trim() &&
        //        //                          (DbFunctions.TruncateTime(stockLedger.LEDGER_DATE_SL) < SDate.Date) && stockLedger.RECEPT_RATE_SL > 0
        //        //                          select stockLedger).ToList();
        //        //RATE_Renamed = 0;
        //        //ConvertedRate = 0;
        //        //ConvertedUnit = "";
        //        //ConvertedQty = 0;
        //        //foreach (var stkRecord in stockLedgerRecords)
        //        //{
        //        //    RATE_Renamed = stkRecord.RECEPT_RATE_SL != null ? Convert.ToDecimal(stkRecord.RECEPT_RATE_SL) : 0;
        //        //    Cunit = stkRecord.UNIT_SL != null ? stkRecord.UNIT_SL : "";
        //        //    aqty = stkRecord.RECEPT_QTY_SL != null ? Convert.ToDouble(stkRecord.RECEPT_QTY_SL) : 0;
        //        //    WebCommon.CommonModelAccessUtility.Proc_ConverUnit(Cunit.Trim(), aqty, RATE_Renamed, out ConvertedQty, out ConvertedUnit, out ConvertedRate, _unitOfWork);
        //        //    RATE_Renamed = ConvertedRate;
        //        //}

        //        functionReturnValue = functionReturnValue + Convert.ToDecimal(totalqty) * RATE_Renamed;
        //    }

        //    return functionReturnValue;
        //}
        public void Proc_ProfitandLossGenerate(DateTime SDate, DateTime FDate)
        {
            string ACCode = null;
            string ACdescription = null;

            decimal Opening = default(decimal);
            decimal UptoYear = default(decimal);

            //decimal DebitAmount = default(decimal);
            //decimal CreditAmount = default(decimal);
            //decimal Net = default(decimal);
            string ACType = null;
            //short i = 0;
            //short c = 0;
            //decimal QTY = default(decimal);
            //decimal RATE_Renamed = default(decimal);
            //decimal stockAmount = default(decimal);

            //decimal CurMonth = default(decimal);
            //decimal CurYear = default(decimal);
            string s = null;
            System.DateTime yearStart = default(System.DateTime);

            string Code = null;
            s = "01-jan-" + FDate.Year;
            yearStart = Convert.ToDateTime(s);
            _unitOfWork.Truncate("PROFITANDLOSS_RPT");
            //using (var transaction = _unitOfWork.BeginTransaction())
            //{
            try
            {

                ReportRepository repo = _unitOfWork.ExtRepositoryFor<ReportRepository>();
                var glMasterRecords = repo.sp_TransactionQueries(1);
                foreach (var record in glMasterRecords)
                {
                    ACCode = "";
                    ACdescription = "";
                    ACCode = Convert.ToString(record.GLCODE_LM);
                    int glCode = Convert.ToInt32(record.GLCODE_LM);
                    ACdescription = record.GLDESCRIPTION_LM;
                    if (glCode >= 3000 & glCode < 3500)
                    {
                        Code = "B";
                        ACType = "Sales";
                    }
                    if (glCode >= 3500 & glCode < 4000)
                    {
                        Code = "D";
                        ACType = "Other Income";
                    }
                    if (glCode >= 4000 & glCode < 4500)
                    {
                        Code = "A";
                        ACType = "Cost of Sales";
                    }
                    if (glCode >= 4500 & glCode < 5000)
                    {
                        Code = "C";
                        ACType = "Other Expense";
                    }
                    Opening = 0;
                    UptoYear = 0;
                    var lstUptoYear = (from glTransaction in _unitOfWork.Repository<GLTRANSACTION1>().Query().Get()
                                       where glTransaction.GLACCODE_GLT.Trim() == ACCode &&
                                       (DbFunctions.TruncateTime(glTransaction.GLDATE_GLT) >= yearStart.Date && DbFunctions.TruncateTime(glTransaction.GLDATE_GLT) <= FDate.Date)
                                       group glTransaction by glTransaction.GLACCODE_GLT into glTransactionG
                                       select new { SumTotal = glTransactionG.Sum(x => (x.DEBITAMOUNT_GLT != null ? x.DEBITAMOUNT_GLT : 0) - (x.CREDITAMOUNT_GLT != null ? x.CREDITAMOUNT_GLT : 0)) }).ToList();

                    var lstOpening = (from glTransaction in _unitOfWork.Repository<GLTRANSACTION1>().Query().Get()
                                      where glTransaction.GLACCODE_GLT.Trim() == ACCode &&
                                      (DbFunctions.TruncateTime(glTransaction.GLDATE_GLT) >= SDate.Date && DbFunctions.TruncateTime(glTransaction.GLDATE_GLT) <= FDate.Date)
                                      group glTransaction by glTransaction.GLACCODE_GLT into glTransactionG
                                      select new { SumTotal = glTransactionG.Sum(x => (x.DEBITAMOUNT_GLT != null ? x.DEBITAMOUNT_GLT : 0) - (x.CREDITAMOUNT_GLT != null ? x.CREDITAMOUNT_GLT : 0)) }).ToList();
                    UptoYear = lstUptoYear != null && lstUptoYear.Count > 0 ? Convert.ToDecimal(lstUptoYear[0].SumTotal) : 0;
                    Opening = lstOpening != null && lstOpening.Count > 0 ? Convert.ToDecimal(lstOpening[0].SumTotal) : 0;




                    var objPLRpt = _unitOfWork.Repository<PROFITANDLOSS_RPT>().Create();
                    objPLRpt.ACCOUNTCODE_PL = ACType.Trim();
                    objPLRpt.DESCRIPTION_PL = ACCode + " " + ACdescription.Trim(); ;
                    objPLRpt.CURMONTH_PL = Opening;
                    objPLRpt.CURRENT_YEARTODATE = UptoYear;
                    objPLRpt.GROUPCODE = Code;
                    _unitOfWork.Repository<PROFITANDLOSS_RPT>().Insert(objPLRpt);
                    _unitOfWork.Save();
                }

                //READ Opening STOCK

                ACdescription = "Opening Stock";
                Opening = repo.sp_GetStockValuation(SDate);
                UptoYear = repo.sp_GetStockValuation(yearStart);
                var objOpenStockPLRpt = _unitOfWork.Repository<PROFITANDLOSS_RPT>().Create();
                objOpenStockPLRpt.ACCOUNTCODE_PL = ACType.Trim();
                objOpenStockPLRpt.DESCRIPTION_PL = ACdescription.Trim(); ;
                objOpenStockPLRpt.CURMONTH_PL = Opening;
                objOpenStockPLRpt.CURRENT_YEARTODATE = UptoYear;
                objOpenStockPLRpt.GROUPCODE = "A";
                _unitOfWork.Repository<PROFITANDLOSS_RPT>().Insert(objOpenStockPLRpt);
                _unitOfWork.Save();

                //READ Closing STOCK
                ACdescription = "Closing Stock";
                ACType = "Sales";
                Opening = 0 - repo.sp_GetStockValuation(FDate);
                UptoYear = 0 - repo.sp_GetStockValuation(FDate);
                var objClosingStockPLRpt = _unitOfWork.Repository<PROFITANDLOSS_RPT>().Create();
                objClosingStockPLRpt.ACCOUNTCODE_PL = ACType.Trim();
                objClosingStockPLRpt.DESCRIPTION_PL = ACdescription.Trim(); ;
                objClosingStockPLRpt.CURMONTH_PL = Opening;
                objClosingStockPLRpt.CURRENT_YEARTODATE = UptoYear;
                objClosingStockPLRpt.GROUPCODE = "B";
                _unitOfWork.Repository<PROFITANDLOSS_RPT>().Insert(objClosingStockPLRpt);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
            }
            //}
        }
        private void Proc_BalanceSheet(DateTime SDate, DateTime FDate)
        {
            string ACCode = null;
            string ACdescription = null;
            decimal Opening = 0;
            decimal UptoYear = 0;
            decimal DebitAmount = 0;
            //decimal CreditAmount = 0;
            //decimal Net = 0;
            string ACType = null;
            //decimal QTY = 0;
            //decimal RATE_Renamed = 0;
            //decimal stockAmount = 0;
            string ARTYPE = null;
            //decimal CurMonth = 0;
            //decimal CurYear = 0;
            string Code = null;

            _unitOfWork.Truncate("BALANCESHEET");
            //using (var transaction = _unitOfWork.BeginTransaction())
            //{
            try
            {
                //PURCHASE AND OPENING STOCK  'CURRENT PERRIOD  'UP TO PERRIOD
                //COST OF SALES
                //READ GLMASTER
                ReportRepository repo = _unitOfWork.ExtRepositoryFor<ReportRepository>();
                var glMasterRecords = repo.sp_TransactionQueries(0);

                foreach (var record in glMasterRecords)
                {
                    ACCode = "";
                    ACdescription = "";
                    ACCode = record.GLCODE_LM;
                    int glCode = Convert.ToInt32(record.GLCODE_LM);
                    ACdescription = record.GLDESCRIPTION_LM;
                    if (glCode >= 1000 & glCode < 1499)
                    {
                        Code = "A";
                        ACType = "Current Asset";
                    }
                    if (glCode >= 1500 & glCode < 1999)
                    {
                        Code = "A";
                        ACType = "Non Current Asset";
                    }

                    if (glCode >= 2000 & glCode < 2499)
                    {
                        Code = "L";
                        ACType = "Current Liability";
                    }
                    if (glCode >= 2500 & glCode < 2899)
                    {
                        Code = "L";
                        ACType = "Long Term Liability";
                    }
                    if (glCode >= 2900 & glCode < 2999)
                    {
                        Code = "L";
                        ACType = "Equity/Capital";
                    }

                    //READ ACCOUNT BALANCE OF EACH A/C between period
                    Opening = 0;
                    //READ UPT PERRIOD
                    UptoYear = 0;

                    var lstUptoYear = (from glTransaction in _unitOfWork.Repository<GLTRANSACTION1>().Query().Get()
                                       where Convert.ToInt32(glTransaction.GLACCODE_GLT) == glCode &&
                                       DbFunctions.TruncateTime(glTransaction.GLDATE_GLT.Value) == DbFunctions.TruncateTime(FDate)
                                       group glTransaction by glTransaction.GLACCODE_GLT into glTransaction
                                       select new { SumTotal = glTransaction.Sum(x => (x.DEBITAMOUNT_GLT != null ? x.DEBITAMOUNT_GLT : 0) - (x.CREDITAMOUNT_GLT != null ? x.CREDITAMOUNT_GLT : 0)) }).ToList();
                    UptoYear = lstUptoYear != null && lstUptoYear.Count > 0 ? Convert.ToDecimal(lstUptoYear[0].SumTotal) : 0;

                    var objBalanceSheet = _unitOfWork.Repository<BALANCESHEET>().Create();
                    objBalanceSheet.MAINCODE_BS = Code.Trim();
                    objBalanceSheet.SUBDESCRIPTION_BS = ACType.Trim();
                    objBalanceSheet.ACDESCRIPTION_BS = ACdescription.Trim();
                    objBalanceSheet.AMOUNT_BS = UptoYear;
                    _unitOfWork.Repository<BALANCESHEET>().Insert(objBalanceSheet);
                    _unitOfWork.Save();
                }
                //AR AP LEDGER READ
                var arapMasterRecords = (from arapMaster in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get()
                                         select arapMaster).ToList();
                foreach (var record in arapMasterRecords)
                {
                    ACCode = "";
                    ACdescription = "";
                    ARTYPE = "";

                    ACCode = record.ARCODE_ARM;
                    ACdescription = record.DESCRIPTION_ARM;
                    ARTYPE = record.TYPE_ARM;
                    if (ARTYPE.Trim() == "AR")
                    {
                        Code = "A";
                        ACType = "Accounts Receivable";
                    }
                    if (ARTYPE.Trim() == "AP")
                    {
                        Code = "L";
                        ACType = "Accounts Payable";
                    }
                    //READ ACCOUNT BALANCE OF EACH A/C between period
                    Opening = 0;

                    UptoYear = 0;

                    var lstLgrUptoYear = (from arapLedger in _unitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                                          where arapLedger.ARAPCODE_ART == ACCode.Trim() &&
                                          DbFunctions.TruncateTime(arapLedger.GLDATE_ART.Value) == DbFunctions.TruncateTime(FDate)
                                          group arapLedger by arapLedger.ARAPCODE_ART into arapLedger
                                          select new { SumTotal = arapLedger.Sum(x => (x.DEBITAMOUNT_ART != null ? x.DEBITAMOUNT_ART : 0) - (x.CREDITAMOUNT_ART != null ? x.CREDITAMOUNT_ART : 0)) }).ToList();
                    UptoYear = lstLgrUptoYear != null && lstLgrUptoYear.Count > 0 ? Convert.ToDecimal(lstLgrUptoYear[0].SumTotal) : 0;
                    var objBalanceSheet = _unitOfWork.Repository<BALANCESHEET>().Create();
                    objBalanceSheet.MAINCODE_BS = Code.Trim();
                    objBalanceSheet.SUBDESCRIPTION_BS = ACType.Trim();
                    objBalanceSheet.ACDESCRIPTION_BS = ACdescription.Trim();
                    objBalanceSheet.AMOUNT_BS = UptoYear;
                    _unitOfWork.Repository<BALANCESHEET>().Insert(objBalanceSheet);
                    _unitOfWork.Save();
                }

                //BANK DETAILS INSERT

                var bankMasterRecords = (from bankMaster in _unitOfWork.Repository<BANKMASTER>().Query().Get()
                                         select bankMaster).ToList();
                foreach (var record in bankMasterRecords)
                {
                    ACCode = "";
                    ACdescription = "";
                    ARTYPE = "";

                    ACCode = record.BANKCODE_BM;
                    ACdescription = record.BANKNAME_BM;

                    Code = "A";
                    ACType = "Bank Details";

                    //READ ACCOUNT BALANCE OF EACH A/C between period
                    Opening = 0;

                    UptoYear = 0;

                    var lstBTUptoYear = (from bankTransaction in _unitOfWork.Repository<BANKTRANSACTION>().Query().Get()
                                         where bankTransaction.BANKCODE_BT == ACCode.Trim() &&
                                         DbFunctions.TruncateTime(bankTransaction.GLDATE_BT.Value) == DbFunctions.TruncateTime(FDate)
                                         group bankTransaction by bankTransaction.BANKCODE_BT into bankTransaction
                                         select new { SumTotal = bankTransaction.Sum(x => (x.DEBITAMOUT_BT != null ? x.DEBITAMOUT_BT : 0) - (x.CREDITAMOUT_BT != null ? x.CREDITAMOUT_BT : 0)) }).ToList();
                    UptoYear = lstBTUptoYear != null && lstBTUptoYear.Count > 0 ? Convert.ToDecimal(lstBTUptoYear[0].SumTotal) : 0;

                    var objBalanceSheet = _unitOfWork.Repository<BALANCESHEET>().Create();
                    objBalanceSheet.MAINCODE_BS = Code.Trim();
                    objBalanceSheet.SUBDESCRIPTION_BS = ACType.Trim();
                    objBalanceSheet.ACDESCRIPTION_BS = ACdescription.Trim();
                    objBalanceSheet.AMOUNT_BS = UptoYear;
                    _unitOfWork.Repository<BALANCESHEET>().Insert(objBalanceSheet);
                    _unitOfWork.Save();
                }

                //find Profit and loss account Balance

                var lstGLTDebitAmount = (from glTransaction in _unitOfWork.Repository<GLTRANSACTION1>().Query().Get()
                                         where (Convert.ToInt32(glTransaction.GLACCODE_GLT) >= 4000 && Convert.ToInt32(glTransaction.GLACCODE_GLT) <= 4999) &&
                                         DbFunctions.TruncateTime(glTransaction.GLDATE_GLT.Value) == DbFunctions.TruncateTime(FDate)
                                         group glTransaction by glTransaction.GLACCODE_GLT into glTransaction
                                         select new { SumTotal = glTransaction.Sum(x => (x.DEBITAMOUNT_GLT != null ? x.DEBITAMOUNT_GLT : 0) - (x.CREDITAMOUNT_GLT != null ? x.CREDITAMOUNT_GLT : 0)) }).ToList();
                DebitAmount = lstGLTDebitAmount != null && lstGLTDebitAmount.Count > 0 ? Convert.ToDecimal(lstGLTDebitAmount[0].SumTotal) : 0;
            }
            catch (Exception)
            {
            }
            //}
        }
        //BALANSE SHEET**************** BEFORE CALLL P AND L TRANSER****************
        private void Proc_BalanseSheet(DateTime SDate, DateTime FDate)
        {
            string ACCode = null;
            string ACdescription = null;

            decimal Opening = default(decimal);
            decimal UptoYear = default(decimal);

            //decimal DebitAmount = default(decimal);
            //decimal CreditAmount = default(decimal);
            //decimal Net = default(decimal);
            string ACType = null;
            //decimal QTY = default(decimal);
            //decimal RATE_Renamed = default(decimal);
            //decimal stockAmount = default(decimal);

            //decimal CurMonth = default(decimal);
            //decimal CurYear = default(decimal);
            string s = null;
            System.DateTime yearStart = default(System.DateTime);
            string Code = null;
            s = "01-jan-" + FDate.Year;
            yearStart = Convert.ToDateTime(s);
            _unitOfWork.Truncate("BALANCESHEET");
            //using (var transaction = _unitOfWork.BeginTransaction())
            //{
            try
            {
                //PURCHASE AND OPENING STOCK  'CURRENT PERRIOD  'UP TO PERRIOD
                //COST OF SALES
                //READ GLMASTER
                ReportRepository repo = _unitOfWork.ExtRepositoryFor<ReportRepository>();
                var glMasterRecords = repo.sp_TransactionQueries(0);

                foreach (var record in glMasterRecords)
                {
                    ACCode = "";
                    ACdescription = "";
                    ACCode = record.GLCODE_LM;
                    int glCode = Convert.ToInt32(record.GLCODE_LM);
                    ACdescription = record.GLDESCRIPTION_LM;
                    if (glCode >= 1000 & glCode < 1500)
                    {
                        Code = "A";
                        ACType = "Current Asset";
                    }
                    if (glCode >= 1500 & glCode < 1900)
                    {
                        Code = "A";
                        ACType = "Non Current Asset";
                    }
                    if (glCode >= 1900 & glCode < 2000)
                    {
                        Code = "A";
                        ACType = "Intangible Asset";
                    }
                    if (glCode >= 2000 & glCode < 2500)
                    {
                        Code = "L";
                        ACType = "Current Liability";
                    }
                    if (glCode >= 2500 & glCode < 2900)
                    {
                        Code = "L";
                        ACType = "LongTerm Liability";
                    }
                    if (glCode >= 2900 & glCode < 3000)
                    {
                        Code = "L";
                        ACType = "Equity/ Capital";
                    }

                    //READ ACCOUNT BALANCE OF EACH A/C between period
                    Opening = 0;
                    UptoYear = 0;

                    //var lstUptoYear = (from glTransaction in _unitOfWork.Repository<GLTRANSACTION1>().Query().Get()
                    //                   where glTransaction.GLACCODE_GLT.Trim() == ACCode &&
                    //                   (DbFunctions.TruncateTime(glTransaction.GLDATE_GLT) >= yearStart.Date && DbFunctions.TruncateTime(glTransaction.GLDATE_GLT) <= FDate.Date)
                    //                   group glTransaction by glTransaction.GLACCODE_GLT into glTransactionG
                    //                   select new { SumTotal = glTransactionG.Sum(x => (x.DEBITAMOUNT_GLT != null ? x.DEBITAMOUNT_GLT : 0) - (x.CREDITAMOUNT_GLT != null ? x.CREDITAMOUNT_GLT : 0)) }).ToList();

                    //var lstOpening = (from glTransaction in _unitOfWork.Repository<GLTRANSACTION1>().Query().Get()
                    //                  where glTransaction.GLACCODE_GLT.Trim() == ACCode &&
                    //                  (DbFunctions.TruncateTime(glTransaction.GLDATE_GLT) >= SDate.Date && DbFunctions.TruncateTime(glTransaction.GLDATE_GLT) <= FDate.Date)
                    //                  group glTransaction by glTransaction.GLACCODE_GLT into glTransactionG
                    //                  select new { SumTotal = glTransactionG.Sum(x => (x.DEBITAMOUNT_GLT != null ? x.DEBITAMOUNT_GLT : 0) - (x.CREDITAMOUNT_GLT != null ? x.CREDITAMOUNT_GLT : 0)) }).ToList();

                    //UptoYear = lstUptoYear != null && lstUptoYear.Count > 0 ? Convert.ToDecimal(lstUptoYear[0]) : 0;
                    //Opening = lstOpening != null && lstOpening.Count > 0 ? Convert.ToDecimal(lstOpening[0]) : 0;
                    var lstUptoYear = repo.sp_FinancialTables(1, ACCode, yearStart, FDate);
                    var lstOpening = repo.sp_FinancialTables(1, ACCode, SDate, FDate);
                    UptoYear = lstUptoYear != null && lstUptoYear.Count > 0 ? Convert.ToDecimal(lstUptoYear[0].Sum1) : 0;
                    Opening = lstOpening != null && lstOpening.Count > 0 ? Convert.ToDecimal(lstOpening[0].Sum1) : 0;

                    var objBalanceSheet = _unitOfWork.Repository<BALANCESHEET>().Create();
                    objBalanceSheet.MAINCODE_BS = Code.Trim();
                    objBalanceSheet.ACCODE_BS = ACCode;
                    objBalanceSheet.SUBDESCRIPTION_BS = ACType.Trim();
                    objBalanceSheet.ACDESCRIPTION_BS = ACdescription.Trim();
                    objBalanceSheet.AMOUNT_BS = Opening;
                    objBalanceSheet.AMOUNT2_BS = UptoYear;
                    _unitOfWork.Repository<BALANCESHEET>().Insert(objBalanceSheet);
                    _unitOfWork.Save();
                }
                Opening = 0;
                UptoYear = 0;
                var PLReportRecords = repo.sp_FinancialTables(2, null, SDate, FDate);
                //(from plRecords in _unitOfWork.Repository<PROFITANDLOSS_RPT>().Query().Get()
                //                       group plRecords by plRecords.GROUPCODE into plRecordsG
                //                       select new
                //                       {
                //                           curMonthSum = plRecordsG.Sum(x => x.CURMONTH_PL != null ? x.CURMONTH_PL : 0),
                //                           CurYear2DateSum = plRecordsG.Sum(x => x.CURRENT_YEARTODATE != null ? x.CURRENT_YEARTODATE : 0)
                //                       }).Select(a => new { a.curMonthSum, a.CurYear2DateSum }).ToList();
                Code = "L";
                ACType = "Equity / Capital";
                if (PLReportRecords != null && PLReportRecords.Count > 0)
                {
                    Opening = Convert.ToDecimal(PLReportRecords[0].Sum1);
                    UptoYear = Convert.ToDecimal(PLReportRecords[0].Sum2);
                }
                ACdescription = "Profit & Loss Account";
                ACCode = "6000";
                var objPLBalanceSheet = _unitOfWork.Repository<BALANCESHEET>().Create();
                objPLBalanceSheet.MAINCODE_BS = Code.Trim();
                objPLBalanceSheet.ACCODE_BS = ACCode;
                objPLBalanceSheet.SUBDESCRIPTION_BS = ACType.Trim();
                objPLBalanceSheet.ACDESCRIPTION_BS = ACdescription.Trim();
                objPLBalanceSheet.AMOUNT_BS = Opening;
                objPLBalanceSheet.AMOUNT2_BS = UptoYear;
                _unitOfWork.Repository<BALANCESHEET>().Insert(objPLBalanceSheet);
                _unitOfWork.Save();

                //READ Closinbg STOCK
                //AR TRANSFT
                Opening = 0;
                UptoYear = 0;

                //var lstArUptoYear = (from arapLedger in _unitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                //                     join arapMaster in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get() on arapLedger.ARAPCODE_ART equals arapMaster.ARCODE_ARM
                //                     where ((arapLedger.ARAPCODE_ART.Equals(arapMaster.ARCODE_ARM) && arapMaster.TYPE_ARM.Equals("AR")
                //                     &&
                //                     (DbFunctions.TruncateTime(arapLedger.GLDATE_ART) >= yearStart.Date && DbFunctions.TruncateTime(arapLedger.GLDATE_ART) <= FDate.Date)))
                //                     group arapLedger by arapLedger.ARAPCODE_ART into arapLedgerG
                //                     select new { SumTotal = arapLedgerG.Sum(x => (x.DEBITAMOUNT_ART != null ? x.DEBITAMOUNT_ART : 0) - (x.CREDITAMOUNT_ART != null ? x.CREDITAMOUNT_ART : 0)) }).ToList();
                //var lstArOpening = (from arapLedger in _unitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                //                    join arapMaster in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get() on arapLedger.ARAPCODE_ART equals arapMaster.ARCODE_ARM
                //                    where ((arapLedger.ARAPCODE_ART.Equals(arapMaster.ARCODE_ARM) && arapMaster.TYPE_ARM.Equals("AR")
                //                    &&
                //                    (DbFunctions.TruncateTime(arapLedger.GLDATE_ART) >= SDate.Date && DbFunctions.TruncateTime(arapLedger.GLDATE_ART) <= FDate.Date)))
                //                    group arapLedger by arapLedger.ARAPCODE_ART into arapLedgerG
                //                    select new { SumTotal = arapLedgerG.Sum(x => (x.DEBITAMOUNT_ART != null ? x.DEBITAMOUNT_ART : 0) - (x.CREDITAMOUNT_ART != null ? x.CREDITAMOUNT_ART : 0)) }).ToList();
                //UptoYear = lstArUptoYear != null && lstArUptoYear.Count > 0 ? Convert.ToDecimal(lstArUptoYear[0].SumTotal) : 0;
                //Opening = lstArOpening != null && lstArOpening.Count > 0 ? Convert.ToDecimal(lstArOpening[0].SumTotal) : 0;
                var lstArUptoYear = repo.sp_FinancialTables(3, ACCode, yearStart, FDate);
                var lstArOpening = repo.sp_FinancialTables(3, ACCode, SDate, FDate);
                UptoYear = lstArUptoYear != null && lstArUptoYear.Count > 0 ? Convert.ToDecimal(lstArUptoYear[0].Sum1) : 0;
                Opening = lstArOpening != null && lstArOpening.Count > 0 ? Convert.ToDecimal(lstArOpening[0].Sum1) : 0;
                Code = "A";
                ACType = "Current Asset";
                ACdescription = "ACCOUNT RECEIVABLE";
                ACCode = "9000";
                var objARBalanceSheet = _unitOfWork.Repository<BALANCESHEET>().Create();
                objARBalanceSheet.MAINCODE_BS = Code.Trim();
                objARBalanceSheet.ACCODE_BS = ACCode;
                objARBalanceSheet.SUBDESCRIPTION_BS = ACType.Trim();
                objARBalanceSheet.ACDESCRIPTION_BS = ACdescription.Trim();
                objARBalanceSheet.AMOUNT_BS = Opening;
                objARBalanceSheet.AMOUNT2_BS = UptoYear;
                _unitOfWork.Repository<BALANCESHEET>().Insert(objARBalanceSheet);
                _unitOfWork.Save();

                //AP TRANSFT
                Opening = 0;
                UptoYear = 0;
                //var lstApUptoYear = (from arapLedger in _unitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                //                     join arapMaster in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get() on arapLedger.ARAPCODE_ART equals arapMaster.ARCODE_ARM
                //                     where ((arapLedger.ARAPCODE_ART.Equals(arapMaster.ARCODE_ARM) && arapMaster.TYPE_ARM.Equals("AP")
                //                     &&
                //                     (DbFunctions.TruncateTime(arapLedger.GLDATE_ART) >= yearStart.Date && DbFunctions.TruncateTime(arapLedger.GLDATE_ART) <= FDate.Date)))
                //                     group arapLedger by arapLedger.ARAPCODE_ART into arapLedgerG
                //                     select new { SumTotal = arapLedgerG.Sum(x => (x.DEBITAMOUNT_ART != null ? x.DEBITAMOUNT_ART : 0) - (x.CREDITAMOUNT_ART != null ? x.CREDITAMOUNT_ART : 0)) }).ToList();
                //var lstApOpening = (from arapLedger in _unitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                //                    join arapMaster in _unitOfWork.Repository<AR_AP_MASTER>().Query().Get() on arapLedger.ARAPCODE_ART equals arapMaster.ARCODE_ARM
                //                    where ((arapLedger.ARAPCODE_ART.Equals(arapMaster.ARCODE_ARM) && arapMaster.TYPE_ARM.Equals("AP")
                //                    &&
                //                    (DbFunctions.TruncateTime(arapLedger.GLDATE_ART) >= SDate.Date && DbFunctions.TruncateTime(arapLedger.GLDATE_ART) <= FDate.Date)))
                //                    group arapLedger by arapLedger.ARAPCODE_ART into arapLedgerG
                //                    select new { SumTotal = arapLedgerG.Sum(x => (x.DEBITAMOUNT_ART != null ? x.DEBITAMOUNT_ART : 0) - (x.CREDITAMOUNT_ART != null ? x.CREDITAMOUNT_ART : 0)) }).ToList();
                //UptoYear = lstApUptoYear != null && lstApUptoYear.Count > 0 ? Convert.ToDecimal(lstApUptoYear[0].SumTotal) : 0;
                //Opening = lstApOpening != null && lstApOpening.Count > 0 ? Convert.ToDecimal(lstApOpening[0].SumTotal) : 0;
                var lstApUptoYear = repo.sp_FinancialTables(4, ACCode, yearStart, FDate);
                var lstApOpening = repo.sp_FinancialTables(4, ACCode, SDate, FDate);
                UptoYear = lstApUptoYear != null && lstApUptoYear.Count > 0 ? Convert.ToDecimal(lstApUptoYear[0].Sum1) : 0;
                Opening = lstApOpening != null && lstApOpening.Count > 0 ? Convert.ToDecimal(lstApOpening[0].Sum1) : 0;

                Code = "L";
                ACType = "Current Liability";
                ACdescription = "ACCOUNT PAYABLE";
                ACCode = "9500";
                var objAPBalanceSheet = _unitOfWork.Repository<BALANCESHEET>().Create();
                objAPBalanceSheet.MAINCODE_BS = Code.Trim();
                objAPBalanceSheet.ACCODE_BS = ACCode;
                objAPBalanceSheet.SUBDESCRIPTION_BS = ACType.Trim();
                objAPBalanceSheet.ACDESCRIPTION_BS = ACdescription.Trim();
                objAPBalanceSheet.AMOUNT_BS = Opening;
                objAPBalanceSheet.AMOUNT2_BS = UptoYear;
                _unitOfWork.Repository<BALANCESHEET>().Insert(objAPBalanceSheet);
                _unitOfWork.Save();

                //BANK TRANSFER

                var bankMasterRecords = (from bankMaster in _unitOfWork.Repository<BANKMASTER>().Query().Get()
                                         select bankMaster).ToList();
                foreach (var record in bankMasterRecords)
                {
                    ACCode = "";
                    ACdescription = "";

                    ACCode = record.BANKCODE_BM;
                    ACdescription = record.BANKNAME_BM;

                    Code = "A";
                    ACType = "Bank Details";

                    //READ ACCOUNT BALANCE OF EACH A/C between period
                    Opening = 0;

                    UptoYear = 0;

                    //var lstBTUptoYear = (from bnkTransaction in _unitOfWork.Repository<BANKTRANSACTION>().Query().Get()
                    //                     where bnkTransaction.BANKCODE_BT.Trim() == ACCode &&
                    //                     (DbFunctions.TruncateTime(bnkTransaction.GLDATE_BT) >= yearStart.Date && DbFunctions.TruncateTime(bnkTransaction.GLDATE_BT) <= FDate.Date)
                    //                     group bnkTransaction by bnkTransaction.BANKCODE_BT into bnkTransactionG
                    //                     select new { SumTotal = bnkTransactionG.Sum(x => (x.DEBITAMOUT_BT != null ? x.DEBITAMOUT_BT : 0) - (x.CREDITAMOUT_BT != null ? x.CREDITAMOUT_BT : 0)) }).ToList();

                    //var lstBTOpening = (from bnkTransaction in _unitOfWork.Repository<BANKTRANSACTION>().Query().Get()
                    //                    where bnkTransaction.BANKCODE_BT.Trim() == ACCode &&
                    //                    (DbFunctions.TruncateTime(bnkTransaction.GLDATE_BT) >= SDate.Date && DbFunctions.TruncateTime(bnkTransaction.GLDATE_BT) <= FDate.Date)
                    //                    group bnkTransaction by bnkTransaction.BANKCODE_BT into bnkTransactionG
                    //                    select new { SumTotal = bnkTransactionG.Sum(x => (x.DEBITAMOUT_BT != null ? x.DEBITAMOUT_BT : 0) - (x.CREDITAMOUT_BT != null ? x.CREDITAMOUT_BT : 0)) }).ToList();

                    //UptoYear = lstBTUptoYear != null && lstBTUptoYear.Count > 0 ? Convert.ToDecimal(lstBTUptoYear[0].SumTotal) : 0;
                    //Opening = lstBTOpening != null && lstBTOpening.Count > 0 ? Convert.ToDecimal(lstBTOpening[0].SumTotal) : 0;
                    var lstBTUptoYear = repo.sp_FinancialTables(5, ACCode, yearStart, FDate);
                    var lstBTOpening = repo.sp_FinancialTables(5, ACCode, SDate, FDate);
                    UptoYear = lstBTUptoYear != null && lstBTUptoYear.Count > 0 ? Convert.ToDecimal(lstBTUptoYear[0].Sum1) : 0;
                    Opening = lstBTOpening != null && lstBTOpening.Count > 0 ? Convert.ToDecimal(lstBTOpening[0].Sum1) : 0;

                    Code = "A";
                    ACType = "Current Asset";
                    var objBalanceSheet = _unitOfWork.Repository<BALANCESHEET>().Create();
                    objBalanceSheet.MAINCODE_BS = Code.Trim();
                    objBalanceSheet.ACCODE_BS = ACCode;
                    objBalanceSheet.SUBDESCRIPTION_BS = ACType.Trim();
                    objBalanceSheet.ACDESCRIPTION_BS = ACdescription.Trim();
                    objBalanceSheet.AMOUNT_BS = Opening;
                    objBalanceSheet.AMOUNT2_BS = UptoYear;
                    _unitOfWork.Repository<BALANCESHEET>().Insert(objBalanceSheet);
                    _unitOfWork.Save();
                }
                //CLOSING STOCK ADD

                Opening = repo.sp_GetStockValuation(FDate);
                UptoYear = repo.sp_GetStockValuation(FDate);
                Code = "A";
                ACType = "Current Asset";
                ACdescription = "CLOSING STOCK";
                ACCode = "9000";
                var objStockBalanceSheet = _unitOfWork.Repository<BALANCESHEET>().Create();
                objStockBalanceSheet.MAINCODE_BS = Code.Trim();
                objStockBalanceSheet.ACCODE_BS = ACCode;
                objStockBalanceSheet.SUBDESCRIPTION_BS = ACType.Trim();
                objStockBalanceSheet.ACDESCRIPTION_BS = ACdescription.Trim();
                objStockBalanceSheet.AMOUNT_BS = Opening;
                objStockBalanceSheet.AMOUNT2_BS = UptoYear;
                _unitOfWork.Repository<BALANCESHEET>().Insert(objStockBalanceSheet);
                _unitOfWork.Save();

            }
            catch (Exception ex)
            {
            }
            //}
        }
    }
}