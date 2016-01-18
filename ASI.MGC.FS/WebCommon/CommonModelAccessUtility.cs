using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;

namespace ASI.MGC.FS.WebCommon
{
    public class CommonModelAccessUtility
    {
        public static int GetCurrMrvCount(IUnitOfWork iUnitOfWork)
        {
            var currYear = DateTime.Now.Year.ToString();
            var mrvCount = (from objMrv in iUnitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                            where objMrv.MRVNO_MRV.EndsWith(currYear)
                            select objMrv.MRVNO_MRV).Distinct().Count();
            return mrvCount;
        }

        public static int GetJobMasterCount(IUnitOfWork iUnitOfWork)
        {
            var currYear = DateTime.Now.Year.ToString();
            var jobCount = (from objMrv in iUnitOfWork.Repository<JOBMASTER>().Query().Get()
                            where objMrv.JOBNO_JM.EndsWith(currYear)
                            select objMrv.JOBNO_JM).Distinct().Count();
            return jobCount;
        }

        public static int GetCashSaleCount(IUnitOfWork iUnitOfWork)
        {
            var currYear = DateTime.Now.Year.ToString();
            var cashSaleCount = (from lstBankTransaction in iUnitOfWork.Repository<BANKTRANSACTION>().Query().Get()
                                 where lstBankTransaction.DOCNUMBER_BT.Contains("RCT") && lstBankTransaction.DOCNUMBER_BT.EndsWith(currYear)
                                 select lstBankTransaction.DOCNUMBER_BT).Distinct().Count();
            return cashSaleCount;
        }

        public static int GetCurrPurchaseCount(IUnitOfWork iUnitOfWork)
        {
            var currYear = DateTime.Now.Year.ToString();
            var purCount = (from lstArApLedger in iUnitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                            where lstArApLedger.DOCNUMBER_ART.Contains("CRP") && lstArApLedger.DOCNUMBER_ART.EndsWith(currYear)
                            select lstArApLedger.DOCNUMBER_ART).Distinct().Count();
            return purCount;
        }

        public static int GetInvoiceCount(IUnitOfWork iUnitOfWork)
        {
            var currYear = DateTime.Now.Year.ToString();
            var invoiceCount = (from lstArApLedger in iUnitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                                where lstArApLedger.DOCNUMBER_ART.Contains("INV") && lstArApLedger.DOCNUMBER_ART.EndsWith(currYear)
                                select lstArApLedger.DOCNUMBER_ART).Distinct().Count();
            return invoiceCount;
        }

        public static int GetQuotationCount(IUnitOfWork iUnitOfWork)
        {
            var currYear = DateTime.Now.Year.ToString();
            var quotCount = (from quotations in iUnitOfWork.Repository<QUOTATION_MASTER>().Query().Get()
                                where quotations.QUOTNO_QM.Contains("QOT") && quotations.QUOTNO_QM.EndsWith(currYear)
                                select quotations.QUOTNO_QM).Distinct().Count();
            return quotCount;
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
            lstSaleType.Add(new SelectListItem { Text = "Product", Value = "Product", Selected = true });
            lstSaleType.Add(new SelectListItem { Text = "SOW", Value = "SOW" });
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
                docDictionary.Add(count,doc);
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
    }
}