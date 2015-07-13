using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASI.MGC.FS.Domain;
using ASI.MGC.FS.Model;

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
    }
}