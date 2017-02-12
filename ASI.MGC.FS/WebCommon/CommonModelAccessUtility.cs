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
        public static int GetCurrMrvCount(IUnitOfWork iUnitOfWork)
        {
            var currYear = DateTime.Now.Year.ToString();
            var mrvCount = (from objMrv in iUnitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                            //where objMrv.MRVNO_MRV.EndsWith(currYear)
                            select objMrv.MRVNO_MRV).Distinct().Count();
            return mrvCount;
        }

        public static int GetJobMasterCount(IUnitOfWork iUnitOfWork)
        {
            var currYear = DateTime.Now.Year.ToString();
            var jobCount = (from objMrv in iUnitOfWork.Repository<JOBMASTER>().Query().Get()
                            //where objMrv.JOBNO_JM.EndsWith(currYear)
                            select objMrv.JOBNO_JM).Distinct().Count();
            return jobCount;
        }

        public static int GetCashSaleCount(IUnitOfWork iUnitOfWork)
        {
            var currYear = DateTime.Now.Year.ToString();
            var cashSaleCount = (from lstBankTransaction in iUnitOfWork.Repository<BANKTRANSACTION>().Query().Get()
                                 where lstBankTransaction.DOCNUMBER_BT.Contains("RCT")
                                 //&& lstBankTransaction.DOCNUMBER_BT.EndsWith(currYear)
                                 select lstBankTransaction.DOCNUMBER_BT).Distinct().Count();
            return cashSaleCount;
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

        public static int GetInvoiceCount(IUnitOfWork iUnitOfWork)
        {
            var currYear = DateTime.Now.Year.ToString();
            var invoiceCount = (from lstArApLedger in iUnitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                                where lstArApLedger.DOCNUMBER_ART.Contains("INV")
                                //&& lstArApLedger.DOCNUMBER_ART.EndsWith(currYear)
                                select lstArApLedger.DOCNUMBER_ART).Distinct().Count();
            return invoiceCount;
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
    }
}