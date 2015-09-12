using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;
using Microsoft.Ajax.Utilities;

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

        public static int GetInvoiceCount(IUnitOfWork iUnitOfWork)
        {
            var currYear = DateTime.Now.Year.ToString();
            var invoiceCount = (from lstArApLedger in iUnitOfWork.Repository<AR_AP_LEDGER>().Query().Get()
                                where lstArApLedger.DOCNUMBER_ART.Contains("INV") && lstArApLedger.DOCNUMBER_ART.EndsWith(currYear)
                                select lstArApLedger.DOCNUMBER_ART).Distinct().Count();
            return invoiceCount;
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

    }
}