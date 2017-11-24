using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Domain.Repositories;
using ASI.MGC.FS.Model;
using ASI.MGC.FS.Models;

namespace ASI.MGC.FS.WebCommon
{
    public class CommonModelAccessUtility
    {
        public static string GetCurrMrvCount(IUnitOfWork iUnitOfWork)
        {
            var currYear = DateTime.Now.Year.ToString();
            var mrvCount = (from objMrv in iUnitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                                //where objMrv.MRVNO_MRV.EndsWith(currYear)
                            select objMrv.MRVNO_MRV).Distinct().Count();
            string mrvCode = "MRV/" + (1001 + mrvCount) + "/" + currYear;
            return mrvCode;
        }

        public static int GetJobMasterCount(IUnitOfWork iUnitOfWork)
        {
            var currYear = DateTime.Now.Year.ToString();
            var jobCount = (from objMrv in iUnitOfWork.Repository<JOBMASTER>().Query().Get()
                                //where objMrv.JOBNO_JM.EndsWith(currYear)
                            select objMrv.JOBNO_JM).Distinct().Count();
            return jobCount;
        }

        public static string GetCashSaleCount(IUnitOfWork iUnitOfWork)
        {
            var currYear = DateTime.Now.Year;
            var cashSaleCount = (from lstBankTransaction in iUnitOfWork.Repository<BANKTRANSACTION>().Query().Get()
                                 where lstBankTransaction.DOCNUMBER_BT.StartsWith("RCT") && lstBankTransaction.GLDATE_BT.Value.Year == currYear
                                 select lstBankTransaction.DOCNUMBER_BT).Distinct().Count();
            int cmCount = cashSaleCount + 1 + 1000;
            if (currYear == 2017)
            {
                DateTime startDate = Convert.ToDateTime("2017/01/01");
                DateTime endDate = Convert.ToDateTime("2017/05/05");
                var revCashSaleCount = (from lstBankTransaction in iUnitOfWork.Repository<BANKTRANSACTION>().Query().Get()
                                        where lstBankTransaction.DOCNUMBER_BT.StartsWith("RevRCT") && (lstBankTransaction.GLDATE_BT >= startDate && lstBankTransaction.GLDATE_BT <= endDate)
                                        select lstBankTransaction.DOCNUMBER_BT).Distinct().Count();
                cmCount = cmCount + revCashSaleCount;
            }
            string cashMemoCode = Convert.ToString("RCT/" + Convert.ToString(cmCount) + "/" + currYear);
            return cashMemoCode;
        }

        public static int GetCurrPurchaseCount(IUnitOfWork iUnitOfWork)
        {
            var currYear = DateTime.Now.Year.ToString();
            var purCount = (from lstArApLedger in iUnitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                            where lstArApLedger.DOCNUMBER_ART.Contains("CRP")
                            //&& lstArApLedger.DOCNUMBER_ART.EndsWith(currYear)
                            select lstArApLedger.DOCNUMBER_ART).Distinct().Count();
            return purCount;
        }

        public static string GetInvoiceCount(IUnitOfWork iUnitOfWork)
        {
            var currYear = DateTime.Now.Year;
            var invoiceCount = (from lstArApLedger in iUnitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                                where lstArApLedger.DOCNUMBER_ART.StartsWith("INV") && lstArApLedger.GLDATE_ART.Value.Year == currYear
                                select lstArApLedger.DOCNUMBER_ART).Distinct().Count();
            int invCount = invoiceCount + 1 + 1000;
            string cashMemoCode = Convert.ToString("INV/" + Convert.ToString(invCount) + "/" + currYear);
            return cashMemoCode;
        }

        public static int GetQuotationCount(IUnitOfWork iUnitOfWork)
        {
            var currYear = DateTime.Now.Year.ToString();
            var quotCount = (from quotations in iUnitOfWork.Repository<QUOTATION_MASTER>().Query().Get()
                             where quotations.QUOTNO_QM.Contains("QOT")
                             //&& quotations.QUOTNO_QM.EndsWith(currYear)
                             select quotations.QUOTNO_QM).Distinct().Count();
            return quotCount;
        }

        public static string GetDlnNumber(IUnitOfWork iUnitOfWork)
        {
            string dlnNumber = "";
            var currYear = DateTime.Now.Year.ToString();
            var repo = iUnitOfWork.ExtRepositoryFor<ReportRepository>();
            var lastDlnNumber = repo.sp_GetLastDeliveryNumber();
            if (!string.IsNullOrEmpty(lastDlnNumber))
            {
                int dlnCount = Convert.ToInt32(lastDlnNumber.Split('/')[1]) + 1;
                dlnNumber = "DLN/" + dlnCount + "/" + currYear;

            }
            return dlnNumber;
        }
        public static string GetDeleNumberCount(IUnitOfWork iUnitOfWork)
        {
            var currYear = DateTime.Now.Year;
            var cashSaleCount = (from lstBankTransaction in iUnitOfWork.Repository<BANKTRANSACTION>().Query().Get()
                                 where lstBankTransaction.DOCNUMBER_BT.Contains("RCT") && lstBankTransaction.GLDATE_BT.Value.Year == currYear
                                 select lstBankTransaction.DOCNUMBER_BT).Distinct().Count();
            var invSaleCount = (from lstArTransaction in iUnitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                                where lstArTransaction.DOCNUMBER_ART.Contains("INV") && lstArTransaction.GLDATE_ART.Value.Year == currYear
                                select lstArTransaction.DOCNUMBER_ART).Distinct().Count();
            int delCount = cashSaleCount + invSaleCount + 1000;
            return "DLN/" + delCount + "/" + currYear;
        }
        public static IList<SelectListItem> GetPaymentMethodList()
        {
            IList<SelectListItem> lstPaymentMethods = new List<SelectListItem>();
            lstPaymentMethods.Add(new SelectListItem { Text = "Cash", Value = "Cash", Selected = true });
            lstPaymentMethods.Add(new SelectListItem { Text = "Credit", Value = "Credit" });
            return lstPaymentMethods;
        }

        public static IList<SelectListItem> GetSaleTypeList()
        {
            IList<SelectListItem> lstSaleType = new List<SelectListItem>();
            lstSaleType.Add(new SelectListItem { Text = "SOW", Value = "SOW", Selected = true });
            lstSaleType.Add(new SelectListItem { Text = "Product", Value = "Product" });
            return lstSaleType;
        }

        public static SALEDETAIL GetSaleDetailByMrv(string mrvNumber, IUnitOfWork iUnitOfWork)
        {
            var objSaleDetail = (from saleDetails in iUnitOfWork.Repository<SALEDETAIL>().Query().Get()
                                 where saleDetails.MRVNO_SD.Equals(mrvNumber)
                                 select saleDetails).FirstOrDefault();
            return objSaleDetail;
        }

        public static JOBMASTER GetJobDetailByMrv(string mrvNumber, IUnitOfWork iUnitOfWork)
        {
            var objJobMaster = (from jobMaster in iUnitOfWork.Repository<JOBMASTER>().Query().Get()
                                where jobMaster.MRVNO_JM.Equals(mrvNumber)
                                select jobMaster).FirstOrDefault();
            return objJobMaster;
        }

        public static IList<SelectListItem> GetSearchTypeList()
        {
            IList<SelectListItem> lstSearchType = new List<SelectListItem>();
            lstSearchType.Add(new SelectListItem { Text = "AR Details", Value = "ArDetails", Selected = true });
            lstSearchType.Add(new SelectListItem { Text = "Others", Value = "other" });
            return lstSearchType;
        }
        public static Dictionary<int, string> GetDocTypes(IUnitOfWork iUnitOfWork)
        {
            var docTypes = (from jobMaster in iUnitOfWork.Repository<DOCCUMENTMASTER>().Query().Get()
                            select jobMaster).Select(o => o.DOCABBREVIATION_DM);
            var docDictionary = new Dictionary<int, string>();
            int count = 1;
            foreach (var doc in docTypes)
            {
                docDictionary.Add(count, doc);
                count += 1;
            }

            return docDictionary;
        }
        public static Dictionary<int, string> GetAccountsType()
        {
            var accountsDictionary = new Dictionary<int, string>
            {
                {1, "Asset"},
                {2,"Liability" },
                {3,"Income" },
                {4,"Expense" }
            };

            return accountsDictionary;
        }
        public static Dictionary<int, string> GetBalanceType()
        {
            var balanceDictionary = new Dictionary<int, string>
            {
                {1, "Debit"},
                {2,"Credit" }
            };

            return balanceDictionary;
        }
        public static Dictionary<string, string> GetGlType()
        {
            var accountsDictionary = new Dictionary<string, string>
            {
                {"P", "Posting"},
                {"T","Title" }
            };

            return accountsDictionary;
        }
        public static Dictionary<int, string> GetBankModes()
        {
            var bankModeDictionary = new Dictionary<int, string>
            {
                {1, "Bank"},
                {2,"Cash" },
                {3,"Collection Cash" },
                {4,"PDC" },
                {5,"Collection Cheque" },
                {6,"Collection PDC" }
            };

            return bankModeDictionary;
        }
        public static Dictionary<int, string> GetBankStatus()
        {
            var bankStatusDictionary = new Dictionary<int, string>
            {
                {1, "Active"},
                {2,"Closed" },
                {3,"Temp Closed" },
                {4,"Limit Full" }
            };

            return bankStatusDictionary;
        }
        public static int GetReturnPurchaseCount(IUnitOfWork iUnitOfWork)
        {
            var currYear = DateTime.Now.Year.ToString();
            var revCount = (from lstArApLedger in iUnitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                            where lstArApLedger.DOCNUMBER_ART.Contains("RPC") && lstArApLedger.DOCNUMBER_ART.EndsWith(currYear)
                            select lstArApLedger.DOCNUMBER_ART).Distinct().Count();
            return revCount;
        }
        public static IList<SelectListItem> GetUserRoles(IUnitOfWork iUnitOfWork)
        {
            IList<SelectListItem> userRoles = new List<SelectListItem>();
            var roles = (from mesRoles in iUnitOfWork.Repository<MESRole>().Query().Get()
                         where mesRoles.isActive.Equals(true)
                         select mesRoles);
            foreach (var role in roles)
            {
                userRoles.Add(new SelectListItem { Text = role.RoleName, Value = Convert.ToString(role.RoleID) });
            }
            return userRoles;
        }
        public static string GetCurrentUser(IUnitOfWork iUnitOfWork)
        {
            string currUserCode = null;
            var currentUser = (from users in iUnitOfWork.Repository<MESUser>().Query().Get()
                               where users.Email.Equals(HttpContext.Current.User.Identity.Name)
                               select users).Select(a => new
                               {
                                   a.UserName
                               }).FirstOrDefault();
            if (currentUser != null)
            {
                currUserCode = currentUser.UserName;
            }
            return currUserCode;
        }
        public static Dictionary<int, string> GetVoucherTypes()
        {
            var voucherDictionary = new Dictionary<int, string>
            {
                {1, "Cash Payment"},
                {2, "Cash Receipt" },
                {3, "Bank Payment"},
                {4, "Bank Receipt" }
            };

            return voucherDictionary;
        }
        public static string GetDocNo(IUnitOfWork unitOfWork, string docType)
        {
            string docNo = "";
            var currYear = DateTime.Now.Year;
            var docCount = (from objMrv in unitOfWork.Repository<NOGENERATOR>().Query().Get()
                            where objMrv.DOCTYPE_NG.Contains(docType) && objMrv.FINYEAR_NG == (currYear)
                            select objMrv.SLNO_NG).SingleOrDefault();
            if (docCount == null)
            {
                docNo = Convert.ToString(docType + "/" + 1001 + "/" + currYear);
            }
            else
            {
                docNo = Convert.ToString(docType + "/" + (docCount + 1) + "/" + currYear);
            }
            return docNo;
        }
        public static void updateDocNo(IUnitOfWork unitOfWork, string docType)
        {
            var currYear = DateTime.Now.Year;
            var doc = (from objMrv in unitOfWork.Repository<NOGENERATOR>().Query().Get()
                       where objMrv.DOCTYPE_NG.Contains(docType) && objMrv.FINYEAR_NG == (currYear)
                       select objMrv).SingleOrDefault();
            if (doc == null)
            {
                doc = new NOGENERATOR();
                doc.DOCTYPE_NG = docType;
                doc.SLNO_NG = 1001;
                doc.FINYEAR_NG = currYear;
                unitOfWork.Repository<NOGENERATOR>().Insert(doc);
                unitOfWork.Save();
            }
            else
            {
                doc.SLNO_NG = doc.SLNO_NG + 1;
                unitOfWork.Repository<NOGENERATOR>().Update(doc);
                unitOfWork.Save();
            }
        }
        public static DayEndOperationModel InitializeDayEndObj(IUnitOfWork _unitofWork)
        {
            var objDayEnd = new DayEndOperationModel();
            var period = (from objPeriod in _unitofWork.Repository<FINYEARMASTER>().Query().Get()
                          where objPeriod.CURRENTPERIOD_FM == true
                          select objPeriod).SingleOrDefault();
            var lastDayEndOprDate = (from objSaleDetails in _unitofWork.Repository<SALEDETAIL>().Query().Get()
                                     where objSaleDetails.DAYENDDOC_NO != null && objSaleDetails.SALEDATE_SD.Value.Year == period.PERRIEDFROM_FM.Year
                                     select objSaleDetails).Max(o => o.SALEDATE_SD);
            var lastDayEndOprDocNo = (from objSaleDetails in _unitofWork.Repository<SALEDETAIL>().Query().Get()
                                      where objSaleDetails.DAYENDDOC_NO != null && objSaleDetails.SALEDATE_SD.Value.Year == period.PERRIEDFROM_FM.Year
                                      select objSaleDetails).Max(o => o.DAYENDDOC_NO);
            objDayEnd.LastUpdateDate = lastDayEndOprDate != null ? Convert.ToDateTime(lastDayEndOprDate).ToShortDateString() : Convert.ToDateTime(period.PERRIEDFROM_FM).ToShortDateString();
            objDayEnd.DayFrom = lastDayEndOprDate != null ? Convert.ToDateTime(lastDayEndOprDate).AddDays(1).ToShortDateString() : Convert.ToDateTime(period.PERRIEDFROM_FM).ToShortDateString();
            objDayEnd.LastDocumentNo = lastDayEndOprDocNo != null ? lastDayEndOprDocNo : "";
            if (!string.IsNullOrEmpty(lastDayEndOprDocNo))
            {
                var arrDocNo = lastDayEndOprDocNo.Split('/');
                objDayEnd.DocumentNo = arrDocNo[0] + "/" + (Convert.ToInt32(arrDocNo[1]) + 1) + "/" + arrDocNo[2];
            }
            else
            {
                objDayEnd.DocumentNo = "DJV/1000/" + Convert.ToDateTime(period.PERRIEDFROM_FM).Year;
            }
            return objDayEnd;
        }
        public static Dictionary<int, string> GetDocTypesForPrint(IUnitOfWork _unitofWork)
        {
            var docDictionary = new Dictionary<int, string>();
            docDictionary.Add(0, "MRV");
            docDictionary.Add(1, "DLN");
            docDictionary.Add(2, "INV");
            docDictionary.Add(3, "RCT");
            docDictionary.Add(4, "JOB");
            docDictionary.Add(5, "QOT");
            docDictionary.Add(6, "STJ");
            return docDictionary;
        }

        public static void Proc_ConverUnit(string Unit, double QTY, decimal RATE_Renamed, out double ConvertedQty, out string ConvetedUnit, out decimal ConvrtedRate, IUnitOfWork _unitofWork)
        {
            ConvertedQty = 0;
            ConvetedUnit = "";
            decimal a = default(decimal);
            decimal b = default(decimal);
            decimal UnitQty = default(decimal);
            decimal BasicQty = default(decimal);
            string BasicUnit = null;
            ConvrtedRate = 0;
            var unit = (from unitMessurement in _unitofWork.Repository<UNITMESSUREMENT>().Query().Get()
                        where unitMessurement.UNIT_UM.Equals(Unit)
                        select unitMessurement).SingleOrDefault();
            if (unit != null)
            {
                UnitQty = Convert.ToDecimal(unit.UNITQTY_UM != null ? unit.UNITQTY_UM : 0);
                BasicQty = Convert.ToDecimal(unit.BASICPRIMARYQTY_UM != null ? unit.BASICPRIMARYQTY_UM : 0);
                BasicUnit = unit.BASICPRIMARYUNIT_UM != null ? unit.BASICPRIMARYUNIT_UM : "";
                a = BasicQty / UnitQty;
                ConvertedQty = Convert.ToDouble(QTY * Convert.ToDouble(a));
                ConvrtedRate = RATE_Renamed / a;
            }
        }
    }
}