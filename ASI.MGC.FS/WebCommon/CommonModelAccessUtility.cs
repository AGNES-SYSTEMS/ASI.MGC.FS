using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;
using System.Web.Mvc;

namespace ASI.MGC.FS.WebCommon
{
    public class CommonModelAccessUtility
    {
        static CommonModelAccessUtility()
        {

        }

        public static int getCurrMRVCount(IUnitOfWork _iUnitOfWork)
        {
            string currYear = System.DateTime.Now.Year.ToString();
            int mrvCount = (from objMRV in _iUnitOfWork.Repository<MATERIALRECEIPTMASTER>().Query().Get()
                            where objMRV.MRVNO_MRV.EndsWith(currYear)
                            select objMRV).Count();
            return mrvCount;
        }

        public static IList<SelectListItem> getPaymentMethodList()
        {
            IList<SelectListItem> lstPaymentMethods = new List<SelectListItem>();
            lstPaymentMethods.Add(new SelectListItem { Text = "Cash", Value = "Cash", Selected = true });
            lstPaymentMethods.Add(new SelectListItem { Text = "Credit", Value = "Credit" });
            return lstPaymentMethods;
        }

        public static IList<SelectListItem> getSaleTypeList()
        {
            IList<SelectListItem> lstSaleType = new List<SelectListItem>();
            lstSaleType.Add(new SelectListItem { Text = "Product", Value = "Product", Selected = true });
            lstSaleType.Add(new SelectListItem { Text = "SOW", Value = "SOW" });
            return lstSaleType;
        }

    }
}