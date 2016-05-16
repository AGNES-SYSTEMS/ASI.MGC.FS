using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;
using ASI.MGC.FS.Models;
using ASI.MGC.FS.WebCommon;

namespace ASI.MGC.FS.Controllers
{
    public class FinanceController : Controller
    {
        readonly IUnitOfWork _unitOfWork;
        public FinanceController()
        {
            _unitOfWork = new UnitOfWork();
        }
        // GET: Finance
        public ActionResult GlCreation()
        {
            var objGlMaster = _unitOfWork.Repository<GLMASTER>().Create();
            ViewBag.accountsType = CommonModelAccessUtility.GetAccountsType();
            ViewBag.glType = CommonModelAccessUtility.GetGlType();
            ViewBag.balanceType = CommonModelAccessUtility.GetBalanceType();
            return View(objGlMaster);
        }

        public JsonResult SaveGlCreation(FormCollection frm, GLMASTER objGlMaster)
        {
            try
            {
                _unitOfWork.Repository<GLMASTER>().Insert(objGlMaster);
                _unitOfWork.Save();

                var objGlTransaction = _unitOfWork.Repository<GLTRANSACTION1>().Create();
                objGlTransaction.DOCDATE_GLT = DateTime.Now;
                objGlTransaction.GLDATE_GLT = Convert.ToDateTime(frm["GLDate"]);
                objGlTransaction.DOCNUMBER_GLT = objGlMaster.GLCODE_LM;
                objGlTransaction.GLACCODE_GLT = objGlMaster.GLCODE_LM;
                objGlTransaction.OTHERREF_GLT = objGlMaster.GLCODE_LM;
                if (Convert.ToInt32(frm["BalanceType"]) == 1)
                {
                    objGlTransaction.CREDITAMOUNT_GLT = Convert.ToDecimal(frm["OpenBalance"]);
                    objGlTransaction.DEBITAMOUNT_GLT = 0;
                }
                else
                {
                    objGlTransaction.CREDITAMOUNT_GLT = 0;
                    objGlTransaction.DEBITAMOUNT_GLT = Convert.ToDecimal(frm["OpenBalance"]);
                }
                objGlTransaction.NARRATION_GLT = "Opening Balance";
                objGlTransaction.GLSTATUS_GLT = "OP";
                _unitOfWork.Repository<GLTRANSACTION1>().Insert(objGlTransaction);
                _unitOfWork.Save();

            }
            catch (Exception)
            {
                // ignored
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AccountsReceivable()
        {
            return View();
        }

        public ActionResult AccountsPayable()
        {
            return View();
        }

        public ActionResult SaveAccountsPayable()
        {
            throw new System.NotImplementedException();
        }

        public ActionResult SaveAccountsReceivable()
        {
            throw new System.NotImplementedException();
        }

        public ActionResult PdcReceipt()
        {
            return View();
        }

        public ActionResult JvCreation()
        {
            return View();
        }

        public ActionResult ArapMatching()
        {
            return View();
        }

        public ActionResult ArUnmatching()
        {
            ViewBag.DocType = CommonModelAccessUtility.GetDocTypes(_unitOfWork);
            return View();
        }

        public ActionResult DocumentReversal()
        {
            @ViewBag.DocType = CommonModelAccessUtility.GetDocTypes(_unitOfWork);
            return View();
        }

        public ActionResult DayEndOperation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SavePdcReceipt(FormCollection form, BANKTRANSACTION objBankTransaction)
        {
            string pdcNo = "";
            try
            {
                objBankTransaction.STATUS_BT = "P";
                objBankTransaction.MASTERSTATUS_BT = "M";
                _unitOfWork.Repository<BANKTRANSACTION>().Insert(objBankTransaction);
                _unitOfWork.Save();
                _unitOfWork.Truncate("VOUCHERMASTER_RPT");
                _unitOfWork.Truncate("VOUCHERCHILD_RPT");
                pdcNo = objBankTransaction.DOCNUMBER_BT;
                var objVoucherMaster = _unitOfWork.Repository<VOUCHERMASTER_RPT>().Create();
                objVoucherMaster.GLDATE_VRPT = objBankTransaction.GLDATE_BT;
                objVoucherMaster.ALLCODE_VRPT = "Bank";
                objVoucherMaster.BANKCODE_VRT = objBankTransaction.BANKCODE_BT;
                objVoucherMaster.ACCDESCRIPTION_VRPT = Convert.ToString(form["BankName"]);
                objVoucherMaster.PARTICULARS_VRPT = objBankTransaction.NARRATION_BT;
                objVoucherMaster.NOTES_VRPT = objBankTransaction.NOTE_BT;
                objVoucherMaster.DEBITAMOUT_VRPT = objBankTransaction.DEBITAMOUT_BT;
                objVoucherMaster.CREDITAMOUNT_VRPT = 0;
                objVoucherMaster.CHQNO_VRPT = objBankTransaction.CHQNO_BT;
                objVoucherMaster.CHQDATE_VRPT = objBankTransaction.CHQDATE_BT;
                objVoucherMaster.DOCNO_VRPT = objBankTransaction.DOCNUMBER_BT;
                objVoucherMaster.VOUCHER_TYPE = "BR";
                _unitOfWork.Repository<VOUCHERMASTER_RPT>().Insert(objVoucherMaster);
                _unitOfWork.Save();

                string jsonAllocDetails = form["allocDetails"];
                var serializer = new JavaScriptSerializer();
                var lstAllocDetails = serializer.Deserialize<List<CustomAllocationDetails>>(jsonAllocDetails);
                foreach (var allocDetail in lstAllocDetails)
                {
                    switch (allocDetail.AlCode)
                    {
                        case "AP":
                            var objApLedger = _unitOfWork.Repository<AR_AP_LEDGER>().Create();
                            objApLedger.DOCNUMBER_ART = objBankTransaction.DOCNUMBER_BT;
                            objApLedger.DODATE_ART = objBankTransaction.DOCDATE_BT;
                            objApLedger.GLDATE_ART = objBankTransaction.GLDATE_BT;
                            objApLedger.ARAPCODE_ART = allocDetail.AccountCode;
                            objApLedger.CREDITAMOUNT_ART = Convert.ToInt32(allocDetail.Amount);
                            objApLedger.OTHERREF_ART = objBankTransaction.OTHERREF_BT;
                            objApLedger.NARRATION_ART = allocDetail.Narration;
                            objApLedger.MATCHVALUE_AR = 0;
                            objApLedger.STATUS_ART = "P";
                            _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objApLedger);
                            _unitOfWork.Save();

                            var objApVoucherChild = _unitOfWork.Repository<VOUCHERCHILD_RPT>().Create();
                            objApVoucherChild.ALLCODE_VCD = "GL";
                            objApVoucherChild.NARRATION_VCD = objBankTransaction.NARRATION_BT;
                            objApVoucherChild.ACCODE_VCD = allocDetail.AccountCode;
                            objApVoucherChild.ACDESCRIPTION_VCD = allocDetail.Description;
                            objApVoucherChild.CREDITAMOUNT_VCD = Convert.ToDecimal(allocDetail.Amount);
                            objApVoucherChild.AMOUNTSTATUTS_VCD = "Cr";
                            objApVoucherChild.DOCNO_VCD = objBankTransaction.DOCNUMBER_BT;
                            _unitOfWork.Repository<VOUCHERCHILD_RPT>().Insert(objApVoucherChild);
                            _unitOfWork.Save();
                            break;
                        case "AR":
                            var objArLedger = _unitOfWork.Repository<AR_AP_LEDGER>().Create();
                            objArLedger.DOCNUMBER_ART = objBankTransaction.DOCNUMBER_BT;
                            objArLedger.DODATE_ART = objBankTransaction.DOCDATE_BT;
                            objArLedger.GLDATE_ART = objBankTransaction.GLDATE_BT;
                            objArLedger.ARAPCODE_ART = allocDetail.AccountCode;
                            objArLedger.CREDITAMOUNT_ART = Convert.ToInt32(allocDetail.Amount);
                            objArLedger.OTHERREF_ART = objBankTransaction.OTHERREF_BT;
                            objArLedger.NARRATION_ART = allocDetail.Narration;
                            objArLedger.MATCHVALUE_AR = 0;
                            objArLedger.STATUS_ART = "P";
                            _unitOfWork.Repository<AR_AP_LEDGER>().Insert(objArLedger);
                            _unitOfWork.Save();

                            var objArVoucherChild = _unitOfWork.Repository<VOUCHERCHILD_RPT>().Create();
                            objArVoucherChild.ALLCODE_VCD = "GL";
                            objArVoucherChild.NARRATION_VCD = objBankTransaction.NARRATION_BT;
                            objArVoucherChild.ACCODE_VCD = allocDetail.AccountCode;
                            objArVoucherChild.ACDESCRIPTION_VCD = allocDetail.Description;
                            objArVoucherChild.CREDITAMOUNT_VCD = Convert.ToDecimal(allocDetail.Amount);
                            objArVoucherChild.AMOUNTSTATUTS_VCD = "Cr";
                            objArVoucherChild.DOCNO_VCD = objBankTransaction.DOCNUMBER_BT;
                            _unitOfWork.Repository<VOUCHERCHILD_RPT>().Insert(objArVoucherChild);
                            _unitOfWork.Save();
                            break;
                        case "BA":
                            var objBTransaction = _unitOfWork.Repository<BANKTRANSACTION>().Create();
                            objBTransaction.DOCNUMBER_BT = objBankTransaction.DOCNUMBER_BT;
                            objBTransaction.BANKCODE_BT = allocDetail.AccountCode;
                            objBTransaction.DOCDATE_BT = objBankTransaction.DOCDATE_BT;
                            objBTransaction.GLDATE_BT = objBankTransaction.GLDATE_BT;
                            objBTransaction.DEBITAMOUT_BT = Convert.ToDecimal(allocDetail.Amount);
                            objBTransaction.OTHERREF_BT = objBankTransaction.OTHERREF_BT;
                            objBTransaction.CHQDATE_BT = objBankTransaction.CHQDATE_BT;
                            objBankTransaction.CLEARANCEDATE_BT = objBankTransaction.CLEARANCEDATE_BT;
                            objBTransaction.NARRATION_BT = allocDetail.Narration;
                            objBTransaction.NOTE_BT = objBankTransaction.NOTE_BT;
                            objBTransaction.STATUS_BT = "P";
                            _unitOfWork.Repository<BANKTRANSACTION>().Insert(objBTransaction);
                            _unitOfWork.Save();
                            break;
                        case "GL":
                            var objGlTransaction = _unitOfWork.Repository<GLTRANSACTION1>().Create();
                            objGlTransaction.DOCNUMBER_GLT = objBankTransaction.DOCNUMBER_BT;
                            objGlTransaction.DOCDATE_GLT = objBankTransaction.DOCDATE_BT;
                            objGlTransaction.GLDATE_GLT = objBankTransaction.GLDATE_BT;
                            objGlTransaction.GLACCODE_GLT = allocDetail.AccountCode;
                            objGlTransaction.OTHERREF_GLT = objBankTransaction.OTHERREF_BT;
                            objGlTransaction.CREDITAMOUNT_GLT = Convert.ToDecimal(allocDetail.Amount);
                            objGlTransaction.NARRATION_GLT = allocDetail.Narration;
                            objGlTransaction.GLSTATUS_GLT = "P";
                            _unitOfWork.Repository<GLTRANSACTION1>().Insert(objGlTransaction);
                            _unitOfWork.Save();

                            var objGlVoucherChild = _unitOfWork.Repository<VOUCHERCHILD_RPT>().Create();
                            objGlVoucherChild.ALLCODE_VCD = "GL";
                            objGlVoucherChild.NARRATION_VCD = objBankTransaction.NARRATION_BT;
                            objGlVoucherChild.ACCODE_VCD = allocDetail.AccountCode;
                            objGlVoucherChild.ACDESCRIPTION_VCD = allocDetail.Description;
                            objGlVoucherChild.CREDITAMOUNT_VCD = Convert.ToDecimal(allocDetail.Amount);
                            objGlVoucherChild.AMOUNTSTATUTS_VCD = "Dr";
                            objGlVoucherChild.DOCNO_VCD = objBankTransaction.DOCNUMBER_BT;
                            _unitOfWork.Repository<VOUCHERCHILD_RPT>().Insert(objGlVoucherChild);
                            _unitOfWork.Save();
                            break;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return Json(pdcNo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveDayEndOperation()
        {
            throw new System.NotImplementedException();
        }

        [HttpPost]
        public ActionResult SaveDocumentReversal(FormCollection frm)
        {
            bool success;
            var invCode = frm["InvNumber"];
            try
            {
                // Checking Bank Transactions entries and adding their reverse
                var btEnteries = (from bankTransactions in _unitOfWork.Repository<BANKTRANSACTION>().Query().Get()
                                  where (bankTransactions.DOCNUMBER_BT == invCode)
                                  select bankTransactions);
                foreach (var entry in btEnteries.ToList())
                {
                    entry.STATUS_BT = "R";
                    _unitOfWork.Repository<BANKTRANSACTION>().Update(entry);
                    _unitOfWork.Save();

                    // adding reverse entry
                    ReverseBankTransactions(entry);
                }

                // Checking AR_AP Ledger entries and adding their reverse
                var arApEnteries = (from arApLedger in _unitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                                    where (arApLedger.DOCNUMBER_ART == invCode)
                                    select arApLedger);
                foreach (var entry in arApEnteries.ToList())
                {
                    entry.STATUS_ART = "R";
                    _unitOfWork.Repository<AR_AP_LEDGER>().Update(entry);
                    _unitOfWork.Save();

                    // adding reverse entry
                    ReverseAr_Ap_Enteries(entry);
                }

                // Checking GL Transactions entries and adding their reverse
                var glEnteries = (from glTransactions in _unitOfWork.Repository<GLTRANSACTION1>().Query().Get()
                                  where (glTransactions.DOCNUMBER_GLT == invCode)
                                  select glTransactions);
                foreach (var entry in glEnteries.ToList())
                {
                    entry.GLSTATUS_GLT = "R";
                    _unitOfWork.Repository<GLTRANSACTION1>().Update(entry);
                    _unitOfWork.Save();

                    // adding reverse entry
                    ReverseGlTransactions(entry);
                }
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            return Json(success, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveArApMatching()
        {
            throw new NotImplementedException();
        }

        public ActionResult SaveArUnMatching()
        {
            throw new System.NotImplementedException();
        }

        public ActionResult AccountsPayableList()
        {
            return View();
        }

        public ActionResult AccountsReceivableList()
        {
            return View();
        }

        public ActionResult GeneralLedgerList()
        {
            return View();
        }
        public ActionResult SaveJvCreation()
        {
            throw new System.NotImplementedException();
        }

        public JsonResult GetGlCodes(string sidx, string sord, int page, int rows, string searchValue = "")
        {
            var glCodeList = (from glDetails in _unitOfWork.Repository<GLMASTER>().Query().Get()
                              select glDetails).Select(a => new { a.GLCODE_LM, a.GLDESCRIPTION_LM });
            if (!string.IsNullOrEmpty(searchValue))
            {
                glCodeList = (from glDetails in _unitOfWork.Repository<GLMASTER>().Query().Get()
                              where glDetails.GLDESCRIPTION_LM.Contains(searchValue)
                              select glDetails).Select(a => new { a.GLCODE_LM, a.GLDESCRIPTION_LM });
            }
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = glCodeList.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / pageSize);
            if (sord.ToUpper() == "ASC")
            {
                glCodeList = glCodeList.OrderBy(a => a.GLCODE_LM);
                glCodeList = glCodeList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            else
            {
                glCodeList = glCodeList.OrderByDescending(a => a.GLCODE_LM);
                glCodeList = glCodeList.Skip(pageIndex * pageSize).Take(pageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = glCodeList

            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetGlCodeDetails(string glCode = "")
        {
            Dictionary<string, string> glData = new Dictionary<string, string>();
            try
            {
                var glCodeData = (from glMaster in _unitOfWork.Repository<GLMASTER>().Query().Get()
                                  where (glMaster.GLCODE_LM == glCode)
                                  select new
                                  {
                                      glMaster.GLCODE_LM,
                                      glMaster.GLDESCRIPTION_LM,
                                      glMaster.SUBCODE_LM,
                                      glMaster.MAINCODE_LM,
                                      glMaster.NOTES_LM,
                                      glMaster.GTYPE_LM
                                  }).SingleOrDefault();
                var glCodeDetails = (from glTransaction in _unitOfWork.Repository<GLTRANSACTION1>().Query().Get()
                                     where (glTransaction.GLACCODE_GLT == glCode && glTransaction.GLSTATUS_GLT == "OP")
                                     select new
                                     {
                                         glTransaction.DEBITAMOUNT_GLT,
                                         glTransaction.CREDITAMOUNT_GLT,
                                         glTransaction.GLDATE_GLT
                                     }).SingleOrDefault();

                if (glCodeData != null)
                {
                    glData.Add("glCode", glCodeData.GLCODE_LM);
                    glData.Add("glDesc", glCodeData.GLDESCRIPTION_LM);
                    glData.Add("glSubCode", glCodeData.SUBCODE_LM);
                    glData.Add("glMainCode", glCodeData.MAINCODE_LM);
                    glData.Add("glNotes", glCodeData.NOTES_LM);
                    glData.Add("glType", glCodeData.GTYPE_LM);
                }
                if (glCodeDetails != null)
                {
                    glData.Add("debitAmount", Convert.ToString(glCodeDetails.DEBITAMOUNT_GLT));
                    glData.Add("creditAmount", Convert.ToString(glCodeDetails.CREDITAMOUNT_GLT));
                    glData.Add("glDate", Convert.ToDateTime(glCodeDetails.GLDATE_GLT).ToShortDateString());
                }
                else
                {
                    glData.Add("debitAmount", "");
                    glData.Add("creditAmount", "");
                    glData.Add("glDate", DateTime.Now.ToShortDateString());
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return Json(glData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetInvDetails(string invCode = "")
        {
            try
            {
                var invCodeData = (from bankTransactions in _unitOfWork.Repository<BANKTRANSACTION>().Query().Get()
                                   where (bankTransactions.DOCNUMBER_BT == invCode)
                                   select new
                                   {
                                       bankTransactions.DOCDATE_BT,
                                       bankTransactions.OTHERREF_BT,
                                       bankTransactions.BANKCODE_BT,
                                       bankTransactions.CREDITAMOUT_BT,
                                       bankTransactions.STATUS_BT,
                                       bankTransactions.DEBITAMOUT_BT
                                   }).FirstOrDefault();
                return Json(invCodeData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                // ignored
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        private void ReverseBankTransactions(BANKTRANSACTION entry)
        {
            var reverseEntry = _unitOfWork.Repository<BANKTRANSACTION>().Create();
            reverseEntry.DOCNUMBER_BT = "Rev" + entry.DOCNUMBER_BT;
            reverseEntry.BANKCODE_BT = entry.BANKCODE_BT;
            reverseEntry.DOCDATE_BT = entry.DOCDATE_BT;
            reverseEntry.GLDATE_BT = entry.GLDATE_BT;
            reverseEntry.OTHERREF_BT = entry.OTHERREF_BT;
            reverseEntry.CHQNO_BT = entry.CHQNO_BT;
            reverseEntry.CHQDATE_BT = entry.CHQDATE_BT;
            reverseEntry.CLEARANCEDATE_BT = entry.CLEARANCEDATE_BT;
            reverseEntry.BENACNO_BT = entry.BENACNO_BT;
            reverseEntry.BENACCOUNT_BT = entry.BENACCOUNT_BT;
            reverseEntry.NARRATION_BT = entry.NARRATION_BT;
            reverseEntry.NOTE_BT = entry.NOTE_BT;
            reverseEntry.MASTERSTATUS_BT = entry.MASTERSTATUS_BT;
            reverseEntry.CREDITAMOUT_BT = entry.DEBITAMOUT_BT;
            reverseEntry.DEBITAMOUT_BT = entry.CREDITAMOUT_BT;
            reverseEntry.STATUS_BT = "R";

            _unitOfWork.Repository<BANKTRANSACTION>().Insert(reverseEntry);
            _unitOfWork.Save();
        }

        private void ReverseAr_Ap_Enteries(AR_AP_LEDGER entry)
        {
            var reverseEntry = _unitOfWork.Repository<AR_AP_LEDGER>().Create();
            reverseEntry.DOCNUMBER_ART = "Rev" + entry.DOCNUMBER_ART;
            reverseEntry.DODATE_ART = entry.DODATE_ART;
            reverseEntry.GLDATE_ART = entry.GLDATE_ART;
            reverseEntry.ARAPCODE_ART = entry.ARAPCODE_ART;
            reverseEntry.OTHERREF_ART = entry.OTHERREF_ART;
            reverseEntry.NARRATION_ART = entry.NARRATION_ART;
            reverseEntry.MATCHVALUE_AR = entry.MATCHVALUE_AR;
            reverseEntry.CREDITAMOUNT_ART = entry.DEBITAMOUNT_ART;
            reverseEntry.DEBITAMOUNT_ART = entry.CREDITAMOUNT_ART;
            reverseEntry.STATUS_ART = "R";
            _unitOfWork.Repository<AR_AP_LEDGER>().Insert(reverseEntry);
            _unitOfWork.Save();
        }

        private void ReverseGlTransactions(GLTRANSACTION1 entry)
        {
            var reverseEntry = _unitOfWork.Repository<GLTRANSACTION1>().Create();
            reverseEntry.DOCNUMBER_GLT = "Rev" + entry.DOCNUMBER_GLT;
            reverseEntry.DOCDATE_GLT = entry.DOCDATE_GLT;
            reverseEntry.GLDATE_GLT = entry.GLDATE_GLT;
            reverseEntry.GLACCODE_GLT = entry.GLACCODE_GLT;
            reverseEntry.OTHERREF_GLT = entry.OTHERREF_GLT;
            reverseEntry.NARRATION_GLT = entry.NARRATION_GLT;
            reverseEntry.CREDITAMOUNT_GLT = entry.DEBITAMOUNT_GLT;
            reverseEntry.DEBITAMOUNT_GLT = entry.CREDITAMOUNT_GLT;
            reverseEntry.GLSTATUS_GLT = "R";
            _unitOfWork.Repository<GLTRANSACTION1>().Insert(reverseEntry);
            _unitOfWork.Save();
        }

        public ActionResult JvPrinting()
        {
            @ViewBag.DocType = CommonModelAccessUtility.GetDocTypes(_unitOfWork);
            return View();
        }

        public ActionResult VoucherPrinting()
        {
            @ViewBag.VoucherTypes = CommonModelAccessUtility.GetVoucherTypes();
            @ViewBag.DocType = CommonModelAccessUtility.GetDocTypes(_unitOfWork);
            return View();
        }
    }
}